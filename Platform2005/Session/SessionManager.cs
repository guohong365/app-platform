namespace Platform.Session
{
    using System;
    using System.Collections;

    public sealed class SessionManager
    {
        private static Hashtable m_SessionChannels = new Hashtable();

        public static object GetSession(string channelName, int type, params object[] infos)
        {
            if ((channelName == null) || (channelName == ""))
            {
                return null;
            }
            ISessionChannel channel = m_SessionChannels[channelName] as ISessionChannel;
            if (channel == null)
            {
                return null;
            }
            return channel.GetSession(type, infos);
        }

        public static ArrayList LoadSessions(string channelName, int type)
        {
            if ((channelName == null) || (channelName == ""))
            {
                return null;
            }
            ISessionChannel channel = m_SessionChannels[channelName] as ISessionChannel;
            if (channel == null)
            {
                return null;
            }
            return channel.LoadSessions(type);
        }

        public static bool RegisterSessionChannel(string sessionChannelName, Type sessionChannelType)
        {
            if (((sessionChannelType == null) || (sessionChannelName == null)) || (sessionChannelName == ""))
            {
                return false;
            }
            if (sessionChannelType.GetInterface("Platform.Session.ISessionChannel", false) == null)
            {
                return false;
            }
            try
            {
                ISessionChannel channel = Activator.CreateInstance(sessionChannelType) as ISessionChannel;
                if (channel == null)
                {
                    return false;
                }
                m_SessionChannels[sessionChannelName] = channel;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RemoveSession(string channelName, int type, params object[] infos)
        {
            if ((channelName == null) || (channelName == ""))
            {
                return false;
            }
            ISessionChannel channel = m_SessionChannels[channelName] as ISessionChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.RemoveSession(type, infos);
        }

        public static bool SaveSession(string channelName, int type, params object[] infos)
        {
            if ((channelName == null) || (channelName == ""))
            {
                return false;
            }
            ISessionChannel channel = m_SessionChannels[channelName] as ISessionChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.SaveSession(type, infos);
        }

        public static bool UpdateSession(string channelName, int type, params object[] infos)
        {
            if ((channelName == null) || (channelName == ""))
            {
                return false;
            }
            ISessionChannel channel = m_SessionChannels[channelName] as ISessionChannel;
            if (channel == null)
            {
                return false;
            }
            return channel.UpdateSession(type, infos);
        }
    }
}
