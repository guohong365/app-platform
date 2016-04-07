namespace Platform.Security
{
    using Platform;
    using Platform.Caching;
    using Platform.ExceptionHandling;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public sealed class Security
    {
        private static string m_HashOID;
        private static BufferStorage m_HashStorage = new BufferStorage(typeof(HashUtility));
        private static HashType m_HashType;
        private static RSAParameters m_RsaKey;
        private static BufferStorage m_RsaStorage = new BufferStorage(typeof(RsaUtility));
        private static BufferStorage m_SymmetricStorage = new BufferStorage(typeof(SymmetricUtility));
        private static SymmetricType m_SymmetricType;

        public static byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        public static byte[] ComputeHash(string input)
        {
            return ComputeHash(PlatformConfig.TextEncoding.GetBytes(input));
        }

        public static byte[] ComputeHash(Platform.IO.MemoryStream ms, int count)
        {
            byte[] buffer;
            HashUtility item = m_HashStorage.GetBuffer() as HashUtility;
            try
            {
                buffer = item.ComputeHash(ms, count);
            }
            finally
            {
                m_HashStorage.FreeBuffer(item);
            }
            return buffer;
        }

        public static byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            byte[] buffer2;
            HashUtility item = m_HashStorage.GetBuffer() as HashUtility;
            try
            {
                buffer2 = item.ComputeHash(buffer, offset, count);
            }
            finally
            {
                m_HashStorage.FreeBuffer(item);
            }
            return buffer2;
        }

        public static string ComputeHashToBase64String(byte[] buffer)
        {
            return Convert.ToBase64String(ComputeHash(buffer, 0, buffer.Length));
        }

        public static string ComputeHashToBase64String(string input)
        {
            return Convert.ToBase64String(ComputeHash(input));
        }

        public static string ComputeHashToBase64String(byte[] buffer, int offset, int count)
        {
            return Convert.ToBase64String(ComputeHash(buffer, offset, count));
        }

        public static bool CreateSymmetricCryptoTransform(byte[] key, byte[] iv, out ICryptoTransform enCryptoTransform, out ICryptoTransform deCryptoTransform)
        {
            SymmetricUtility buffer = m_SymmetricStorage.GetBuffer() as SymmetricUtility;
            try
            {
                if (!buffer.GenerateCryptoTransform(key, iv))
                {
                    enCryptoTransform = null;
                    deCryptoTransform = null;
                    return false;
                }
                enCryptoTransform = buffer.EnCryptoTransform;
                deCryptoTransform = buffer.DeCryptoTransform;
            }
            finally
            {
                m_SymmetricStorage.FreeBuffer(buffer);
            }
            return true;
        }

        public static ICryptoTransform CreateSymmetricDeTransform(byte[] key, byte[] iv)
        {
            ICryptoTransform transform;
            SymmetricUtility buffer = m_SymmetricStorage.GetBuffer() as SymmetricUtility;
            try
            {
                transform = buffer.GenerateDeCryptoTransform(key, iv);
            }
            finally
            {
                m_SymmetricStorage.FreeBuffer(buffer);
            }
            return transform;
        }

        public static ICryptoTransform CreateSymmetricEnTransform(byte[] key, byte[] iv)
        {
            ICryptoTransform transform;
            SymmetricUtility buffer = m_SymmetricStorage.GetBuffer() as SymmetricUtility;
            try
            {
                transform = buffer.GenerateEnCryptoTransform(key, iv);
            }
            finally
            {
                m_SymmetricStorage.FreeBuffer(buffer);
            }
            return transform;
        }

        public static void GenerateSymmetricKeyIV(out byte[] key, out byte[] iv)
        {
            SymmetricUtility buffer = m_SymmetricStorage.GetBuffer() as SymmetricUtility;
            try
            {
                buffer.GenerateKey(out key, out iv);
            }
            finally
            {
                m_SymmetricStorage.FreeBuffer(buffer);
            }
        }

        public static bool Initialize(HashType hash, SymmetricType symmetric, RSAParameters rsakey)
        {
            m_HashType = hash;
            m_RsaKey = rsakey;
            m_HashOID = CryptoConfig.MapNameToOID(m_HashType.ToString());
            m_SymmetricType = symmetric;
            m_SymmetricStorage.Args = new object[] { m_SymmetricType };
            m_HashStorage.Args = new object[] { m_HashType };
            m_RsaStorage.Args = new object[] { m_RsaKey };
            return true;
        }

        public static void PreStoreBuffer(int rsaCount, int hashCount, int symmetricCount)
        {
            m_HashStorage.PreStoreItem(hashCount);
            m_RsaStorage.PreStoreItem(rsaCount);
            m_SymmetricStorage.PreStoreItem(symmetricCount);
        }

        public static byte[] RsaPrivateDecrypt(byte[] input)
        {
            byte[] buffer;
            RsaUtility item = m_RsaStorage.GetBuffer() as RsaUtility;
            try
            {
                buffer = item.PrivateDecrypt(input);
            }
            finally
            {
                m_RsaStorage.FreeBuffer(item);
            }
            return buffer;
        }

        public static byte[] RsaPublicEncrypt(byte[] input)
        {
            byte[] buffer;
            RsaUtility item = m_RsaStorage.GetBuffer() as RsaUtility;
            try
            {
                buffer = item.PublicEncrypt(input);
            }
            finally
            {
                m_RsaStorage.FreeBuffer(item);
            }
            return buffer;
        }

        public static byte[] RsaSign(byte[] data)
        {
            byte[] buffer;
            HashUtility hash = m_HashStorage.GetBuffer() as HashUtility;
            RsaUtility item = m_RsaStorage.GetBuffer() as RsaUtility;
            try
            {
                buffer = item.Sign(data, hash);
            }
            finally
            {
                m_HashStorage.FreeBuffer(hash);
                m_RsaStorage.FreeBuffer(item);
            }
            return buffer;
        }

        public static bool RsaVerify(byte[] data, byte[] sign)
        {
            bool flag;
            HashUtility buffer = m_HashStorage.GetBuffer() as HashUtility;
            RsaUtility item = m_RsaStorage.GetBuffer() as RsaUtility;
            try
            {
                flag = item.Verify(data, sign, buffer);
            }
            finally
            {
                m_HashStorage.FreeBuffer(buffer);
                m_RsaStorage.FreeBuffer(item);
            }
            return flag;
        }

        public static byte[] SymmetricCrypt(byte[] input, ICryptoTransform transform)
        {
            return SymmetricCrypt(input, 0, input.Length, transform);
        }

        public static byte[] SymmetricCrypt(Platform.IO.MemoryStream ms, int offset, int count, ICryptoTransform transform)
        {
            if (count > (ms.Length - ms.Position))
            {
                return null;
            }
            Platform.IO.MemoryStream stream = new Platform.IO.MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            try
            {
                int bufferLength = ms.BufferLength;
                ms.Seek((long) offset, SeekOrigin.Begin);
                ArrayList buffers = ms.Buffers;
                int bufferIndex = ms.BufferIndex;
                int bufferOffset = ms.BufferOffset;
                int num4 = (offset + count) / bufferLength;
                int num5 = (offset + count) % bufferLength;
                if (bufferIndex == num4)
                {
                    if (num5 > bufferOffset)
                    {
                        stream2.Write((byte[]) buffers[num4], bufferOffset, num5 - bufferOffset);
                    }
                }
                else
                {
                    stream2.Write((byte[]) buffers[bufferIndex], bufferOffset, bufferLength - bufferOffset);
                    for (int i = bufferIndex + 1; i < num4; i++)
                    {
                        stream2.Write((byte[]) buffers[i], 0, bufferLength);
                    }
                    if (num5 > 0)
                    {
                        stream2.Write((byte[]) buffers[num4], 0, num5);
                    }
                }
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

        public static byte[] SymmetricCrypt(byte[] input, byte[] key, byte[] iv, bool encrypt)
        {
            return SymmetricCrypt(input, 0, input.Length, key, iv, encrypt);
        }

        public static byte[] SymmetricCrypt(byte[] input, int index, int count, ICryptoTransform transform)
        {
            Platform.IO.MemoryStream stream = new Platform.IO.MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            try
            {
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

        public static byte[] SymmetricCrypt(Platform.IO.MemoryStream ms, byte[] key, byte[] iv, bool encrypt)
        {
            return SymmetricCrypt(ms, (int) ms.Position, (int) (ms.Length - ms.Position), encrypt ? CreateSymmetricEnTransform(key, iv) : CreateSymmetricDeTransform(key, iv));
        }

        public static bool SymmetricCrypt(Platform.IO.MemoryStream ms, int offset, int count, ICryptoTransform transform, Platform.IO.MemoryStream outms)
        {
            if (count > (ms.Length - ms.Position))
            {
                return false;
            }
            CryptoStream stream = new CryptoStream(outms, transform, CryptoStreamMode.Write);
            try
            {
                int bufferLength = ms.BufferLength;
                ms.Seek((long) offset, SeekOrigin.Begin);
                ArrayList buffers = ms.Buffers;
                int bufferIndex = ms.BufferIndex;
                int bufferOffset = ms.BufferOffset;
                int num4 = (offset + count) / bufferLength;
                int num5 = (offset + count) % bufferLength;
                if (bufferIndex == num4)
                {
                    if (num5 > bufferOffset)
                    {
                        stream.Write((byte[]) buffers[num4], bufferOffset, num5 - bufferOffset);
                    }
                }
                else
                {
                    stream.Write((byte[]) buffers[bufferIndex], bufferOffset, bufferLength - bufferOffset);
                    for (int i = bufferIndex + 1; i < num4; i++)
                    {
                        stream.Write((byte[]) buffers[i], 0, bufferLength);
                    }
                    if (num5 > 0)
                    {
                        stream.Write((byte[]) buffers[num4], 0, num5);
                    }
                }
                stream.FlushFinalBlock();
                return true;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public static bool SymmetricCrypt(Platform.IO.MemoryStream ms, byte[] key, byte[] iv, bool encrypt, Platform.IO.MemoryStream outms)
        {
            return SymmetricCrypt(ms, (int) ms.Position, (int) (ms.Length - ms.Position), encrypt ? CreateSymmetricEnTransform(key, iv) : CreateSymmetricDeTransform(key, iv), outms);
        }

        public static byte[] SymmetricCrypt(Platform.IO.MemoryStream ms, int offset, int count, byte[] key, byte[] iv, bool encrypt)
        {
            return SymmetricCrypt(ms, offset, count, encrypt ? CreateSymmetricEnTransform(key, iv) : CreateSymmetricDeTransform(key, iv));
        }

        public static byte[] SymmetricCrypt(byte[] input, int index, int count, byte[] key, byte[] iv, bool encrypt)
        {
            return SymmetricCrypt(input, index, count, encrypt ? CreateSymmetricEnTransform(key, iv) : CreateSymmetricDeTransform(key, iv));
        }

        public static bool SymmetricCrypt(Platform.IO.MemoryStream ms, int offset, int count, byte[] key, byte[] iv, bool encrypt, Platform.IO.MemoryStream outms)
        {
            return SymmetricCrypt(ms, offset, count, encrypt ? CreateSymmetricEnTransform(key, iv) : CreateSymmetricDeTransform(key, iv), outms);
        }
    }
}
