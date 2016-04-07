namespace Platform.CSS.SerialSink
{
    using Platform.CSS;
    using Platform.CSS.Communication.Packet;
    using Platform.Identity;
    using Platform.IO;
    using Platform.Security;
    using System;
    using System.IO;

    [SerialSink(0x10)]
    public sealed class SecuritySerialSink : ISerialSink
    {
        public Platform.IO.MemoryStream Deserial(Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            Guid empty = Guid.Empty;
            ms.Seek((long)flagOffset, SeekOrigin.Begin);
            if (!ms.Read(ref empty))
            {
                return null;
            }
            if (empty == Guid.Empty)
            {
                return null;
            }
            UserManager.SetCurrentThreadUserID(empty);
            IUser currentThreadUser = UserManager.GetCurrentThreadUser();
            if (currentThreadUser == null)
            {
                throw new SecuritySerialSinkException("安全性校验错误！代码：01");
            }
            if (CSSConfig.SecurityLevel == SecurityLevel.None)
            {
                return ms;
            }
            Platform.IO.MemoryStream stream = ms;
            ms.Seek((long)0, SeekOrigin.Begin);
            byte[] buffer = Platform.Security.Security.ComputeHash(ms, flagOffset + 0x10);
            int num = 0;
        Label_0068:
            ms = stream;
            if (CSSConfig.SecurityLevel == SecurityLevel.Full)
            {
                ms.Seek((long)0, SeekOrigin.Begin);
                Platform.IO.MemoryStream outms = new Platform.IO.MemoryStream(ms, headerLen);
                if (!Platform.Security.Security.SymmetricCrypt(ms, headerLen, ((int)ms.Length) - headerLen, currentThreadUser.SymmetricDecryptTransform, outms))
                {
                    throw new SecuritySerialSinkException("安全性校验错误！代码：02");
                }
                ms = outms;
            }
            ms.Seek((long)headerLen, SeekOrigin.Begin);
            byte[] buffer2 = Platform.Security.Security.ComputeHash(ms, (((int)ms.Length) - headerLen) - 8);
            byte[] input = new byte[buffer2.Length];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(buffer[i] ^ buffer2[i]);
            }
            if (CSSConfig.SecurityLevel == SecurityLevel.Mac)
            {
                input = Platform.Security.Security.SymmetricCrypt(input, 0, 8, currentThreadUser.SymmetricEncryptTransform);
            }
            byte[] buffer4 = new byte[8];
            ms.Seek((long)8, SeekOrigin.End);
            ms.Read(buffer4, 0, 8);
            bool flag = true;
            for (int j = 0; j < 8; j++)
            {
                if (input[j] != buffer4[j])
                {
                    flag = false;
                    break;
                }
            }
            if (!flag && (num < 1))
            {
                if (CSSConfig.SecurityLevel == SecurityLevel.Hash)
                {
                    throw new SecuritySerialSinkException("安全性校验错误！代码：03");
                }
                currentThreadUser = UserManager.LoadSessionUser(empty);
                if (currentThreadUser == null)
                {
                    throw new SecuritySerialSinkException("安全性校验错误！代码：04");
                }
                if (CSSConfig.SecurityLevel == SecurityLevel.Full)
                {
                    num++;
                    goto Label_0068;
                }
                input = Platform.Security.Security.SymmetricCrypt(input, 0, 8, currentThreadUser.SymmetricEncryptTransform);
                flag = true;
                for (int k = 0; k < 8; k++)
                {
                    if (input[k] != buffer4[k])
                    {
                        throw new SecuritySerialSinkException("安全性校验错误！代码：05");
                    }
                }
            }
            if (!flag)
            {
                throw new SecuritySerialSinkException("安全性校验错误！代码：06");
            }
            ms.SetLength(ms.Length - 8);
            return ms;
        }

        public Platform.IO.MemoryStream Serial(CommunicationPacketBase packet, Platform.IO.MemoryStream ms, int headerLen, int flagOffset)
        {
            IUser currentThreadUser = UserManager.GetCurrentThreadUser();
            if ((currentThreadUser == null) || (currentThreadUser.PlatformUserID == Guid.Empty))
            {
                return null;
            }
            ms.Seek((long)flagOffset, SeekOrigin.Begin);
            ms.Write(currentThreadUser.PlatformUserID);
            if (CSSConfig.SecurityLevel == SecurityLevel.None)
            {
                return ms;
            }
            ms.Seek((long)0, SeekOrigin.Begin);
            byte[] buffer = Platform.Security.Security.ComputeHash(ms, flagOffset + 0x10);
            ms.Seek((long)headerLen, SeekOrigin.Begin);
            byte[] buffer2 = Platform.Security.Security.ComputeHash(ms, ((int)ms.Length) - headerLen);
            byte[] buffer3 = new byte[buffer2.Length];
            for (int i = 0; i < buffer3.Length; i++)
            {
                buffer3[i] = (byte)(buffer[i] ^ buffer2[i]);
            }
            if (CSSConfig.SecurityLevel == SecurityLevel.Hash)
            {
                ms.Seek((long)0, SeekOrigin.End);
                ms.Write(buffer3, 0, 8);
                return ms;
            }
            if (CSSConfig.SecurityLevel == SecurityLevel.Mac)
            {
                buffer3 = Platform.Security.Security.SymmetricCrypt(buffer3, 0, 8, currentThreadUser.SymmetricEncryptTransform);
                ms.Seek((long)0, SeekOrigin.End);
                ms.Write(buffer3, 0, 8);
                return ms;
            }
            ms.Seek((long)0, SeekOrigin.End);
            ms.Write(buffer3, 0, 8);
            ms.Seek((long)0, SeekOrigin.Begin);
            Platform.IO.MemoryStream outms = new Platform.IO.MemoryStream(ms, headerLen);
            if (!Platform.Security.Security.SymmetricCrypt(ms, headerLen, ((int)ms.Length) - headerLen, currentThreadUser.SymmetricEncryptTransform, outms))
            {
                return null;
            }
            return outms;
        }
    }
}
