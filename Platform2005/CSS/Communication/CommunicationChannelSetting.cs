namespace Platform.CSS.Communication
{
    using Platform.CSS.Communication.Channels;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Net;

    public sealed class CommunicationChannelSetting
    {
        private string m_ChannelName;
        private Type m_ChannelType;
        private IPEndPoint[] m_IPEndPoints;

        private CommunicationChannelSetting(string channelName, Type channelType, IPEndPoint[] ipes)
        {
            this.m_ChannelName = channelName;
            this.m_ChannelType = channelType;
            this.m_IPEndPoints = ipes;
        }

        public static CommunicationChannelSetting GetCommunicationChannelSetting(string init)
        {
            try
            {
                string[] textArray = init.Split(new char[] { ':' });
                Type typeFromName = TypeUtility.GetTypeFromName(textArray[0]);
                ChannelAttribute[] customAttributes = typeFromName.GetCustomAttributes(typeof(ChannelAttribute), false) as ChannelAttribute[];
                if (customAttributes.Length < 1)
                {
                    return null;
                }
                string channelName = customAttributes[0].ChannelName;
                ArrayList list = new ArrayList();
                if (textArray[1].Trim().Length > 1)
                {
                    string[] textArray2 = textArray[1].Trim().Split(new char[] { '|' });
                    for (int i = 0; i < textArray2.Length; i++)
                    {
                        string[] textArray3 = textArray2[i].Split(new char[] { '-' });
                        if (textArray3.Length == 2)
                        {
                            try
                            {
                                IPEndPoint point = new IPEndPoint((textArray3[0] == "Any") ? IPAddress.Any : IPAddress.Parse(textArray3[0]), int.Parse(textArray3[1]));
                                list.Add(point);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                return new CommunicationChannelSetting(channelName, typeFromName, list.ToArray(typeof(IPEndPoint)) as IPEndPoint[]);
            }
            catch
            {
                return null;
            }
        }

        public static CommunicationChannelSetting[] GetCommunicationChannelSettings(string inits)
        {
            CommunicationChannelSetting[] settingArray;
            if (inits == null)
            {
                  throw new Exception("GetCommunicationChannelInfos ²ÎÊýÅäÖÃ´íÎó£¡");
            }
            try
            {
                string[] textArray = inits.Split(new char[] { ',' });
                ArrayList list = new ArrayList();
                foreach (string text in textArray)
                {
                    CommunicationChannelSetting communicationChannelSetting = GetCommunicationChannelSetting(text);
                    if (communicationChannelSetting != null)
                    {
                        list.Add(communicationChannelSetting);
                    }
                }
                settingArray = list.ToArray(typeof(CommunicationChannelSetting)) as CommunicationChannelSetting[];
            }
            catch
            {
                throw new Exception("GetCommunicationChannelInfos ²ÎÊýÅäÖÃ´íÎó£¡");
            }
            return settingArray;
        }

        public string ChannelName
        {
            get
            {
                return this.m_ChannelName;
            }
        }

        public Type ChannelType
        {
            get
            {
                return this.m_ChannelType;
            }
        }

        public IPEndPoint[] IPEndPoints
        {
            get
            {
                return this.m_IPEndPoints;
            }
        }
    }
}
