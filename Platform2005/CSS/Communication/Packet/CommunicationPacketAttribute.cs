namespace Platform.CSS.Communication.Packet
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommunicationPacketAttribute : Attribute
    {
        private string m_BroascastChannelName;
        private string m_ChannelName;
        private ushort m_Clsid;

        public CommunicationPacketAttribute(ushort clsid) : this(clsid, "Auto", "Auto")
        {
        }

        public CommunicationPacketAttribute(ushort clsid, string channelName) : this(clsid, channelName, "Auto")
        {
        }

        public CommunicationPacketAttribute(ushort clsid, string channelName, string broascastChannelName)
        {
            this.m_Clsid = clsid;
            this.m_ChannelName = channelName;
            this.m_BroascastChannelName = broascastChannelName;
        }

        public string BroascastChannelName
        {
            get
            {
                return this.m_BroascastChannelName;
            }
        }

        public string ChannelName
        {
            get
            {
                return this.m_ChannelName;
            }
        }

        public ushort CLSID
        {
            get
            {
                return this.m_Clsid;
            }
        }
    }
}
