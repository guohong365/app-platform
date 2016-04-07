namespace Platform.Security
{
    using Platform.Configuration;
    using System;
    using System.Security.Cryptography;

    public sealed class SecurityConfig
    {
        [ConfigItem("/PlatformSettings", "MKeyD", "HexStringToBytesConverter")]
        private static byte[] D = null;
        [ConfigItem("/PlatformSettings", "MKeyDP", "HexStringToBytesConverter")]
        private static byte[] DP = null;
        [ConfigItem("/PlatformSettings", "MKeyDQ", "HexStringToBytesConverter")]
        private static byte[] DQ = null;
        [ConfigItem("/PlatformSettings", "MKeyExponent", "HexStringToBytesConverter")]
        private static byte[] Exponent = null;
        [ConfigItem("/PlatformSettings", "HashType", null)]
        public static Platform.Security.HashType HashType;
        [ConfigItem("/PlatformSettings", "MKeyInverseQ", "HexStringToBytesConverter")]
        private static byte[] InverseQ = null;
        private static bool m_HasRsaKeyCreated = false;
        private static readonly int[] m_IVSizes = new int[] { 0x40, 0x40, 0x80, 0x40 };
        private static readonly int[] m_KeySizes = new int[] { 0x40, 0x80, 0x100, 0xc0 };
        private static RSAParameters m_RsaKey;
        [ConfigItem("/PlatformSettings", "MKeyModulus", "HexStringToBytesConverter")]
        private static byte[] Modulus = null;
        [ConfigItem("/PlatformSettings", "MKeyP", "HexStringToBytesConverter")]
        private static byte[] P = null;
        [ConfigItem("/PlatformSettings", "MKeyQ", "HexStringToBytesConverter")]
        private static byte[] Q = null;
        public static int RsaKeySize = 0x400;
        [ConfigItem("/PlatformSettings", "SymmetricType", null)]
        public static Platform.Security.SymmetricType SymmetricType;

        private static void BeforeInitialize()
        {
        }

        public static int IVSize
        {
            get
            {
                return m_IVSizes[(int) SymmetricType];
            }
        }

        public static int KeySize
        {
            get
            {
                return m_KeySizes[(int) SymmetricType];
            }
        }

        public static RSAParameters RsaKey
        {
            get
            {
                if (!m_HasRsaKeyCreated)
                {
                    m_RsaKey = new RSAParameters();
                    m_RsaKey.Exponent = Exponent;
                    m_RsaKey.Modulus = Modulus;
                    if ((((P != null) && (Q != null)) && (D != null)) || (((DP != null) || (DQ != null)) || (InverseQ != null)))
                    {
                        m_RsaKey.P = P;
                        m_RsaKey.Q = Q;
                        m_RsaKey.D = D;
                        m_RsaKey.DP = DP;
                        m_RsaKey.DQ = DQ;
                        m_RsaKey.InverseQ = InverseQ;
                    }
                    m_HasRsaKeyCreated = true;
                }
                return m_RsaKey;
            }
        }
    }
}
