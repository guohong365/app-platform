namespace Platform.Serialization
{
    using Platform;
    using System;

    public sealed class DeserialHelper
    {
        private byte[] m_Data;
        private int m_Offset;
        private int m_ReadCount;
        private int m_Total;

        public bool LoadData(byte[] data)
        {
            return this.LoadData(data, 0, data.Length);
        }

        public bool LoadData(byte[] data, int offset, int count)
        {
            this.m_ReadCount = 0;
            this.m_Data = data;
            this.m_Offset = offset;
            this.m_Total = count;
            if (!this.TestCount(4))
            {
                return false;
            }
            int num = BitConverter.ToInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            if (this.m_Total > count)
            {
                return false;
            }
            this.m_Total = num + 4;
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
            if (!this.TestCount(0x10))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(this.m_Data, this.m_Offset, dst, 0, 0x10);
            value = new Guid(dst);
            this.m_Offset += 0x10;
            return true;
        }

        public bool Read(ref byte[] value)
        {
            if (!this.TestCount(4))
            {
                return false;
            }
            int len = BitConverter.ToInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            if (!this.TestCount(len))
            {
                return false;
            }
            if ((value == null) || (value.Length != len))
            {
                value = new byte[len];
            }
            Buffer.BlockCopy(this.m_Data, this.m_Offset, value, 0, len);
            this.m_Offset += len;
            return true;
        }

        public bool Read(ref byte value)
        {
            if (!this.TestCount(1))
            {
                return false;
            }
            value = this.m_Data[this.m_Offset];
            this.m_Offset++;
            return true;
        }

        public bool Read(ref short value)
        {
            if (!this.TestCount(2))
            {
                return false;
            }
            value = BitConverter.ToInt16(this.m_Data, this.m_Offset);
            this.m_Offset += 2;
            return true;
        }

        public bool Read(ref int value)
        {
            if (!this.TestCount(4))
            {
                return false;
            }
            value = BitConverter.ToInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            return true;
        }

        public bool Read(ref long value)
        {
            if (!this.TestCount(8))
            {
                return false;
            }
            value = BitConverter.ToInt64(this.m_Data, this.m_Offset);
            this.m_Offset += 8;
            return true;
        }

        public bool Read(ref sbyte value)
        {
            if (!this.TestCount(1))
            {
                return false;
            }
            value = (sbyte) this.m_Data[this.m_Offset];
            this.m_Offset++;
            return true;
        }

        public bool Read(ref string value)
        {
            if (!this.TestCount(4))
            {
                return false;
            }
            int len = BitConverter.ToInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            if (!this.TestCount(len))
            {
                return false;
            }
            value = PlatformConfig.TextEncoding.GetString(this.m_Data, this.m_Offset, len);
            this.m_Offset += len;
            return true;
        }

        public bool Read(ref ushort value)
        {
            if (!this.TestCount(2))
            {
                return false;
            }
            value = BitConverter.ToUInt16(this.m_Data, this.m_Offset);
            this.m_Offset += 2;
            return true;
        }

        public bool Read(ref uint value)
        {
            if (!this.TestCount(4))
            {
                return false;
            }
            value = BitConverter.ToUInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            return true;
        }

        public bool Read(ref ulong value)
        {
            if (!this.TestCount(8))
            {
                return false;
            }
            value = BitConverter.ToUInt64(this.m_Data, this.m_Offset);
            this.m_Offset += 8;
            return true;
        }

        public int ReadBytes(ref int offset)
        {
            if (!this.TestCount(4))
            {
                return -1;
            }
            int len = BitConverter.ToInt32(this.m_Data, this.m_Offset);
            this.m_Offset += 4;
            if (!this.TestCount(len))
            {
                return -1;
            }
            offset = this.m_Offset;
            this.m_Offset += len;
            return len;
        }

        private bool TestCount(int len)
        {
            this.m_ReadCount += len;
            if (this.m_ReadCount <= this.m_Total)
            {
                return true;
            }
            this.m_ReadCount -= len;
            return false;
        }

        public int Count
        {
            get
            {
                return this.m_ReadCount;
            }
        }
    }
}
