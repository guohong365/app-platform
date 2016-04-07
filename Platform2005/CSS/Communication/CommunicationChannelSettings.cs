namespace Platform.CSS.Communication
{
    using System;

    public sealed class CommunicationChannelSettings
    {
        private CommunicationChannelSetting[] m_CommunicationChannels;
        private string m_DefaultChannelName;
        private string m_Name;

        public CommunicationChannelSettings(string name, string configText)
        {
            this.m_Name = name.Trim();
            string[] textArray = configText.Split(new char[] { '`' });
            this.m_DefaultChannelName = textArray[0].Trim();
            this.m_CommunicationChannels = CommunicationChannelSetting.GetCommunicationChannelSettings(textArray[1]);
        }

        public CommunicationChannelSetting[] CommunicationChannels
        {
            get
            {
                return this.m_CommunicationChannels;
            }
        }

        public string DefaultChannelName
        {
            get
            {
                return this.m_DefaultChannelName;
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
