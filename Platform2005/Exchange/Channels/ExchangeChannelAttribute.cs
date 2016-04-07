namespace Platform.Exchange.Channels
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class ExchangeChannelAttribute : Attribute
    {
        private string m_ChannelName;

        public ExchangeChannelAttribute(string channelName)
        {
            this.m_ChannelName = channelName;
        }

        public string ChannelName
        {
            get
            {
                return this.m_ChannelName;
            }
            set
            {
                this.m_ChannelName = value;
            }
        }
    }
}
