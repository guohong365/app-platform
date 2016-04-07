namespace Platform.Identity
{
    using System;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public interface IUser
    {
        bool CheckActive(int seconds);
        bool CheckIPAddress(System.Net.IPAddress ipaddr);
        bool GetBan(out string banMessage);
        void SetBan(bool ban, string banMessage);
        bool SetSymmetricKey(byte[] key, byte[] iv);
        void UpdateActiveTime();

        System.Net.IPAddress IPAddress { get; set; }

        Guid PlatformUserID { get; set; }

        Guid PlatformUserVersion { get; set; }

        ICryptoTransform SymmetricDecryptTransform { get; }

        ICryptoTransform SymmetricEncryptTransform { get; }
    }
}
