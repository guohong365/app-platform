namespace Platform.CSS
{
    using Platform.Configuration;
    using Platform.CSS.Communication;
    using Platform.CSS.SerialSink;
    using Platform.ExceptionHandling;
    using System;
    using System.Collections;
    using System.Net;
    using System.Threading;

    public sealed class CSSConfig
    {
        [ConfigItem("/PlatformSettings", "AutoReloginTimes", null)]
        public static int AutoReloginTimes = 1;
        [ConfigItem("/PlatformSettings", "AutoReloginWhenMacError", null)]
        public static bool AutoReloginWhenMacError = false;
        [ConfigItem("/PlatformSettings", "BroadcastServerMultiThreadHandling", null)]
        public static bool BroadcastServerMultiThreadHandling = false;
        [ConfigItem("/PlatformSettings", "ClientCommunicationRemotingHandleException", null)]
        public static bool ClientCommunicationRemotingHandleException = false;
        [ConfigItem("/PlatformSettings", "ClientCommunicationRemotingShowException", null)]
        public static bool ClientCommunicationRemotingShowException = false;
        [ConfigItem("/PlatformSettings", "ClientCommunicationResendDelayTicks", null)]
        public static int ClientCommunicationResendDelayTicks = 0;
        [ConfigItem("/PlatformSettings", "ClientCommunicationResendTimes", null)]
        public static int ClientCommunicationResendTimes = 3;
        [ConfigItem("/PlatformSettings", "ClientCommunicationResendTimesOutPrompt", null)]
        public static bool ClientCommunicationResendTimesOutPrompt = false;
        [ConfigItem("/PlatformSettings", "ClientCommunicationResendUseBreakPoint", null)]
        public static bool ClientCommunicationResendUseBreakPoint = false;
        [ConfigItem("/PlatformSettings", "ClientCommunicationTimeout", null)]
        public static int ClientCommunicationTimeout = 60;
        [ConfigItem("/PlatformSettings", "CommunicationAcceptEnable", null)]
        private static bool CommunicationAcceptEnable = false;
        [ConfigItem("/PlatformSettings", "CommunicationAcceptList", null)]
        private static string CommunicationAcceptList = "";
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastAcceptEnable", null)]
        private static bool CommunicationBroadcastAcceptEnable = true;
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastAcceptList", null)]
        private static string CommunicationBroadcastAcceptList = "";
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastClientDefaultSetting", null)]
        public static string CommunicationBroadcastClientDefaultSetting = null;
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastRefuseEnable", null)]
        private static bool CommunicationBroadcastRefuseEnable = false;
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastRefuseList", null)]
        private static string CommunicationBroadcastRefuseList = "";
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastServerDefaultSetting", null)]
        public static string CommunicationBroadcastServerDefaultSetting = null;
        [ConfigItem("/PlatformSettings", "CommunicationBroadcastServerThreadPriority", null)]
        public static ThreadPriority CommunicationBroadcastServerThreadPriority = ThreadPriority.Normal;
        [ConfigItem("/PlatformSettings", "CommunicationBufferLength", null)]
        public static int CommunicationBufferLength = 0x100000;
        [ConfigItem("/PlatformSettings", "CommunicationClientBroadcastLocalIPEnable", null)]
        public static bool CommunicationClientBroadcastLocalIPEnable = false;
        [ConfigCollectionItem("/PlatformCommunicationClientChannels", "CommunicationChannelsConverter")]
        public static Hashtable CommunicationClientChannels;
        [ConfigItem("/PlatformSettings", "CommunicationClientDefaultSetting", null)]
        public static string CommunicationClientDefaultSetting = null;
        [ConfigCollectionItem("/PlatformCommunicationClientPackets", "CommunicationClientPacketsConverter")]
        public static Hashtable CommunicationClientPackets;
        [ConfigItem("/PlatformSettings", "CommunicationRefuseEnable", null)]
        private static bool CommunicationRefuseEnable = true;
        [ConfigItem("/PlatformSettings", "CommunicationRefuseList", null)]
        private static string CommunicationRefuseList = "";
        [ConfigItem("/PlatformSettings", "CommunicationServerBroadcastLocalIPEnable", null)]
        public static bool CommunicationServerBroadcastLocalIPEnable = false;
        [ConfigCollectionItem("/PlatformCommunicationServerChannels", "CommunicationChannelsConverter")]
        public static Hashtable CommunicationServerChannels;
        [ConfigItem("/PlatformSettings", "CommunicationServerCommonCacheThreadPriority", null)]
        public static ThreadPriority CommunicationServerCommonCacheThreadPriority = ThreadPriority.Normal;
        [ConfigItem("/PlatformSettings", "CommunicationServerDefaultSetting", null)]
        public static string CommunicationServerDefaultSetting = null;
        [ConfigCollectionItem("/PlatformCommunicationServerPackets", "CommunicationClientPacketsConverter")]
        public static Hashtable CommunicationServerPackets;
        [ConfigItem("/PlatformSettings", "CommunicationTcpMaxBufferLimited", null)]
        public static int CommunicationTcpMaxBufferLimited = 0xa00000;
        [ConfigItem("/PlatformSettings", "CommunicationUdpBufferLength", null)]
        public static int CommunicationUdpBufferLength = 0x19000;
        [ConfigItem("/PlatformSettings", "CommunicationUdpMaxBufferLimited", null)]
        public static int CommunicationUdpMaxBufferLimited = 0x100000;
        [ConfigItem("/PlatformSettings", "SecurityLevel", null)]
        public static Platform.CSS.SerialSink.SecurityLevel SecurityLevel = Platform.CSS.SerialSink.SecurityLevel.Mac;
        [ConfigItem("/PlatformSettings", "ServerCommunicationTimeout", null)]
        public static int ServerCommunicationTimeout = 600;
        [ConfigItem("/PlatformSettings", "SocketTcpCheckTimeout", null)]
        public static int SocketTcpCheckTimeout = 1;
        [ConfigItem("/PlatformSettings", "SocketTcpCheckUseThread", null)]
        public static bool SocketTcpCheckUseThread = true;
        [ConfigItem("/PlatformSettings", "SocketTcpCheckValue", null)]
        public static byte SocketTcpCheckValue = 0xb7;
        [ConfigItem("/PlatformSettings", "SocketTcpUseAPI", null)]
        public static bool SocketTcpUseAPI = false;
        [ConfigItem("/PlatformSettings", "WriteSecurityConsoleLog", null)]
        public static bool WriteSecurityConsoleLog = true;
        [ConfigItem("/PlatformSettings", "WriteSecurityLog", null)]
        public static bool WriteSecurityLog = false;

        private static void AfterInitialize()
        {
        }

        private static void BeforeInitialize()
        {
            ConfigManager.RegisterConverter(typeof(CSSConfig));
        }

        [ConverterMethod]
        private static object CommunicationChannelsConverter(Hashtable values, ConfigCollectionItemAttribute attr)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DictionaryEntry entry in values)
            {
                string key = entry.Key as string;
                ConfigValueItem item = entry.Value as ConfigValueItem;
                string configText = item.Value;
                try
                {
                    CommunicationChannelSettings settings = new CommunicationChannelSettings(key, configText);
                    hashtable[settings.Name] = settings;
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            return hashtable;
        }

        [ConverterMethod]
        private static object CommunicationClientPacketsConverter(Hashtable values, ConfigCollectionItemAttribute attr)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DictionaryEntry entry in values)
            {
                string key = entry.Key as string;
                ConfigValueItem item = entry.Value as ConfigValueItem;
                string configText = item.Value;
                try
                {
                    CommunicationPacketSettings settings = new CommunicationPacketSettings(key, configText);
                    hashtable[settings.Name] = settings;
                    continue;
                }
                catch (Exception exception)
                {
                    ExceptionForm.ShowException(exception, false, false, false, true, "¼ÓÔØÍ¨Ñ¶ÅäÖÃÒì³££¡£¡£¡Çë¼ì²éÅäÖÃ£¡£¡£¡");
                    continue;
                }
            }
            return hashtable;
        }

        public static bool IsBroadcastRefusedIP(IPAddress addr)
        {
            return ((addr == null) || IsBroadcastRefusedIP(addr.ToString()));
        }

        public static bool IsBroadcastRefusedIP(string ip)
        {
            return ((CommunicationBroadcastRefuseEnable && (CommunicationBroadcastRefuseList.IndexOf(ip + ";") >= 0)) || (CommunicationBroadcastAcceptEnable && (CommunicationBroadcastAcceptList.IndexOf(ip + ";") < 0)));
        }

        public static bool IsRefusedIP(IPAddress addr)
        {
            return ((addr == null) || IsRefusedIP(addr.ToString()));
        }

        public static bool IsRefusedIP(string ip)
        {
            return ((CommunicationRefuseEnable && (CommunicationRefuseList.IndexOf(ip + ";") >= 0)) || (CommunicationAcceptEnable && (CommunicationAcceptList.IndexOf(ip + ";") < 0)));
        }
    }
}
