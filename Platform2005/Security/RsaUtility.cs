namespace Platform.Security
{
    using Platform.Caching;
    using Platform.ExceptionHandling;
    using System;
    using System.Collections;
    using System.Security.Cryptography;

    public sealed class RsaUtility : IBufferItem
    {
        private int m_BlockLength;
        private RSAParameters m_Key;
        private RSACryptoServiceProvider m_Rsa;
        private static object m_SyncRoot = new object();

        public RsaUtility(int size)
        {
            this.m_Rsa = new RSACryptoServiceProvider(size);
        }

        public RsaUtility(RSAParameters key)
        {
            lock (m_SyncRoot)
            {
                this.m_Key = key;
                this.m_BlockLength = key.Modulus.Length;
                this.m_Rsa = new RSACryptoServiceProvider(this.m_BlockLength * 8);
                this.m_Rsa.ImportParameters(this.m_Key);
            }
        }

        public RSAParameters ExportKey(bool includePrivate)
        {
            return this.m_Rsa.ExportParameters(includePrivate);
        }

        public void FinalRelease()
        {
        }

        public void ImportKey(RSAParameters key)
        {
            lock (m_SyncRoot)
            {
                this.m_Key = key;
                this.m_Rsa.ImportParameters(key);
                this.m_BlockLength = this.m_Rsa.KeySize / 8;
            }
        }

        public byte[] PrivateDecrypt(byte[] input)
        {
            try
            {
                if ((input.Length % this.m_BlockLength) != 0)
                {
                    return null;
                }
                int num = input.Length / this.m_BlockLength;
                if (num < 2)
                {
                    return this.m_Rsa.Decrypt(input, false);
                }
                byte[] dst = new byte[this.m_BlockLength];
                ArrayList list = new ArrayList();
                int dstOffset = 0;
                for (int i = 0; i < num; i++)
                {
                    Buffer.BlockCopy(input, i * this.m_BlockLength, dst, 0, dst.Length);
                    byte[] buffer2 = this.m_Rsa.Decrypt(dst, false);
                    list.Add(buffer2);
                    dstOffset += buffer2.Length;
                }
                byte[] buffer3 = new byte[dstOffset];
                dstOffset = 0;
                for (int j = 0; j < num; j++)
                {
                    byte[] src = list[j] as byte[];
                    Buffer.BlockCopy(src, 0, buffer3, dstOffset, src.Length);
                    dstOffset += src.Length;
                }
                return buffer3;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public byte[] PublicEncrypt(byte[] input)
        {
            try
            {
                int num;
                if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
                {
                    num = 0x10;
                }
                else
                {
                    num = this.m_BlockLength - 11;
                }
                int length = input.Length;
                int num3 = ((length + num) - 1) / num;
                if (num3 < 2)
                {
                    return this.m_Rsa.Encrypt(input, false);
                }
                byte[] dst = new byte[this.m_BlockLength * num3];
                byte[] buffer2 = new byte[num];
                for (int i = 0; i < num3; i++)
                {
                    if (i == (num3 - 1))
                    {
                        buffer2 = new byte[length - ((num3 - 1) * num)];
                    }
                    Buffer.BlockCopy(input, i * num, buffer2, 0, buffer2.Length);
                    Buffer.BlockCopy(this.m_Rsa.Encrypt(buffer2, false), 0, dst, i * this.m_BlockLength, this.m_BlockLength);
                }
                return dst;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public byte[] Sign(byte[] data, HashUtility hash)
        {
            try
            {
                byte[] rgbHash = hash.ComputeHash(data);
                return this.m_Rsa.SignHash(rgbHash, hash.HashOID);
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public byte[] Sign(byte[] data, HashAlgorithm hash, string hashOID)
        {
            try
            {
                byte[] rgbHash = hash.ComputeHash(data);
                return this.m_Rsa.SignHash(rgbHash, hashOID);
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public bool Verify(byte[] data, byte[] sign, HashUtility hash)
        {
            try
            {
                byte[] rgbHash = hash.ComputeHash(data);
                return this.m_Rsa.VerifyHash(rgbHash, hash.HashOID, sign);
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public bool Verify(byte[] data, byte[] sign, HashAlgorithm hash, string hashOID)
        {
            try
            {
                byte[] rgbHash = hash.ComputeHash(data);
                return this.m_Rsa.VerifyHash(rgbHash, hashOID, sign);
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }
    }
}
