namespace Platform.CSS.Communication.Channels
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ChannelAttribute : Attribute
    {
        private string m_ChannelName;

        public ChannelAttribute(string channelName)
        {
            this.m_ChannelName = channelName;
        }

        public string ChannelName
        {
            get
            {
                return this.m_ChannelName;
            }
        }
    }
}
