namespace Platform.Compress
{
    using ICSharpCode.SharpZipLib.GZip;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.IO;

    public sealed class GZipUtility
    {
        public static byte[] GUnzip(byte[] input, int offset, int count)
        {
            try
            {
                byte[] buffer = new byte[0x400];
                System.IO.MemoryStream baseInputStream = new System.IO.MemoryStream(input, offset, count);
                GZipInputStream stream2 = new GZipInputStream(baseInputStream);
                System.IO.MemoryStream stream3 = new System.IO.MemoryStream();
                int num = 0;
                while ((num = stream2.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream3.Write(buffer, 0, num);
                }
                stream2.Close();
                return stream3.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] GUnzipData(byte[] buffer)
        {
            return GUnzipData(buffer, 0, buffer.Length);
        }

        public static bool GUnzipData(Platform.IO.MemoryStream ms, int offset, Platform.IO.MemoryStream outms)
        {
            try
            {
                int bufferLength = outms.BufferLength;
                ms.Seek((long) offset, SeekOrigin.Begin);
                GZipInputStream stream = new GZipInputStream(ms);
                int bufferIndex = outms.BufferIndex;
                int bufferOffset = outms.BufferOffset;
                ArrayList buffers = outms.Buffers;
                byte[] buffer = (byte[]) buffers[bufferIndex];
                int num4 = 0;
                int num5 = 0;
                do
                {
                    if (bufferOffset >= bufferLength)
                    {
                        bufferOffset -= bufferLength;
                        bufferIndex++;
                        if (bufferIndex >= buffers.Count)
                        {
                            buffer = new byte[bufferLength];
                            buffers.Add(buffer);
                        }
                        else
                        {
                            buffer = (byte[]) buffers[bufferIndex];
                        }
                    }
                    num5 = stream.Read(buffer, bufferOffset, bufferLength - bufferOffset);
                    num4 += num5;
                    bufferOffset += num5;
                }
                while (num5 > 0);
                outms.SetLength(outms.Position + num4);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] GUnzipData(byte[] buffer, int offset, int count)
        {
            byte[] buffer3;
            try
            {
                using (Platform.IO.MemoryStream stream = new Platform.IO.MemoryStream(buffer, offset, count))
                {
                    GZipInputStream stream2 = new GZipInputStream(stream);
                    using (Platform.IO.MemoryStream stream3 = new Platform.IO.MemoryStream())
                    {
                        int bufferLength = stream3.BufferLength;
                        int bufferIndex = stream3.BufferIndex;
                        int bufferOffset = stream3.BufferOffset;
                        ArrayList buffers = stream3.Buffers;
                        byte[] buffer2 = (byte[]) buffers[bufferIndex];
                        int num4 = 0;
                        int num5 = 0;
                        do
                        {
                            if (bufferOffset >= bufferLength)
                            {
                                bufferOffset -= bufferLength;
                                bufferIndex++;
                                if (bufferIndex >= buffers.Count)
                                {
                                    buffer2 = new byte[bufferLength];
                                    buffers.Add(buffer);
                                }
                                else
                                {
                                    buffer2 = (byte[]) buffers[bufferIndex];
                                }
                            }
                            num5 = stream2.Read(buffer2, bufferOffset, bufferLength - bufferOffset);
                            num4 += num5;
                            bufferOffset += num5;
                        }
                        while (num5 > 0);
                        stream3.SetLength(stream3.Position + num4);
                        buffer3 = stream3.ToArray();
                    }
                }
            }
            catch
            {
                buffer3 = null;
            }
            return buffer3;
        }

        public static byte[] GZip(string fileName)
        {
            byte[] buffer2;
            if (!File.Exists(fileName))
            {
                return null;
            }
            try
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    GZipOutputStream stream2 = new GZipOutputStream(stream);
                    using (FileStream stream3 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] buffer = new byte[0x400];
                        int count = 0;
                        while ((count = stream3.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream2.Write(buffer, 0, count);
                        }
                    }
                    stream2.Close();
                    buffer2 = stream.ToArray();
                }
            }
            catch
            {
                buffer2 = null;
            }
            return buffer2;
        }

        public static byte[] GZipData(byte[] buffer)
        {
            return GZipData(buffer, 0, buffer.Length);
        }

        public static byte[] GZipData(byte[] buffer, int offset, int count)
        {
            byte[] buffer2;
            try
            {
                using (Platform.IO.MemoryStream stream = new Platform.IO.MemoryStream())
                {
                    GZipOutputStream stream2 = new GZipOutputStream(stream);
                    stream2.Write(buffer, offset, count);
                    stream2.Finish();
                    buffer2 = stream.ToArray();
                }
            }
            catch
            {
                buffer2 = null;
            }
            return buffer2;
        }

        public static bool GZipData(Platform.IO.MemoryStream ms, int offset, Platform.IO.MemoryStream outms)
        {
            try
            {
                int bufferLength = ms.BufferLength;
                ms.Seek((long) offset, SeekOrigin.Begin);
                ArrayList buffers = ms.Buffers;
                int bufferIndex = ms.BufferIndex;
                int bufferOffset = ms.BufferOffset;
                int num4 = (int) (ms.Length / ((long) bufferLength));
                int count = (int) (ms.Length % ((long) bufferLength));
                GZipOutputStream stream = new GZipOutputStream(outms);
                if (bufferIndex == num4)
                {
                    if (count > bufferOffset)
                    {
                        stream.Write((byte[]) buffers[num4], bufferOffset, count - bufferOffset);
                    }
                    stream.Finish();
                    return true;
                }
                stream.Write((byte[]) buffers[bufferIndex], bufferOffset, bufferLength - bufferOffset);
                for (int i = bufferIndex + 1; i < num4; i++)
                {
                    stream.Write((byte[]) buffers[i], 0, bufferLength);
                }
                if (count > 0)
                {
                    stream.Write((byte[]) buffers[num4], 0, count);
                }
                stream.Finish();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
