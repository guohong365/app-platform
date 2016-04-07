namespace Platform.CSS.Packet
{
    using Platform.CSS.Communication.Packet;
    using Platform.IO;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;

    public abstract class PacketBase : CommunicationPacketBase
    {
        protected ulong m_Bitmap;
        protected DateTime m_CommunicationTime;
        protected int m_ErrorField;
        private static readonly PacketsFieldCompare m_FieldCompare = new PacketsFieldCompare();
        protected byte[][] m_Fields;
        protected uint m_HandlerID;
        private static Hashtable m_PacketsFieldAttributes = new Hashtable();
        protected uint m_ReturnCode;
        protected uint m_Version;
        public static readonly Type PacketBaseType = typeof(PacketBase);
        public static readonly Type PacketFieldAttributeType = typeof(PacketFieldAttribute);

        public PacketBase()
        {
            PacketFieldAttribute[] attributeArray = m_PacketsFieldAttributes[base.GetType()] as PacketFieldAttribute[];
            if (attributeArray == null)
            {
                throw new Exception("请注册通讯接口类型！");
            }
            this.m_Fields = new byte[attributeArray.Length][];
            for (int i = 0; i < attributeArray.Length; i++)
            {
                if ((attributeArray[i] != null) && attributeArray[i].Fixed)
                {
                    this.m_Fields[i] = new byte[attributeArray[i].Length];
                }
            }
        }

        public bool CheckBitmap(ulong bitmap)
        {
            return ((this.m_Bitmap & bitmap) == bitmap);
        }

        public override bool Deserial(Platform.IO.MemoryStream stream)
        {
            this.m_ErrorField = 0;
            int num = (int) (stream.Length - stream.Position);
            if (num < 20)
            {
                return false;
            }
            int num2 = 0;
            if (!stream.Read(ref num2))
            {
                return false;
            }
            if ((num2 < 20) || (num2 > num))
            {
                return false;
            }
            if (!stream.Read(ref this.m_Version))
            {
                return false;
            }
            if (!stream.Read(ref this.m_Bitmap))
            {
                return false;
            }
            long num3 = 0;
            if (!stream.Read(ref num3))
            {
                return false;
            }
            this.m_CommunicationTime = new DateTime(num3);
            if (!stream.Read(ref this.m_HandlerID))
            {
                return false;
            }
            if (!stream.Read(ref this.m_ReturnCode))
            {
                return false;
            }
            PacketFieldAttribute[] attributeArray = m_PacketsFieldAttributes[base.GetType()] as PacketFieldAttribute[];
            for (int i = 0; i < attributeArray.Length; i++)
            {
                if ((this.m_Bitmap & attributeArray[i].BitMask) != 0)
                {
                    if (attributeArray[i].Fixed)
                    {
                        if (stream.Read(this.Fields[i], 0, attributeArray[i].Length) != attributeArray[i].Length)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        int num5 = 0;
                        if (!stream.Read(ref num5))
                        {
                            return false;
                        }
                        if (num5 < 0)
                        {
                            return false;
                        }
                        this.Fields[i] = new byte[num5];
                        if (stream.Read(this.Fields[i], 0, num5) != num5)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void FreeBuffer()
        {
            PacketFieldAttribute[] attributeArray = m_PacketsFieldAttributes[base.GetType()] as PacketFieldAttribute[];
            for (int i = 0; i < attributeArray.Length; i++)
            {
                if ((attributeArray[i] != null) && !attributeArray[i].Fixed)
                {
                    this.m_Fields[i] = null;
                }
            }
            base.FreeBuffer();
        }

        public static PacketFieldAttribute[] RegisterPacketType(Type type)
        {
            if (!type.IsSubclassOf(PacketBaseType))
            {
                return null;
            }
            PacketFieldAttribute[] attributeArray = m_PacketsFieldAttributes[type] as PacketFieldAttribute[];
            if (attributeArray != null)
            {
                return attributeArray;
            }
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ArrayList list = new ArrayList();
            foreach (PropertyInfo info in properties)
            {
                PacketFieldAttribute[] customAttributes = info.GetCustomAttributes(PacketFieldAttributeType, false) as PacketFieldAttribute[];
                if (customAttributes.Length >= 1)
                {
                    list.Add(customAttributes[0]);
                }
            }
            list.Sort(m_FieldCompare);
            PacketFieldAttribute[] attributeArray3 = (PacketFieldAttribute[]) list.ToArray(PacketFieldAttributeType);
            m_PacketsFieldAttributes[type] = attributeArray3;
            return attributeArray3;
        }

        public override bool Serial(Platform.IO.MemoryStream stream)
        {
            this.m_ErrorField = 0;
            this.m_CommunicationTime = PlatformDateTime.Now;
            long position = stream.Position;
            int num2 = 0;
            stream.Write(BitConverter.GetBytes(num2), 0, 4);
            stream.Write(BitConverter.GetBytes(this.m_Version), 0, 4);
            stream.Write(BitConverter.GetBytes(this.m_Bitmap), 0, 8);
            stream.Write(BitConverter.GetBytes(this.m_CommunicationTime.Ticks), 0, 8);
            stream.Write(BitConverter.GetBytes(this.m_HandlerID), 0, 4);
            stream.Write(BitConverter.GetBytes(this.m_ReturnCode), 0, 4);
            PacketFieldAttribute[] attributeArray = m_PacketsFieldAttributes[base.GetType()] as PacketFieldAttribute[];
            for (int i = 0; i < attributeArray.Length; i++)
            {
                if ((this.m_Bitmap & attributeArray[i].BitMask) != 0)
                {
                    this.SerialField(stream, i, attributeArray[i]);
                }
            }
            num2 = (int) (stream.Position - position);
            stream.Seek(position, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(num2), 0, 4);
            stream.Seek((long) 0, SeekOrigin.End);
            return true;
        }

        private void SerialField(Platform.IO.MemoryStream stream, int fieldIndex, PacketFieldAttribute attr)
        {
            if (attr.Fixed)
            {
                stream.Write(this.Fields[fieldIndex], 0, attr.Length);
            }
            else
            {
                int length = this.Fields[fieldIndex].Length;
                stream.Write(BitConverter.GetBytes(length), 0, 4);
                stream.Write(this.Fields[fieldIndex], 0, length);
            }
        }

        public ulong Bitmap
        {
            get
            {
                return this.m_Bitmap;
            }
            set
            {
                this.m_Bitmap = value;
            }
        }

        public DateTime CommunicationTime
        {
            get
            {
                return this.m_CommunicationTime;
            }
            set
            {
                this.m_CommunicationTime = value;
            }
        }

        public int ErrorField
        {
            get
            {
                return this.m_ErrorField;
            }
        }

        public byte[][] Fields
        {
            get
            {
                return this.m_Fields;
            }
        }

        public uint HandlerID
        {
            get
            {
                return this.m_HandlerID;
            }
            set
            {
                this.m_HandlerID = value;
            }
        }

        public bool HasError
        {
            get
            {
                return (this.m_ErrorField != 0);
            }
        }

        public uint ReturnCode
        {
            get
            {
                return this.m_ReturnCode;
            }
            set
            {
                this.m_ReturnCode = value;
            }
        }

        public uint Version
        {
            get
            {
                return this.m_Version;
            }
            set
            {
                this.m_Version = value;
            }
        }

        private class PacketsFieldCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                return (((PacketFieldAttribute) x).Index - ((PacketFieldAttribute) y).Index);
            }
        }
    }
}
