namespace Platform.CSS.Communication.Packet
{
    using Platform.Caching;
    using Platform.CSS.Communication;
    using Platform.CSS.SerialSink;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;

    public abstract class CommunicationPacketBase : IBufferItem
    {
        public const int HEADER = 0x1a;
        private static ArrayList m_ClientCommonTable = new ArrayList();
        private static Hashtable m_CLSIDTables = new Hashtable();
        private System.Net.IPEndPoint m_IPEndPoint;
        private static readonly Type m_PacketBaseType = typeof(CommunicationPacketBase);
        private static Hashtable m_PacketSerialSinkTables = new Hashtable();
        private static MultiBufferStorage m_PacketStorage = new MultiBufferStorage();
        private static ArrayList m_ServerCommonTable = new ArrayList();
        private static Hashtable m_TYPETables = new Hashtable();

        protected CommunicationPacketBase()
        {
        }

        public abstract bool Deserial(Platform.IO.MemoryStream stream);
        public static CommunicationPacketBase Deserialize(string name, out Guid packetID, out int threadID, out int redoTimes, Platform.IO.MemoryStream ms, bool isServer)
        {
            return Deserialize(name, out packetID, out threadID, out redoTimes, ms, isServer, false);
        }

        public static CommunicationPacketBase Deserialize(string name, out Guid packetID, out int threadID, out int redoTimes, Platform.IO.MemoryStream ms, bool isServer, bool UseBuffer)
        {
            CommunicationPacketBase base3;
            packetID = Guid.Empty;
            threadID = 0;
            redoTimes = 0;
            if (ms.Length < 0x1a)
            {
                throw new Exception("读取通讯类型错误：" + name);
            }
            ms.Seek((long) 0, SeekOrigin.Begin);
            ushort num = 0;
            ms.Read(ref packetID);
            ms.Read(ref threadID);
            ms.Read(ref redoTimes);
            ms.Read(ref num);
            if (isServer)
            {
                if (m_ServerCommonTable.Contains(num))
                {
                    name = "__SERVER__";
                }
            }
            else if (m_ClientCommonTable.Contains(num))
            {
                name = "__CLIENT__";
            }
            Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
            if (hashtable == null)
            {
               throw new Exception(string.Concat(new object[] { "未注册通讯类型：", name, "::", num }));
            }
            Type type = hashtable[num] as Type;
            if (type == null)
            {
               throw new Exception(string.Concat(new object[] { "未注册通讯类型：", name, "::", num }));
            }
            CommunicationPacketBase buffer = null;
            PacketSerialSinkInfo info = null;
            Hashtable hashtable2 = m_PacketSerialSinkTables[name] as Hashtable;
            if (hashtable2 != null)
            {
                info = hashtable2[type] as PacketSerialSinkInfo;
            }
            try
            {
                int headerLen = (info == null) ? 0x1a : info.HeaderLen;
                if (info != null)
                {
                    foreach (SerialSinkInfo info2 in info.DeserialSinkTable)
                    {
                        ms = info2.Sink.Deserial(ms, headerLen, info2.HeaderOffset);
                        if (ms == null)
                        {
                            throw new Exception(string.Concat(new object[] { "反序列化，Sink 错误：", name, "::", num, " ", type, " ", info2.Sink.ToString() }));
                        }
                    }
                }
                ms.Seek((long) headerLen, SeekOrigin.Begin);
                if (UseBuffer)
                {
                    buffer = m_PacketStorage.GetBuffer(type) as CommunicationPacketBase;
                }
                else
                {
                    buffer = Activator.CreateInstance(type) as CommunicationPacketBase;
                }
                if (buffer == null)
                {
                    throw new Exception(string.Concat(new object[] { "反序列化，创建数据包类型错误：", name, "::", num, " ", type.ToString() }));
                }
                if (!buffer.Deserial(ms))
                {
                    if (UseBuffer)
                    {
                        buffer.FreeBuffer();
                    }
                    throw new Exception(string.Concat(new object[] { "反序列化错误：", name, "::", num, " ", type.ToString(), " 数据长度：", ms.Length }));
                }
                base3 = buffer;
            }
            catch (Exception exception)
            {
                if ((buffer != null) && UseBuffer)
                {
                    buffer.FreeBuffer();
                }
                throw exception;
            }
            return base3;
        }

        public void FinalRelease()
        {
        }

        public virtual void FreeBuffer()
        {
            m_PacketStorage.FreeBuffer(this);
        }

        public static CommunicationPacketBase GetBuffer()
        {
            return (m_PacketStorage.GetBuffer() as CommunicationPacketBase);
        }

