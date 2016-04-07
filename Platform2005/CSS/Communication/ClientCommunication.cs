namespace Platform.CSS.Communication
{
    using Platform.CSS;
    using Platform.CSS.Client;
    using Platform.CSS.Communication.Channels;
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Packet;
    using Platform.CSS.SerialSink;
    using Platform.ExceptionHandling;
    using Platform.IO;
    using Platform.Tracing;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    public abstract class ClientCommunication
    {
        private static Hashtable m_Channels = new Hashtable();
        private static readonly LocalDataStoreSlot m_ErrorSlot = Thread.GetNamedDataSlot("PlatformCSSError");
        private static int m_ThreadIDSeed = 0;
        private static readonly LocalDataStoreSlot m_ThreadIDSlot = Thread.GetNamedDataSlot("PlatformCommunicationThreadID");
        private static MethodInfo PrepForRemotingHethod = typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        static ClientCommunication()
        {
            CommunicationPacketBase.RegisterCommunicationPacketType("__CLIENT__", typeof(ExceptionPacket), new System.Type[] { typeof(ZipSerialSink) });
        }

        protected ClientCommunication()
        {
        }

        public static void BroadcastPacket(CommunicationPacketBase packet)
        {
            string communicationPacketBroadcastChannelName = CommunicationPacketBase.GetCommunicationPacketBroadcastChannelName(CSSConfig.CommunicationBroadcastClientDefaultSetting, packet.GetType());
            BroadcastPacket(packet, communicationPacketBroadcastChannelName);
        }

        public static void BroadcastPacket(CommunicationPacketBase packet, bool UseBuffer)
        {
            string communicationPacketBroadcastChannelName = CommunicationPacketBase.GetCommunicationPacketBroadcastChannelName(CSSConfig.CommunicationBroadcastClientDefaultSetting, packet.GetType());
            BroadcastPacket(packet, communicationPacketBroadcastChannelName, UseBuffer);
        }

        public static void BroadcastPacket(CommunicationPacketBase packet, string channelName)
        {
            BroadcastPacket(packet, channelName, false);
        }

        public static void BroadcastPacket(string settingName, CommunicationPacketBase packet)
        {
            string communicationPacketBroadcastChannelName = CommunicationPacketBase.GetCommunicationPacketBroadcastChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            SendPacket(settingName, packet, communicationPacketBroadcastChannelName, false);
        }

        public static void BroadcastPacket(CommunicationPacketBase packet, string channelName, bool UseBuffer)
        {
            SendPacket(CSSConfig.CommunicationBroadcastClientDefaultSetting, packet, channelName, UseBuffer);
        }

        public static void BroadcastPacket(string settingName, CommunicationPacketBase packet, bool UseBuffer)
        {
            string communicationPacketBroadcastChannelName = CommunicationPacketBase.GetCommunicationPacketBroadcastChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            SendPacket(settingName, packet, communicationPacketBroadcastChannelName, UseBuffer);
        }

        public static bool CheckServerOnLine()
        {
            return CheckServerOnLine(CSSConfig.CommunicationClientDefaultSetting);
        }

        public static bool CheckServerOnLine(string settingName)
        {
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable == null)
            {
                return false;
            }
            CommunicationChannelSettings settings = CSSConfig.CommunicationClientChannels[settingName] as CommunicationChannelSettings;
            string defaultChannelName = settings.DefaultChannelName;
            IClientChannel channel = null;
            if (defaultChannelName != "Auto")
            {
                channel = hashtable[defaultChannelName] as IClientChannel;
            }
            if ((channel == null) && (hashtable.Count > 0))
            {
                try
                {
                    IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
                    enumerator.MoveNext();
                    channel = (IClientChannel) enumerator.Value;
                    defaultChannelName = (string) enumerator.Key;
                }
                catch
                {
                    channel = null;
                }
            }
            if (channel == null)
            {
                return false;
            }
            try
            {
                return channel.CheckServerOnLine(settingName);
            }
            catch
            {
                return false;
            }
        }

        public static void CleanErrorMessage()
        {
            Thread.SetData(m_ErrorSlot, 0);
        }

        public static void CloseConnections()
        {
            CloseConnections(CSSConfig.CommunicationClientDefaultSetting);
        }

        public static void CloseConnections(string settingName)
        {
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable != null)
            {
                CommunicationChannelSettings settings = CSSConfig.CommunicationClientChannels[settingName] as CommunicationChannelSettings;
                string defaultChannelName = settings.DefaultChannelName;
                IClientChannel channel = null;
                if (defaultChannelName != "Auto")
                {
                    channel = hashtable[defaultChannelName] as IClientChannel;
                }
                if ((channel == null) && (hashtable.Count > 0))
                {
                    try
                    {
                        IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
                        enumerator.MoveNext();
                        channel = (IClientChannel) enumerator.Value;
                        defaultChannelName = (string) enumerator.Key;
                    }
                    catch
                    {
                        channel = null;
                    }
                }
                if (channel != null)
                {
                    try
                    {
                        channel.CloseConnections(settingName);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static IClientChannel GetCommunicationClientChannel(string settingName, string channelName)
        {
            if (settingName == null)
            {
                return null;
            }
            CleanErrorMessage();
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            IClientChannel channel = null;
            if (channelName == null)
            {
                channelName = "Auto";
            }
            CommunicationChannelSettings settings = CSSConfig.CommunicationClientChannels[settingName] as CommunicationChannelSettings;
            if (settings.DefaultChannelName != "Default")
            {
                channelName = settings.DefaultChannelName;
            }
            if (channelName != "Auto")
            {
                channel = hashtable[channelName] as IClientChannel;
            }
            if ((channel == null) && (hashtable.Count > 0))
            {
                try
                {
                    IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
                    enumerator.MoveNext();
                    channel = (IClientChannel) enumerator.Value;
                    channelName = (string) enumerator.Key;
                }
                catch
                {
                    channel = null;
                }
            }
            if (channel == null)
            {
                SetErrorCode(-1);
                return null;
            }
            return channel;
        }

        [StackTracePass]
        public static int GetErrorCode()
        {
            object data = Thread.GetData(m_ErrorSlot);
            if (data == null)
            {
                return 0;
            }
            return (int) data;
        }

        [StackTracePass]
        public static string GetErrorMessage()
        {
            return CommunicationErrorCode.GetErrorMessage(GetErrorCode());
        }

        private static int GetThreadID()
        {
            object data = Thread.GetData(m_ThreadIDSlot);
            if ((data != null) && (data.GetType() == typeof(int)))
            {
                return (int) data;
            }
            int num = Interlocked.Increment(ref m_ThreadIDSeed);
            Thread.SetData(m_ThreadIDSlot, num);
            return num;
        }

        public static void RegisterCommunicationClientChannels(CommunicationChannelSettings settings)
        {
            Hashtable hashtable = m_Channels[settings.Name] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_Channels[settings.Name] = hashtable;
            }
            foreach (CommunicationChannelSetting setting in settings.CommunicationChannels)
            {
                if (setting.ChannelType.GetInterface("Platform.CSS.Communication.Channels.IClientChannel", false) != null)
                {
                    try
                    {
                        IClientChannel channel = Activator.CreateInstance(setting.ChannelType) as IClientChannel;
                        if ((channel != null) && channel.InitializeChannel(settings.Name, setting.IPEndPoints))
                        {
                            hashtable[setting.ChannelName] = channel;
                        }
                    }
                    catch (Exception exception)
                    {
                        ExceptionHelper.HandleException(exception);
                    }
                }
            }
        }

        public static void RegisterCommunicationClientChannels(Hashtable settings)
        {
            foreach (CommunicationChannelSettings settings2 in settings.Values)
            {
                RegisterCommunicationClientChannels(settings2);
            }
        }

        public static CommunicationPacketBase SendPacket(CommunicationPacketBase packet)
        {
            string communicationPacketChannelName = CommunicationPacketBase.GetCommunicationPacketChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            return SendPacket(packet, communicationPacketChannelName);
        }

        public static CommunicationPacketBase SendPacket(CommunicationPacketBase packet, bool UseBuffer)
        {
            string communicationPacketChannelName = CommunicationPacketBase.GetCommunicationPacketChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            return SendPacket(packet, communicationPacketChannelName, UseBuffer);
        }

        public static CommunicationPacketBase SendPacket(CommunicationPacketBase packet, string channelName)
        {
            return SendPacket(packet, channelName, false);
        }

        public static CommunicationPacketBase SendPacket(string settingName, CommunicationPacketBase packet)
        {
            string communicationPacketChannelName = CommunicationPacketBase.GetCommunicationPacketChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            return SendPacket(settingName, packet, communicationPacketChannelName, false);
        }

        public static CommunicationPacketBase SendPacket(CommunicationPacketBase packet, string channelName, bool UseBuffer)
        {
            return SendPacket(CSSConfig.CommunicationClientDefaultSetting, packet, channelName, UseBuffer);
        }

        public static CommunicationPacketBase SendPacket(string settingName, CommunicationPacketBase packet, bool UseBuffer)
        {
            string communicationPacketChannelName = CommunicationPacketBase.GetCommunicationPacketChannelName(CSSConfig.CommunicationClientDefaultSetting, packet.GetType());
            return SendPacket(settingName, packet, communicationPacketChannelName, UseBuffer);
        }

        public static CommunicationPacketBase SendPacket(string settingName, CommunicationPacketBase packet, string channelName, bool UseBuffer)
        {
            MemoryStream stream;
            Guid guid2;
            int num5;
            int num6;
            int num7;
            IClientChannel communicationClientChannel = GetCommunicationClientChannel(settingName, channelName);
            if (communicationClientChannel == null)
            {
                return null;
            }
            MemoryStream msrc = new MemoryStream();
            int threadID = GetThreadID();
            int redoTimes = 0;
            int breadPoint = 0;
            Guid packetID = Guid.NewGuid();
            if (communicationClientChannel.IsBroadcastChannel)
            {
                if (!CommunicationPacketBase.Serialize(settingName, packetID, threadID, redoTimes, packet, out stream, false))
                {
                    SetErrorCode(-2);
                    return null;
                }
                communicationClientChannel.Send(settingName, packetID, stream, msrc, ref breadPoint, CSSConfig.ClientCommunicationTimeout);
                return null;
            }
            int num4 = 0;
            CommunicationPacketBase base2 = null;
        Label_0063:
            num7 = (CSSConfig.ClientCommunicationResendTimes > 0) ? CSSConfig.ClientCommunicationResendTimes : 1;
            int num8 = num7;
            int resendTimes = 0;
        Label_007C:
            if (resendTimes >= num8)
            {
                if (!CSSConfig.ClientCommunicationResendTimesOutPrompt)
                {
                    goto Label_0186;
                }
                DialogResult result = new ClientCommunicationTimesOutPromptDlg(resendTimes, num7, breadPoint).ShowDialog();
                if (result == DialogResult.Abort)
                {
                    CSSConfig.ClientCommunicationResendTimesOutPrompt = false;
                    goto Label_0186;
                }
                if (result != DialogResult.Retry)
                {
                    goto Label_0186;
                }
                num8 += num7;
            }
            if ((resendTimes > 0) && (CSSConfig.ClientCommunicationResendDelayTicks > 0))
            {
                DelayUtility.DelayDoEvent(CSSConfig.ClientCommunicationResendDelayTicks);
            }
            redoTimes = resendTimes;
            resendTimes++;
            if (!CommunicationPacketBase.Serialize(settingName, packetID, threadID, redoTimes, packet, out stream, false))
            {
                SetErrorCode(-2);
                goto Label_007C;
            }
            long ticks = DateTime.Now.Ticks;
            try
            {
                if (!communicationClientChannel.Send(settingName, packetID, stream, msrc, ref breadPoint, CSSConfig.ClientCommunicationTimeout))
                {
                    SetErrorCode(-3);
                    goto Label_007C;
                }
            }
            finally
            {
                long num11 = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(num11 - ticks);
                TraceHelper.WriteLine("通讯时间（发送＋服务器处理＋接收）：" + span.TotalSeconds);
            }
            base2 = CommunicationPacketBase.Deserialize(settingName, out guid2, out num5, out num6, msrc, false, UseBuffer);
            if (base2 == null)
            {
                SetErrorCode(-4);
                goto Label_007C;
            }
        Label_0186:
            if (base2 == null)
            {
                SetErrorCode(-4);
                return null;
            }
            ExceptionPacket packet2 = base2 as ExceptionPacket;
            if (packet2 == null)
            {
                return base2;
            }
            Exception innerException = packet2.Exception;
            if ((CSSConfig.AutoReloginWhenMacError && (num4 < CSSConfig.AutoReloginTimes)) && ((packet.GetType() != typeof(LoginPacket)) && (innerException.GetType() == typeof(SecuritySerialSinkException))))
            {
                num4++;
                if (LoginHelper.Relogin(settingName))
                {
                    packetID = Guid.NewGuid();
                    goto Label_0063;
                }
            }
            CleanErrorMessage();
            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            innerException = PrepForRemotingHethod.Invoke(innerException, null) as Exception;
            throw innerException;
        }

        [StackTracePass]
        private static void SetErrorCode(int error)
        {
            Thread.SetData(m_ErrorSlot, error);
            TraceHelper.WriteLine(CommunicationErrorCode.GetErrorMessage(error));
        }
    }
}
