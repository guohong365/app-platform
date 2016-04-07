namespace Platform.IO
{
    using Platform.Configuration;
    using System;
    using System.Collections;
    using System.IO;

    [Serializable]
    public class MemoryStream : Stream
    {
        [ConfigItem("/PlatformSettings", "MEMORYBUFFERLENGTH", null)]
        private static int BUFFERLENGTH = 0x4000;
        private int m_BufferLength;
        private ArrayList m_Buffers;
        private int m_CurrentBufferIndex;
        private int m_CurrentBufferOffset;
        private long m_Length;
        private long m_Position;

        public MemoryStream()
        {
            this.m_Buffers = new ArrayList();
            this.m_BufferLength = BUFFERLENGTH;
            byte[] buffer = new byte[this.m_BufferLength];
            this.m_Buffers.Add(buffer);
            this.m_CurrentBufferIndex = 0;
            this.m_CurrentBufferOffset = 0;
            this.m_Position = 0;
            this.m_Length = 0;
        }

        public MemoryStream(byte[] buffer)
        {
            this.m_Buffers = new ArrayList();
            this.m_BufferLength = BUFFERLENGTH;
            byte[] buffer2 = new byte[this.m_BufferLength];
            this.m_Buffers.Add(buffer2);
            this.m_CurrentBufferIndex = 0;
            this.m_CurrentBufferOffset = 0;
            this.m_Position = 0;
            this.m_Length = 0;
            this.Write(buffer, 0, buffer.Length);
            this.Position = 0;
        }

        public MemoryStream(MemoryStreamData data)
        {
            this.m_Buffers = new ArrayList();
            this.m_BufferLength = BUFFERLENGTH;
            this.Build(data);
        }

        public MemoryStream(Stream m, int count)
        {
            byte[] buffer;
            this.m_Buffers = new ArrayList();
            this.m_BufferLength = BUFFERLENGTH;
            this.m_Length = count;
            this.m_Position = this.m_Length;
            this.m_CurrentBufferIndex = (int) (this.m_Length / ((long) this.m_BufferLength));
            this.m_CurrentBufferOffset = (int) (this.m_Length % ((long) this.m_BufferLength));
            for (int i = 0; i < this.m_CurrentBufferIndex; i++)
            {
                buffer = new byte[this.m_BufferLength];
                m.Read(buffer, 0, this.m_BufferLength);
                this.m_Buffers.Add(buffer);
            }
            buffer = new byte[this.m_BufferLength];
            this.m_Buffers.Add(buffer);
            if (this.m_CurrentBufferOffset > 0)
            {
                m.Read(buffer, 0, this.m_CurrentBufferOffset);
            }
        }

        public MemoryStream(byte[] buffer, int offset, int count)
        {
            this.m_Buffers = new ArrayList();
            this.m_BufferLength = BUFFERLENGTH;
            byte[] buffer2 = new byte[this.m_BufferLength];
            this.m_Buffers.Add(buffer2);
            this.m_CurrentBufferIndex = 0;
            this.m_CurrentBufferOffset = 0;
            this.m_Position = 0;
            this.m_Length = 0;
            this.Write(buffer, offset, count);
            this.Position = 0;
        }

        internal void Build(MemoryStreamData data)
        {
            this.m_Position = data.Position;
            this.m_Length = data.Length;
            this.m_Buffers = data.Buffers;
            this.m_BufferLength = data.BufferLength;
            this.m_CurrentBufferIndex = data.BufferIndex;
            this.m_CurrentBufferOffset = data.BufferOffset;
        }

        public override void Close()
        {
            this.m_Position = 0;
            this.m_Length = 0;
            this.m_Buffers.Clear();
        }

        public override void Flush()
        {
        }

        public bool Read(ref bool value)
        {
            byte[] buffer = new byte[1];
            if (this.Read(buffer, 0, 1) != 1)
            {
                return false;
            }
            value = BitConverter.ToBoolean(buffer, 0);
            return true;
        }

        public bool Read(ref byte value)
        {
            int num = this.ReadByte();
            if (num < 0)
            {
                return false;
            }
            value = (byte) num;
            return true;
        }

        public bool Read(ref DateTime value)
        {
            long num = 0;
            if (!this.Read(ref num))
            {
                return false;
            }
            value = new DateTime(num);
            return true;
        }

        public bool Read(ref Guid value)
        {
            byte[] buffer = new byte[0x10];
            if (this.Read(buffer, 0, 0x10) != 0x10)
            {
                return false;
            }
            value = new Guid(buffer);
            return true;
        }

        public bool Read(ref short value)
        {
            byte[] buffer = new byte[2];
            if (this.Read(buffer, 0, 2) != 2)
            {
                return false;
            }
            value = BitConverter.ToInt16(buffer, 0);
            return true;
        }

        public bool Read(ref int value)
        {
            byte[] buffer = new byte[4];
            if (this.Read(buffer, 0, 4) != 4)
            {
                return false;
            }
            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        public bool Read(ref long value)
        {
            byte[] buffer = new byte[8];
            if (this.Read(buffer, 0, 8) != 8)
            {
                return false;
            }
            value = BitConverter.ToInt64(buffer, 0);
            return true;
        }

        public bool Read(ref sbyte value)
        {
            int num = this.ReadByte();
            if (num < 0)
            {
                return false;
            }
            value = (sbyte) num;
            return true;
        }

        public bool Read(ref ushort value)
        {
            byte[] buffer = new byte[2];
            if (this.Read(buffer, 0, 2) != 2)
            {
                return false;
            }
            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        public bool Read(ref uint value)
        {
            byte[] buffer = new byte[4];
            if (this.Read(buffer, 0, 4) != 4)
            {
                return false;
            }
            value = BitConverter.ToUInt32(buffer, 0);
            return true;
        }

        public bool Read(ref ulong value)
        {
            byte[] buffer = new byte[8];
            if (this.Read(buffer, 0, 8) != 8)
            {
                return false;
            }
            value = BitConverter.ToUInt64(buffer, 0);
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if ((this.m_Position >= this.m_Length) || (count < 1))
            {
                return 0;
            }
            int num = (int) (this.m_Length - this.m_Position);
            int num2 = (num > count) ? count : num;
            num = 0;
            int num3 = num2;
            do
            {
                int num4;
                byte[] src = (byte[]) this.m_Buffers[this.m_CurrentBufferIndex];
                int num5 = this.m_BufferLength - this.m_CurrentBufferOffset;
                if (num3 <= num5)
                {
                    if (num3 < 9)
                    {
                        num4 = num3;
                        while (--num4 >= 0)
                        {
                            buffer[offset + num4] = src[this.m_CurrentBufferOffset + num4];
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(src, this.m_CurrentBufferOffset, buffer, offset, num3);
                    }
                    this.m_CurrentBufferOffset += num3;
                    if (this.m_CurrentBufferOffset == this.m_BufferLength)
                    {
                        this.m_CurrentBufferOffset = 0;
                        this.m_CurrentBufferIndex++;
                    }
                    num3 = 0;
                }
                else
                {
                    if (num5 < 9)
                    {
                        num4 = num5;
                        while (--num4 >= 0)
                        {
                            buffer[offset + num4] = src[this.m_CurrentBufferOffset + num4];
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(src, this.m_CurrentBufferOffset, buffer, offset, num5);
                    }
                    this.m_CurrentBufferOffset = 0;
                    this.m_CurrentBufferIndex++;
                    num3 -= num5;
                    offset += num5;
                }
            }
            while (num3 > 0);
            this.m_Position += num2;
            return num2;
        }

        public override int ReadByte()
        {
            if (this.m_Position >= this.m_Length)
            {
                return -1;
            }
            byte[] buffer = (byte[]) this.m_Buffers[this.m_CurrentBufferIndex];
            int num = buffer[this.m_CurrentBufferOffset];
            this.m_CurrentBufferOffset++;
            if (this.m_CurrentBufferOffset >= this.m_BufferLength)
            {
                this.m_CurrentBufferIndex++;
                this.m_CurrentBufferOffset = 0;
            }
            this.m_Position++;
            return num;
        }

        public byte[] ReadByteArray()
        {
            int num = 0;
            if (!this.Read(ref num))
            {
                return null;
            }
            if (num < 0)
            {
                return null;
            }
            if ((this.m_Length - this.m_Position) < num)
            {
                return null;
            }
            byte[] buffer = new byte[num];
            this.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public bool Save(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    int num = (int) (this.m_Length / ((long) this.m_BufferLength));
                    int count = (int) (this.m_Length % ((long) this.m_BufferLength));
                    for (int i = 0; i < num; i++)
                    {
                        stream.Write((byte[]) this.m_Buffers[i], 0, this.m_BufferLength);
                    }
                    if (count > 0)
                    {
                        stream.Write((byte[]) this.m_Buffers[num], 0, count);
                    }
                    stream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                this.Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                long num = this.m_Position + offset;
                this.Position = num;
            }
            else if (origin == SeekOrigin.End)
            {
                long num2 = this.m_Length - offset;
                this.Position = num2;
            }
            return this.m_Position;
        }

        public override void SetLength(long value)
        {
            if (value >= 0)
            {
                int num = (int) (value / ((long) this.m_BufferLength));
                this.m_Length = value;
                for (int i = this.m_Buffers.Count; i <= num; i++)
                {
                    this.m_Buffers.Add(new byte[this.m_BufferLength]);
                }
            }
        }

        public byte[] ToArray()
        {
            byte[] dst = new byte[this.m_Length];
            int num = (int) (this.m_Length / ((long) this.m_BufferLength));
            int count = (int) (this.m_Length % ((long) this.m_BufferLength));
            for (int i = 0; i < num; i++)
            {
                Buffer.BlockCopy((byte[]) this.m_Buffers[i], 0, dst, i * this.m_BufferLength, this.m_BufferLength);
            }
            if (count > 0)
            {
                Buffer.BlockCopy((byte[]) this.m_Buffers[num], 0, dst, num * this.m_BufferLength, count);
            }
            return dst;
        }

        public byte[] ToArray(int offset)
        {
            if ((offset >= 0) && (offset < this.m_Length))
            {
                return this.ToArray(offset, ((int) this.m_Length) - offset);
            }
            return new byte[0];
        }

        public byte[] ToArray(int offset, int count)
        {
            if (((count <= 0) || (offset < 0)) || ((offset + count) >= this.m_Length))
            {
                return new byte[0];
            }
            byte[] dst = new byte[count];
            int num = offset / this.m_BufferLength;
            int num2 = (offset + count) / this.m_BufferLength;
            int srcOffset = offset % this.m_BufferLength;
            int num4 = (offset + count) % this.m_BufferLength;
            if (num == num2)
            {
                if (num4 > srcOffset)
                {
                    Buffer.BlockCopy((byte[]) this.m_Buffers[num2], srcOffset, dst, 0, num4 - srcOffset);
                }
                return dst;
            }
            Buffer.BlockCopy((byte[]) this.m_Buffers[num], srcOffset, dst, 0, this.m_BufferLength - srcOffset);
            int dstOffset = this.m_BufferLength - srcOffset;
            for (int i = num + 1; i < num2; i++)
            {
                Buffer.BlockCopy((byte[]) this.m_Buffers[i], 0, dst, dstOffset, this.m_BufferLength);
                dstOffset += this.m_BufferLength;
            }
            if (num4 > 0)
            {
                Buffer.BlockCopy((byte[]) this.m_Buffers[num2], 0, dst, dstOffset, num4);
            }
            return dst;
        }

        public void Write(byte[] buffer)
        {
            this.Write(buffer, 0, buffer.Length);
        }

        public void Write(Platform.IO.MemoryStream ms)
        {
            int bufferLength = ms.BufferLength;
            ArrayList buffers = ms.Buffers;
            int bufferIndex = ms.BufferIndex;
            int bufferOffset = ms.BufferOffset;
            int num4 = (int) (ms.Length / ((long) bufferLength));
            int count = (int) (ms.Length % ((long) bufferLength));
            if (bufferIndex == num4)
            {
                if (count > bufferOffset)
                {
                    this.Write((byte[]) buffers[num4], bufferOffset, count - bufferOffset);
                }
            }
            else
            {
                this.Write((byte[]) buffers[bufferIndex], bufferOffset, bufferLength - bufferOffset);
                for (int i = bufferIndex + 1; i < num4; i++)
                {
                    this.Write((byte[]) buffers[i], 0, bufferLength);
                }
                if (count > 0)
                {
                    this.Write((byte[]) buffers[num4], 0, count);
                }
            }
        }

        public void Write(bool value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 1);
        }

        public void Write(byte value)
        {
            this.WriteByte(value);
        }

        public void Write(DateTime value)
        {
            long ticks = value.Ticks;
            this.Write(ticks);
        }

        public void Write(Guid value)
        {
            byte[] buffer = value.ToByteArray();
            this.Write(buffer, 0, 0x10);
        }

        public void Write(short value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void Write(int value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Write(long value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void Write(sbyte value)
        {
            this.WriteByte((byte) value);
        }

        public void Write(ushort value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void Write(uint value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Write(ulong value)
        {
            this.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int num = count;
            byte[] dst = (byte[]) this.m_Buffers[this.m_CurrentBufferIndex];
            do
            {
                int num2;
                int num3 = this.m_BufferLength - this.m_CurrentBufferOffset;
                if (num <= num3)
                {
                    if (num < 9)
                    {
                        num2 = num;
                        while (--num2 >= 0)
                        {
                            dst[this.m_CurrentBufferOffset + num2] = buffer[offset + num2];
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset, dst, this.m_CurrentBufferOffset, num);
                    }
                    this.m_CurrentBufferOffset += num;
                    if (this.m_CurrentBufferOffset == this.m_BufferLength)
                    {
                        this.m_CurrentBufferOffset = 0;
                        this.m_CurrentBufferIndex++;
                        if (this.m_CurrentBufferIndex >= this.m_Buffers.Count)
                        {
                            this.m_Buffers.Add(new byte[this.m_BufferLength]);
                        }
                    }
                    num = 0;
                }
                else
                {
                    if (num3 < 9)
                    {
                        num2 = num3;
                        while (--num2 >= 0)
                        {
                            dst[this.m_CurrentBufferOffset + num2] = buffer[offset + num2];
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset, dst, this.m_CurrentBufferOffset, num3);
                    }
                    this.m_CurrentBufferOffset = 0;
                    this.m_CurrentBufferIndex++;
                    if (this.m_CurrentBufferIndex >= this.m_Buffers.Count)
                    {
                        dst = new byte[this.m_BufferLength];
                        this.m_Buffers.Add(dst);
                    }
                    else
                    {
                        dst = (byte[]) this.m_Buffers[this.m_CurrentBufferIndex];
                    }
                    offset += num3;
                    num -= num3;
                }
            }
            while (num > 0);
            this.m_Position += count;
            if (this.m_Length < this.m_Position)
            {
                this.m_Length = this.m_Position;
            }
        }

        public override void WriteByte(byte value)
        {
            byte[] buffer = (byte[]) this.m_Buffers[this.m_CurrentBufferIndex];
            buffer[this.m_CurrentBufferOffset] = value;
            this.m_CurrentBufferOffset++;
            if (this.m_CurrentBufferOffset >= this.m_BufferLength)
            {
                this.m_CurrentBufferIndex++;
                this.m_CurrentBufferOffset = 0;
                if (this.m_CurrentBufferIndex >= this.m_Buffers.Count)
                {
                    this.m_Buffers.Add(new byte[this.m_BufferLength]);
                }
            }
            if (this.m_Position >= this.m_Length)
            {
                this.m_Length++;
            }
            this.m_Position++;
        }

        public void WriteByteArray(byte[] value)
        {
            if (value == null)
            {
                this.Write(-1);
            }
            else
            {
                int length = value.Length;
                this.Write(length);
                this.Write(value, 0, value.Length);
            }
        }

        public int BufferIndex
        {
            get
            {
                return this.m_CurrentBufferIndex;
            }
        }

        public int BufferLength
        {
            get
            {
                return this.m_BufferLength;
            }
        }

        public int BufferOffset
        {
            get
            {
                return this.m_CurrentBufferOffset;
            }
        }

        public ArrayList Buffers
        {
            get
            {
                return this.m_Buffers;
            }
        }

        public override bool CanRead
        {
            get
            {
                return (this.m_Length >= 0);
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return this.m_Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                if (value < 0)
                {
                    this.m_Position = 0;
                }
                else
                {
                    this.m_Position = value;
                }
                this.m_CurrentBufferIndex = (int) (this.m_Position / ((long) this.m_BufferLength));
                this.m_CurrentBufferOffset = (int) (this.m_Position % ((long) this.m_BufferLength));
                for (int i = this.m_Buffers.Count; i <= this.m_CurrentBufferIndex; i++)
                {
                    this.m_Buffers.Add(new byte[this.m_BufferLength]);
                }
                if (this.m_Length < this.m_Position)
                {
                    this.m_Length = this.m_Position;
                }
            }
        }
    }
}
