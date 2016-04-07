namespace Platform.Security
{
    using Platform.Caching;
    using Platform.ExceptionHandling;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public sealed class SymmetricUtility : IBufferItem
    {
        private ICryptoTransform m_deCryptoTransform;
        private ICryptoTransform m_enCryptoTransform;
        private SymmetricAlgorithm m_Symmetrica;
        private Platform.Security.SymmetricType m_SymmetricType;

        public SymmetricUtility(Platform.Security.SymmetricType symmetricType)
        {
            this.SymmetricType = symmetricType;
        }

        public SymmetricUtility(Platform.Security.SymmetricType symmetricType, byte[] key, byte[] iv)
        {
            this.SymmetricType = symmetricType;
            this.GenerateCryptoTransform(key, iv);
        }

        public byte[] Compute(byte[] input, int index, int count, bool encrypt)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, encrypt ? this.m_enCryptoTransform : this.m_deCryptoTransform, CryptoStreamMode.Write);
                stream2.Write(input, index, count);
                stream2.FlushFinalBlock();
                byte[] buffer = stream.ToArray();
                stream2.Close();
                return buffer;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static byte[] Compute(byte[] input, int index, int count, ICryptoTransform transform)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                stream2.Write(input, index, count);
                stream2.FlushFinalBlock();
                byte[] buffer = stream.ToArray();
                stream2.Close();
                return buffer;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static byte[] Compute(byte[] input, int index, int count, byte[] key, byte[] iv, bool encrypt, Platform.Security.SymmetricType type)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(type.ToString());
                CryptoStream stream2 = new CryptoStream(stream, encrypt ? algorithm.CreateEncryptor(key, iv) : algorithm.CreateDecryptor(key, iv), CryptoStreamMode.Write);
                stream2.Write(input, index, count);
                stream2.FlushFinalBlock();
                byte[] buffer = stream.ToArray();
                stream2.Close();
                return buffer;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public void FinalRelease()
        {
        }

        public bool GenerateCryptoTransform()
        {
            try
            {
                this.m_enCryptoTransform = this.m_Symmetrica.CreateEncryptor();
                this.m_deCryptoTransform = this.m_Symmetrica.CreateDecryptor();
                return true;
            }
            catch (Exception exception)
            {
                this.m_enCryptoTransform = null;
                this.m_deCryptoTransform = null;
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public bool GenerateCryptoTransform(byte[] key, byte[] iv)
        {
            try
            {
                this.m_Symmetrica.Key = key;
                this.m_Symmetrica.IV = iv;
                this.m_enCryptoTransform = this.m_Symmetrica.CreateEncryptor();
                this.m_deCryptoTransform = this.m_Symmetrica.CreateDecryptor();
                return true;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public ICryptoTransform GenerateDeCryptoTransform(byte[] key, byte[] iv)
        {
            try
            {
                this.m_Symmetrica.Key = key;
                this.m_Symmetrica.IV = iv;
                this.m_deCryptoTransform = this.m_Symmetrica.CreateDecryptor();
                return this.m_deCryptoTransform;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public ICryptoTransform GenerateEnCryptoTransform(byte[] key, byte[] iv)
        {
            try
            {
                this.m_Symmetrica.Key = key;
                this.m_Symmetrica.IV = iv;
                this.m_enCryptoTransform = this.m_Symmetrica.CreateEncryptor();
                return this.m_enCryptoTransform;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public void GenerateKey(out byte[] key, out byte[] iv)
        {
            this.m_Symmetrica.GenerateKey();
            this.m_Symmetrica.GenerateIV();
            key = new byte[this.m_Symmetrica.Key.Length];
            iv = new byte[this.m_Symmetrica.IV.Length];
            Buffer.BlockCopy(this.m_Symmetrica.Key, 0, key, 0, this.m_Symmetrica.Key.Length);
            Buffer.BlockCopy(this.m_Symmetrica.IV, 0, iv, 0, this.m_Symmetrica.IV.Length);
        }

        public ICryptoTransform DeCryptoTransform
        {
            get
            {
                return this.m_deCryptoTransform;
            }
        }

        public ICryptoTransform EnCryptoTransform
        {
            get
            {
                return this.m_enCryptoTransform;
            }
        }

        public byte[] IV
        {
            get
            {
                return this.m_Symmetrica.IV;
            }
            set
            {
                this.m_Symmetrica.IV = value;
            }
        }

        public byte[] Key
        {
            get
            {
                return this.m_Symmetrica.Key;
            }
            set
            {
                this.m_Symmetrica.Key = value;
            }
        }

        public int KeySize
        {
            get
            {
                return this.m_Symmetrica.KeySize;
            }
            set
            {
                this.m_Symmetrica.KeySize = value;
            }
        }

        public Platform.Security.SymmetricType SymmetricType
        {
            get
            {
                return this.m_SymmetricType;
            }
            set
            {
                if (this.m_SymmetricType != value)
                {
                    this.m_SymmetricType = value;
                    this.m_Symmetrica = SymmetricAlgorithm.Create(this.m_SymmetricType.ToString());
                }
            }
        }
    }
}