        public static CommunicationPacketBase GetBuffer(Type type)
        {
            if (type.IsSubclassOf(m_PacketBaseType))
            {
                return (m_PacketStorage.GetBuffer(type) as CommunicationPacketBase);
            }
            return null;
        }

        public static CommunicationPacketAttribute GetCommunicationPacketAttribute(string name, int clsid)
        {
            Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return (hashtable[clsid] as CommunicationPacketAttribute);
        }

        public static CommunicationPacketAttribute GetCommunicationPacketAttribute(string name, Type type)
        {
            Hashtable hashtable = m_TYPETables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return (hashtable[type] as CommunicationPacketAttribute);
        }

        public static string GetCommunicationPacketBroadcastChannelName(string name, int clsid)
        {
            Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return ((CommunicationPacketAttribute) hashtable[clsid]).BroascastChannelName;
        }

        public static string GetCommunicationPacketBroadcastChannelName(string name, Type type)
        {
            Hashtable hashtable = m_TYPETables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return ((CommunicationPacketAttribute) hashtable[type]).BroascastChannelName;
        }

        public static string GetCommunicationPacketChannelName(string name, int clsid)
        {
            Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return ((CommunicationPacketAttribute) hashtable[clsid]).ChannelName;
        }

        public static string GetCommunicationPacketChannelName(string name, Type type)
        {
            Hashtable hashtable = m_TYPETables[name] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            return ((CommunicationPacketAttribute) hashtable[type]).ChannelName;
        }

        public static void RegisterCommunicationPackets(CommunicationPacketSettings settings)
        {
            foreach (CommunicationPacketSetting setting in settings.CommunicationPackets)
            {
                RegisterCommunicationPacketType(settings.Name, setting.PackageType, setting.PackageSinkTypes);
            }
        }

        public static void RegisterCommunicationPackets(Hashtable settings)
        {
            if (settings != null)
            {
                foreach (CommunicationPacketSettings settings2 in settings.Values)
                {
                    RegisterCommunicationPackets(settings2);
                }
            }
        }

        public static void RegisterCommunicationPacketType(string name, Type type)
        {
            if (type.IsSubclassOf(m_PacketBaseType))
            {
                CommunicationPacketAttribute[] customAttributes = type.GetCustomAttributes(typeof(CommunicationPacketAttribute), false) as CommunicationPacketAttribute[];
                if (customAttributes.Length >= 0)
                {
                    Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
                    if (hashtable == null)
                    {
                        hashtable = new Hashtable();
                        m_CLSIDTables[name] = hashtable;
                    }
                    Hashtable hashtable2 = m_TYPETables[name] as Hashtable;
                    if (hashtable2 == null)
                    {
                        hashtable2 = new Hashtable();
                        m_TYPETables[name] = hashtable2;
                    }
                    hashtable[customAttributes[0].CLSID] = type;
                    hashtable2[type] = customAttributes[0];
                    if (name == "__SERVER__")
                    {
                        if (!m_ServerCommonTable.Contains(customAttributes[0].CLSID))
                        {
                            m_ServerCommonTable.Add(customAttributes[0].CLSID);
                        }
                        if (!m_ServerCommonTable.Contains(type))
                        {
                            m_ServerCommonTable.Add(type);
                        }
                    }
                    else if (name == "__CLIENT__")
                    {
                        if (!m_ClientCommonTable.Contains(customAttributes[0].CLSID))
                        {
                            m_ClientCommonTable.Add(customAttributes[0].CLSID);
                        }
                        if (!m_ClientCommonTable.Contains(type))
                        {
                            m_ClientCommonTable.Add(type);
                        }
                    }
                }
            }
        }

        public static void RegisterCommunicationPacketType(string name, Type type, params Type[] sinkTypes)
        {
            if (type.IsSubclassOf(m_PacketBaseType))
            {
                CommunicationPacketAttribute[] customAttributes = type.GetCustomAttributes(typeof(CommunicationPacketAttribute), false) as CommunicationPacketAttribute[];
                if (customAttributes.Length >= 0)
                {
                    Hashtable hashtable = m_CLSIDTables[name] as Hashtable;
                    if (hashtable == null)
                    {
                        hashtable = new Hashtable();
                        m_CLSIDTables[name] = hashtable;
                    }
                    Hashtable hashtable2 = m_TYPETables[name] as Hashtable;
                    if (hashtable2 == null)
                    {
                        hashtable2 = new Hashtable();
                        m_TYPETables[name] = hashtable2;
                    }
                    hashtable[customAttributes[0].CLSID] = type;
                    hashtable2[type] = customAttributes[0];
                    if (name == "__SERVER__")
                    {
                        if (!m_ServerCommonTable.Contains(customAttributes[0].CLSID))
                        {
                            m_ServerCommonTable.Add(customAttributes[0].CLSID);
                        }
                        if (!m_ServerCommonTable.Contains(type))
                        {
                            m_ServerCommonTable.Add(type);
                        }
                    }
                    else if (name == "__CLIENT__")
                    {
                        if (!m_ClientCommonTable.Contains(customAttributes[0].CLSID))
                        {
                            m_ClientCommonTable.Add(customAttributes[0].CLSID);
                        }
                        if (!m_ClientCommonTable.Contains(type))
                        {
                            m_ClientCommonTable.Add(type);
                        }
                    }
                    if (sinkTypes != null)
                    {
                        RegisterPacketSerialSinkType(name, type, sinkTypes);
                    }
                }
            }
        }

