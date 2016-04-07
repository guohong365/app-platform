namespace Platform.CSS.Server
{
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Communication.PacketHandler;
    using Platform.CSS.Packet;
    using Platform.Identity;
    using Platform.Log;
    using Platform.Security;
    using System;
    using System.Runtime.CompilerServices;

    public sealed class LoginPacketHandler : ICommunicationPacketHandler
    {
        public static event OnUserLoginFinishHandler OnUserLoginFinish;

        public static event OnUserLogoutFinishHandler OnUserLogoutFinish;

        public CommunicationPacketBase HandlePacket(Guid packetID, CommunicationPacketBase packet)
        {
            LoginPacket loginPacket = packet as LoginPacket;
            if (loginPacket.IsLogin)
            {
                return this.OnLogin(loginPacket);
            }
            return this.OnLogout(loginPacket);
        }

        private LoginPacket OnLogin(LoginPacket loginPacket)
        {
            LoginPacket packet;
            string[] msg = new string[0];
            string platformMsg = "";
            IUser user = null;
            try
            {
                byte[] buffer7;
                int num3;
                UserManager.CleanCurrentThreadUser();
                if (loginPacket.IsReturn)
                {
                    platformMsg = "登录请求数据类型错误";
                    loginPacket.ReturnCode = 1;
                    return loginPacket;
                }
                byte[] keyData = loginPacket.KeyData;
                byte[] userData = loginPacket.UserData;
                loginPacket.Reset();
                loginPacket.IsLogin = true;
                loginPacket.IsReturn = true;
                byte[] buffer3 = Platform.Security.Security.RsaPrivateDecrypt(keyData);
                if ((buffer3 == null) || (buffer3.Length < 8))
                {
                    platformMsg = "登录工作密钥数据错误";
                    loginPacket.ReturnCode = 2;
                    return loginPacket;
                }
                int count = BitConverter.ToInt32(buffer3, 0);
                if ((count <= 0) || (count > (buffer3.Length - 8)))
                {
                    platformMsg = "登录工作密钥数据错误";
                    loginPacket.ReturnCode = 3;
                    return loginPacket;
                }
                byte[] dst = new byte[count];
                Buffer.BlockCopy(buffer3, 4, dst, 0, count);
                int num2 = BitConverter.ToInt32(buffer3, count + 4);
                if (num2 != ((buffer3.Length - 8) - count))
                {
                    platformMsg = "登录工作密钥数据错误";
                    loginPacket.ReturnCode = 4;
                    return loginPacket;
                }
                byte[] buffer5 = new byte[num2];
                Buffer.BlockCopy(buffer3, count + 8, buffer5, 0, num2);
                byte[] userIdentities = Platform.Security.Security.SymmetricCrypt(userData, dst, buffer5, false);
                if (userIdentities == null)
                {
                    platformMsg = "登录工作密钥数据解密错误";
                    loginPacket.ReturnCode = 5;
                    return loginPacket;
                }
                user = UserManager.LoadUser(userIdentities, out buffer7, out num3, out platformMsg, out msg);
                loginPacket.LoginCode = num3;
                if (user == null)
                {
                    platformMsg = "用户名称和口令校验失败";
                    loginPacket.ReturnCode = 6;
                    return loginPacket;
                }
                if (buffer7 == null)
                {
                    platformMsg = "用户返回数据错误";
                    loginPacket.ReturnCode = 7;
                    return loginPacket;
                }
                bool flag = user.CheckIPAddress(loginPacket.IPEndPoint.Address);
                if (!flag)
                {
                    flag = user.CheckActive(LoginHandlersConfig.LoginLimitation);
                }
                if (!flag)
                {
                    platformMsg = "用户重复登录";
                    loginPacket.ReturnCode = 8;
                    return loginPacket;
                }
                if (!user.SetSymmetricKey(dst, buffer5))
                {
                    platformMsg = "设置用户密钥错误";
                    loginPacket.ReturnCode = 9;
                    return loginPacket;
                }
                loginPacket.UserData = Platform.Security.Security.SymmetricCrypt(buffer7, user.SymmetricEncryptTransform);
                loginPacket.UserID = user.PlatformUserID;
                loginPacket.KeyData = Platform.Security.Security.RsaSign(buffer3);
                UserManager.AddUser(user);
                user.IPAddress = loginPacket.IPEndPoint.Address;
                user.PlatformUserVersion = Guid.NewGuid();
                if (OnUserLoginFinish != null)
                {
                    OnUserLoginFinish(user, dst, buffer5);
                }
                platformMsg = "登录成功！";
                packet = loginPacket;
            }
            catch (Exception exception)
            {
                platformMsg = "异常信息：" + exception.Message;
                loginPacket.ReturnCode = -5;
                packet = loginPacket;
            }
            finally
            {
                string userInfo = "";
                try
                {
                    userInfo = (user == null) ? loginPacket.IPEndPoint.ToString() : user.ToString();
                }
                catch
                {
                }
                try
                {
                    OperationLogSink.Write(userInfo, "用户登录", "返回值 " + loginPacket.ReturnCode.ToString());
                }
                catch
                {
                }
                try
                {
                    LoginLogSink.Write("用户登录", userInfo, loginPacket.ReturnCode.ToString(), platformMsg, msg);
                }
                catch
                {
                }
            }
            return packet;
        }

        private LoginPacket OnLogout(LoginPacket loginPacket)
        {
            LoginPacket packet;
            string[] msg = new string[0];
            string platformMsg = "";
            IUser user = null;
            try
            {
                byte[] buffer5;
                int num;
                UserManager.CleanCurrentThreadUser();
                if (loginPacket.IsReturn)
                {
                    platformMsg = "登录请求数据类型错误";
                    loginPacket.ReturnCode = 1;
                    return loginPacket;
                }
                byte[] keyData = loginPacket.KeyData;
                byte[] userData = loginPacket.UserData;
                Guid userID = loginPacket.UserID;
                loginPacket.Reset();
                loginPacket.IsLogin = false;
                loginPacket.IsReturn = true;
                user = UserManager.GetUser(userID);
                if (user == null)
                {
                    platformMsg = "获取用户失败";
                    loginPacket.ReturnCode = 6;
                    return loginPacket;
                }
                byte[] b = Platform.Security.Security.SymmetricCrypt(keyData, user.SymmetricDecryptTransform);
                if (userID != new Guid(b))
                {
                    user = UserManager.LoadSessionUser(userID);
                    if (user == null)
                    {
                        platformMsg = "用户信息错误";
                        loginPacket.ReturnCode = 10;
                        return loginPacket;
                    }
                    b = Platform.Security.Security.SymmetricCrypt(keyData, user.SymmetricDecryptTransform);
                    if (userID != new Guid(b))
                    {
                        platformMsg = "用户信息错误(ID)";
                        loginPacket.ReturnCode = 10;
                        return loginPacket;
                    }
                }
                byte[] userIdentities = Platform.Security.Security.SymmetricCrypt(userData, user.SymmetricDecryptTransform);
                if (!UserManager.RemoveUser(user, userIdentities, out buffer5, out num, out platformMsg, out msg))
                {
                    platformMsg = "删除用户登录信息错误";
                    loginPacket.ReturnCode = 11;
                    return loginPacket;
                }
                if (!UserManager.RemoveSessionUser(user))
                {
                    platformMsg = "删除用户登录Session信息错误";
                    loginPacket.ReturnCode = 11;
                    return loginPacket;
                }
                loginPacket.UserData = Platform.Security.Security.SymmetricCrypt(buffer5, user.SymmetricEncryptTransform);
                loginPacket.UserID = userID;
                loginPacket.KeyData = Platform.Security.Security.RsaSign(keyData);
                if (OnUserLogoutFinish != null)
                {
                    OnUserLogoutFinish(user);
                }
                packet = loginPacket;
            }
            finally
            {
                string userInfo = "";
                try
                {
                    userInfo = (user == null) ? loginPacket.IPEndPoint.ToString() : user.ToString();
                }
                catch
                {
                }
                try
                {
                    OperationLogSink.Write(userInfo, "用户登退", "返回值 " + loginPacket.ReturnCode.ToString());
                }
                catch
                {
                }
                try
                {
                    LoginLogSink.Write("用户登退", userInfo, loginPacket.ReturnCode.ToString(), platformMsg, msg);
                }
                catch
                {
                }
            }
            return packet;
        }

        public bool CacheResult
        {
            get
            {
                return false;
            }
        }

        public bool IsBroadcastHandler
        {
            get
            {
                return false;
            }
        }
    }
}
