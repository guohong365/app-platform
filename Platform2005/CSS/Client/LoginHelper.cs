namespace Platform.CSS.Client
{
    using Platform.CSS.Communication;
    using Platform.CSS.Packet;
    using Platform.Identity;
    using Platform.Security;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public sealed class LoginHelper
    {
        private static Hashtable m_Errors = new Hashtable();
        private static MacErrorReloginHandler m_MacErrorRelogin = null;

        static LoginHelper()
        {
            m_Errors[0] = "成功";
            m_Errors[-1] = "通讯失败";
            m_Errors[-2] = "返回数据期待格式错误";
            m_Errors[-3] = "密钥校验错误";
            m_Errors[-4] = "用户数据错误";
            m_Errors[-5] = "登录异常";
            m_Errors[1] = "发送数据格式错误";
            m_Errors[2] = "解密工作密钥错误";
            m_Errors[3] = "工作密钥数据错误";
            m_Errors[4] = "工作密钥数据错误";
            m_Errors[5] = "解密用户数据错误";
            m_Errors[6] = "获取用户失败（用户名称或密码错误）";
            m_Errors[7] = "用户返回数据错误";
            m_Errors[8] = "该帐号已经被人登录";
            m_Errors[9] = "设置用户密钥错误";
            m_Errors[10] = "用户信息错误";
            m_Errors[11] = "删除用户登录信息错误";
        }

        public static int DoLogin(byte[] userInfo, out IUser user, out byte[] userData, out int error)
        {
            byte[] buffer;
            byte[] buffer2;
            error = 0;
            user = null;
            userData = null;
            Platform.Security.Security.GenerateSymmetricKeyIV(out buffer, out buffer2);
            byte[] dst = new byte[(buffer.Length + buffer2.Length) + 8];
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, dst, 0, 4);
            Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer2.Length), 0, dst, buffer.Length + 4, 4);
            Buffer.BlockCopy(buffer2, 0, dst, buffer.Length + 8, buffer2.Length);
            LoginPacket packet = new LoginPacket();
            packet.UserData = Platform.Security.Security.SymmetricCrypt(userInfo, buffer, buffer2, true);
            packet.KeyData = Platform.Security.Security.RsaPublicEncrypt(dst);
            packet.IsLogin = true;
            packet.IsReturn = false;
            LoginPacket packet2 = ClientCommunication.SendPacket(packet) as LoginPacket;
            if (packet2 == null)
            {
                return -1;
            }
            if (!packet2.IsReturn || !packet2.IsLogin)
            {
                return -2;
            }
            error = packet2.LoginCode;
            if (packet2.ReturnCode != 0)
            {
                return packet2.ReturnCode;
            }
            if (!Platform.Security.Security.RsaVerify(dst, packet2.KeyData))
            {
                return -3;
            }
            if (packet2.UserData == null)
            {
                return -4;
            }
            user = UserManager.CreateUserInstance();
            user.PlatformUserID = packet2.UserID;
            user.SetSymmetricKey(buffer, buffer2);
            UserManager.AddUser(user);
            UserManager.SetCurrentThreadUser(user);
            userData = Platform.Security.Security.SymmetricCrypt(packet2.UserData, user.SymmetricDecryptTransform);
            return 0;
        }

        public static int DoLogout(byte[] userInfo, IUser user, out byte[] userData, out int error)
        {
            error = 0;
            userData = new byte[0];
            if (user != null)
            {
                LoginPacket packet = new LoginPacket();
                packet.UserData = Platform.Security.Security.SymmetricCrypt(userInfo, user.SymmetricEncryptTransform);
                byte[] data = Platform.Security.Security.SymmetricCrypt(user.PlatformUserID.ToByteArray(), user.SymmetricEncryptTransform);
                packet.KeyData = data;
                packet.IsLogin = false;
                packet.IsReturn = false;
                packet.UserID = user.PlatformUserID;
                LoginPacket packet2 = ClientCommunication.SendPacket(packet) as LoginPacket;
                if (packet2 == null)
                {
                    return -1;
                }
                if (!packet2.IsReturn || packet2.IsLogin)
                {
                    return -2;
                }
                error = packet2.LoginCode;
                if (packet2.ReturnCode != 0)
                {
                    return packet2.ReturnCode;
                }
                if (!Platform.Security.Security.RsaVerify(data, packet2.KeyData))
                {
                    return -3;
                }
                if (packet2.UserData == null)
                {
                    return -4;
                }
                userData = Platform.Security.Security.SymmetricCrypt(packet2.UserData, user.SymmetricDecryptTransform);
                UserManager.RemoveUser(user);
                UserManager.CleanCurrentThreadUser();
            }
            return 0;
        }

        public static string GetErrorMessage(int errorCode)
        {
            string text = m_Errors[errorCode] as string;
            if (text == null)
            {
               return ("未知错误！ （错误代码 " + errorCode.ToString() + "）");
            }
            return (text + " （错误代码 " + errorCode.ToString() + "）");
        }

        public static bool Relogin(string settingName)
        {
            if (m_MacErrorRelogin == null)
            {
                return false;
            }
            return m_MacErrorRelogin(settingName);
        }

        public static void RregisterMacErrorReloginHandler(MacErrorReloginHandler handler)
        {
            m_MacErrorRelogin = handler;
        }
    }
}