        public static void RegisterPacketSerialSinkType(string name, Type packetType, Type sinkType)
        {
            SerialSinkAttribute[] customAttributes = sinkType.GetCustomAttributes(typeof(SerialSinkAttribute), false) as SerialSinkAttribute[];
            if (customAttributes.Length >= 1)
            {
                RegisterPacketSerialSinkType(name, packetType, sinkType, customAttributes[0].HeaderLength);
            }
        }

        public static void RegisterPacketSerialSinkType(string name, Type packetType, params Type[] sinkTypes)
        {
            if ((sinkTypes != null) && (sinkTypes.Length >= 1))
            {
                foreach (Type type in sinkTypes)
                {
                    SerialSinkAttribute[] customAttributes = type.GetCustomAttributes(typeof(SerialSinkAttribute), false) as SerialSinkAttribute[];
                    if (customAttributes.Length < 1)
                    {
                        break;
                    }
                    RegisterPacketSerialSinkType(name, packetType, type, customAttributes[0].HeaderLength);
                }
            }
        }

        public static void RegisterPacketSerialSinkType(string name, Type packetType, Type sinkType, int headerLength)
        {
            Hashtable hashtable = m_PacketSerialSinkTables[name] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_PacketSerialSinkTables[name] = hashtable;
            }
            PacketSerialSinkInfo info = hashtable[packetType] as PacketSerialSinkInfo;
            if (info == null)
            {
                info = new PacketSerialSinkInfo();
                hashtable[packetType] = info;
            }
            info.RegisterSerialSinkType(sinkType, headerLength);
        }

        public abstract bool Serial(Platform.IO.MemoryStream stream);
        public static bool Serialize(string name, Guid packetID, int threadID, int redoTimes, CommunicationPacketBase commPacket, out Platform.IO.MemoryStream outms, bool isServer)
        {
            if (isServer)
            {
                if (m_ServerCommonTable.Contains(commPacket.GetType()))
                {
                    name = "__SERVER__";
                }
            }
            else if (m_ClientCommonTable.Contains(commPacket.GetType()))
            {
                name = "__CLIENT__";
            }
            Hashtable hashtable = m_TYPETables[name] as Hashtable;
            if (hashtable == null)
            {
                throw new Exception("未注册通讯类型：" + name);
            }
            Type type = commPacket.GetType();
            CommunicationPacketAttribute attribute = hashtable[type] as CommunicationPacketAttribute;
            if (attribute == null)
            {
                throw new Exception(string.Concat(new object[] { "未注册通讯类型：", name, ";;", type }));
            }
            PacketSerialSinkInfo info = null;
            Hashtable hashtable2 = m_PacketSerialSinkTables[name] as Hashtable;
            if (hashtable2 != null)
            {
                info = hashtable2[type] as PacketSerialSinkInfo;
            }
            int headerLen = (info == null) ? 0x1a : info.HeaderLen;
            outms = new Platform.IO.MemoryStream();
            outms.Write(packetID);
            outms.Write(threadID);
            outms.Write(redoTimes);
            outms.Write(attribute.CLSID);
            outms.Seek((long) headerLen, SeekOrigin.Begin);
            if (!commPacket.Serial(outms))
            {
                throw new Exception(string.Concat(new object[] { "序列化错误：", name, ";;", attribute.CLSID, " ", type }));
            }
            if (info != null)
            {
                foreach (SerialSinkInfo info2 in info.SerialSinkTable)
                {
                    outms = info2.Sink.Serial(commPacket, outms, headerLen, info2.HeaderOffset);
                    if (outms == null)
                    {
                        throw new Exception(string.Concat(new object[] { "序列化，Sink 错误：", name, ";;", attribute.CLSID, " ", type, " ", info2.Sink.ToString() }));
                    }
                }
            }
            return true;
        }

        public System.Net.IPEndPoint IPEndPoint
        {
            get
            {
                return this.m_IPEndPoint;
            }
            set
            {
                this.m_IPEndPoint = value;
            }
        }
    }
}
