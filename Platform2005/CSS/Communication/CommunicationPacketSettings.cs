namespace Platform.CSS.Communication
{
    using System;

    public sealed class CommunicationPacketSettings
    {
        private CommunicationPacketSetting[] m_CommunicationPackets;
        private string m_Name;

        public CommunicationPacketSettings(string name, string configText)
        {
            this.m_Name = name.Trim();
            this.m_CommunicationPackets = CommunicationPacketSetting.GetCommunicationPacketSettings(configText);
        }

        public CommunicationPacketSetting[] CommunicationPackets
        {
            get
            {
                return this.m_CommunicationPackets;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
    }
}
