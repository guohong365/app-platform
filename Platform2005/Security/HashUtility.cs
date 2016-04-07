namespace Platform.Security
{
    using Platform;
    using Platform.Caching;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Cryptography;

    public sealed class HashUtility : IBufferItem
    {
        private static byte[] m_EmptyBuffer = new byte[0];
        private HashAlgorithm m_Hash;
        private string m_HashOID;
        private Platform.Security.HashType m_HashType;

        public HashUtility(Platform.Security.HashType hashType)
        {
            this.HashType = hashType;
        }

        public byte[] ComputeHash(string input)
        {
            this.m_Hash.Initialize();
            return this.m_Hash.ComputeHash(PlatformConfig.TextEncoding.GetBytes(input));
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            this.m_Hash.Initialize();
            return this.m_Hash.ComputeHash(buffer);
        }

        public byte[] ComputeHash(Platform.IO.MemoryStream ms)
        {
            return this.ComputeHash(ms, (int) (ms.Length - ms.Position));
        }

        public byte[] ComputeHash(Platform.IO.MemoryStream ms, int count)
        {
            ArrayList buffers = ms.Buffers;
            int bufferLength = ms.BufferLength;
            int num2 = (int) (ms.Position / ((long) bufferLength));
            int inputOffset = (int) (ms.Position % ((long) bufferLength));
            int num4 = (int) ((ms.Position + count) / ((long) bufferLength));
            int inputCount = (int) ((ms.Position + count) % ((long) bufferLength));
            this.m_Hash.Initialize();
            if (num2 == num4)
            {
                this.m_Hash.TransformFinalBlock((byte[]) buffers[num4], inputOffset, inputCount - inputOffset);
            }
            else
            {
                this.m_Hash.TransformBlock((byte[]) buffers[num2], inputOffset, bufferLength - inputOffset, (byte[]) buffers[num2], inputOffset);
                for (int i = num2 + 1; i < num4; i++)
                {
                    this.m_Hash.TransformBlock((byte[]) buffers[i], 0, bufferLength, (byte[]) buffers[i], 0);
                }
                this.m_Hash.TransformFinalBlock((byte[]) buffers[num4], 0, inputCount);
            }
            return this.m_Hash.Hash;
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            this.m_Hash.Initialize();
            return this.m_Hash.ComputeHash(buffer, offset, count);
        }

        public void FinalRelease()
        {
        }

        public byte[] GetFileHash(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    this.m_Hash.Initialize();
                    this.m_Hash.TransformBlock(m_EmptyBuffer, 0, 0, m_EmptyBuffer, 0);
                    byte[] buffer = new byte[0x400];
                    int inputCount = 0;
                    while ((inputCount = stream.Read(buffer, 0, buffer.Length)) == buffer.Length)
                    {
                        this.m_Hash.TransformBlock(buffer, 0, inputCount, buffer, 0);
                    }
                    this.m_Hash.TransformFinalBlock(buffer, 0, inputCount);
                }
                return this.m_Hash.Hash;
            }
            catch
            {
                return null;
            }
        }

        public string HashOID
        {
            get
            {
                return this.m_HashOID;
            }
        }

        public Platform.Security.HashType HashType
        {
            get
            {
                return this.m_HashType;
            }
            set
            {
                if (this.m_HashType != value)
                {
                    this.m_HashType = value;
                    this.m_HashOID = CryptoConfig.MapNameToOID(this.m_HashType.ToString());
                    this.m_Hash = HashAlgorithm.Create(this.m_HashType.ToString());
                }
            }
        }
    }
}
