namespace Platform.Serialization
{
    using Platform;
    using System;
    using System.IO;

    public sealed class SerialHelper
    {
        private MemoryStream m_Stream = new MemoryStream();
        private int m_Total = 0;

        public SerialHelper()
        {
            byte[] bytes = BitConverter.GetBytes(this.m_Total);
            this.m_Stream.Write(bytes, 0, 4);
        }

        public byte[] GetBytes()
        {
            this.m_Stream.Seek((long) 0, SeekOrigin.Begin);
            byte[] bytes = BitConverter.GetBytes(this.m_Total);
            this.m_Stream.Write(bytes, 0, 4);
            return this.m_Stream.ToArray();
        }

        public void Write(DateTime value)
        {
            this.Write(value.Ticks);
        }

        public void Write(byte[] value)
        {
            byte[] bytes = BitConverter.GetBytes(value.Length);
            this.m_Stream.Write(bytes, 0, 4);
            this.m_Stream.Write(value, 0, value.Length);
            this.m_Total += value.Length + 4;
        }

        public void Write(byte value)
        {
            this.m_Stream.WriteByte(value);
            this.m_Total++;
        }

        public void Write(Guid value)
        {
            byte[] buffer = value.ToByteArray();
            this.m_Stream.Write(buffer, 0, 0x10);
            this.m_Total += 0x10;
        }

        public void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 2);
            this.m_Total += 2;
        }

        public void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 4);
            this.m_Total += 4;
        }

        public void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 8);
            this.m_Total += 8;
        }

        public void Write(sbyte value)
        {
            this.m_Stream.WriteByte((byte) value);
            this.m_Total++;
        }

        public void Write(string value)
        {
            byte[] bytes = PlatformConfig.TextEncoding.GetBytes(value);
            byte[] buffer = BitConverter.GetBytes(bytes.Length);
            this.m_Stream.Write(buffer, 0, 4);
            this.m_Stream.Write(bytes, 0, bytes.Length);
            this.m_Total += bytes.Length + 4;
        }

        public void Write(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 2);
            this.m_Total += 2;
        }

        public void Write(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 4);
            this.m_Total += 4;
        }

        public void Write(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.m_Stream.Write(bytes, 0, 8);
            this.m_Total += 8;
        }
    }
}
