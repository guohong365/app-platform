namespace Platform.CSS.Packet
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PacketFieldAttribute : Attribute
    {
        private ulong m_BitMask;
        private bool m_Fixed;
        private int m_Index;
        private int m_Length;
        private string m_Name;
        private int m_Type;

        public PacketFieldAttribute(int index, ulong bitmask, string name, int type, int length, bool fix)
        {
            this.m_BitMask = bitmask;
            this.m_Index = index;
            this.m_Name = name;
            this.m_Type = type;
            this.m_Length = length;
            this.m_Fixed = fix;
        }

        public ulong BitMask
        {
            get
            {
                return this.m_BitMask;
            }
        }

        public bool Fixed
        {
            get
            {
                return this.m_Fixed;
            }
        }

        public int Index
        {
            get
            {
                return this.m_Index;
            }
        }

        public int Length
        {
            get
            {
                return this.m_Length;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public int Type
        {
            get
            {
                return this.m_Type;
            }
        }
    }
}
