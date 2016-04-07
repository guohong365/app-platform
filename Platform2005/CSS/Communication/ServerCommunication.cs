namespace Platform.CSS.Communication
{
    using Platform.CSS;
    using Platform.CSS.Communication.Channels;
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Communication.PacketHandler;
    using Platform.CSS.PacketHandling;
    using Platform.CSS.SerialSink;
    using Platform.ExceptionHandling;
    using Platform.Identity;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.Net;
    using System.Runtime.InteropServices;

    public sealed class ServerCommunication
    {
        private static Hashtable m_Channels = new Hashtable();
        private static Hashtable m_Handlers = new Hashtable();

        static ServerCommunication()
        {
            RegisterServerCommunicationPacketHandler("__SERVER__", typeof(ExceptionPacket), typeof(ExceptionPacketHandler), new Type[] { typeof(ZipSerialSink) });
        }

        public static ICommunicationPacketHandler GetCommunicationPacketHandler(string name, Type packetType)
        {
            return (m_Handlers[name + "::" + packetType] as ICommunicationPacketHandler);
        }

        public static bool HandlePacket(string name, MemoryStream ms, out MemoryStream msrc, out bool breakFlag)
        {
            return HandlePacket(name, ms, out msrc, out breakFlag, null);
        }

        public static bool HandlePacket(string name, MemoryStream ms, out MemoryStream msrc, out bool breakFlag, IPEndPoint ipendpoint)
        {
            CommunicationPacketBase base2 = null;
            CommunicationPacketBase commPacket = null;
            ICommunicationPacketHandler handler = null;
            Guid empty = Guid.Empty;
            int threadID = 0;
            int redoTimes = 0;
            msrc = null;
            breakFlag = false;
            try
            {
                base2 = CommunicationPacketBase.Deserialize(name, out empty, out threadID, out redoTimes, ms, true, true);
                if (base2 == null)
                {
                   throw new Exception("反序列化错误！" + name);
                }
                base2.IPEndPoint = ipendpoint;
                handler = m_Handlers[name + "::" + base2.GetType()] as ICommunicationPacketHandler;
                if (handler == null)
                {
                    throw new Exception(string.Concat(new object[] { "获取处理器错误：", name, "::", base2.GetType() }));
                }
                if (!handler.IsBroadcastHandler)
                {
                    UserManager.CurrentUserActiveTimeChanged();
                }
                if (handler.CacheResult && (redoTimes > 0))
                {
                    ServerCacheResultManager.GetServerResult(name, UserManager.GetCurrentThreadUser(), empty, threadID, out msrc);
                    if (msrc != null)
                    {
                        breakFlag = true;
                        return true;
                    }
                }
                redoTimes = 0;
                commPacket = handler.HandlePacket(empty, base2);
                if (handler.IsBroadcastHandler)
                {
                    return false;
                }
                if (commPacket == null)
                {
                    throw new Exception("服务器处理错误！");
                }
                if (!CommunicationPacketBase.Serialize(name, empty, threadID, redoTimes, commPacket, out msrc, true))
                {
                   throw new Exception("序列化结果错误！");
                }
                if (handler.CacheResult && (msrc != null))
                {
                    ServerCacheResultManager.SaveServerResult(name, UserManager.GetCurrentThreadUser(), empty, threadID, msrc);
                }
            }
            catch (Exception innerException)
            {
                breakFlag = false;
                msrc = null;
                try
                {
                    while (innerException.InnerException != null)
                    {
                        innerException = innerException.InnerException;
                    }
                    Platform.ExceptionHandling.ExceptionHelper.HandleException(innerException);
                    if ((handler != null) && handler.IsBroadcastHandler)
                    {
                        return false;
                    }
                    ExceptionPacket packet = new ExceptionPacket(innerException);
                    redoTimes = 0;
                    if (!CommunicationPacketBase.Serialize("", empty, threadID, redoTimes, packet, out msrc, true))
                    {
                        return false;
                    }
                    if (((handler != null) && handler.CacheResult) && (msrc != null))
                    {
                        ServerCacheResultManager.SaveServerResult(name, UserManager.GetCurrentThreadUser(), empty, threadID, msrc);
                    }
                    return true;
                }
                catch (Exception exception2)
                {
                    msrc = null;
                    try
                    {
                        Platform.ExceptionHandling.ExceptionHelper.HandleException(exception2);
                    }
                    catch
                    {
                    }
                    return false;
                }
            }
            finally
            {
                try
                {
                    if (base2 != null)
                    {
                        base2.FreeBuffer();
                    }
                }
                catch (Exception exception3)
                {
                    try
                    {
                        Platform.ExceptionHandling.ExceptionHelper.HandleException(exception3);
                    }
                    catch
                    {
                    }
                }
            }
            return true;
        }

        public static void RegisterCommunicationServerChannels(CommunicationChannelSettings infos)
        {
            Hashtable hashtable = m_Channels[infos.Name] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_Channels[infos.Name] = hashtable;
            }
            foreach (CommunicationChannelSetting setting in infos.CommunicationChannels)
            {
                if (setting.ChannelType.GetInterface("Platform.CSS.Communication.Channels.IServerChannel", false) != null)
                {
                    try
                    {
                        IServerChannel channel = Activator.CreateInstance(setting.ChannelType) as IServerChannel;
                        if ((channel != null) && channel.InitializeChannel(infos.Name, setting.IPEndPoints))
                        {
                            hashtable[setting.ChannelName] = channel;
                        }
                    }
                    catch (Exception exception)
                    {
                        Platform.ExceptionHandling.ExceptionHelper.HandleException(exception, new string[] { "初始化 " + setting.ChannelName + " 失败！" });
                    }
                }
            }
        }

        public static void RegisterCommunicationServerChannels(Hashtable settings)
        {
            foreach (CommunicationChannelSettings settings2 in settings.Values)
            {
                RegisterCommunicationServerChannels(settings2);
            }
        }

        public static void RegisterCommunicationServerPackets(CommunicationPacketSettings settings)
        {
            foreach (CommunicationPacketSetting setting in settings.CommunicationPackets)
            {
                RegisterServerCommunicationPacketHandler(settings.Name, setting.PackageType, setting.PackageHandlerType, setting.PackageSinkTypes);
            }
        }

        public static void RegisterCommunicationServerPackets(Hashtable settings)
        {
            if (settings != null)
            {
                foreach (CommunicationPacketSettings settings2 in settings.Values)
                {
                    RegisterCommunicationServerPackets(settings2);
                }
            }
        }

        public static void RegisterServerCommunicationPacketHandler(string name, Type packetType, Type handlerType)
        {
            if (handlerType.GetInterface("Platform.CSS.Communication.PacketHandler.ICommunicationPacketHandler") != null)
            {
                CommunicationPacketBase.RegisterCommunicationPacketType(name, packetType);
                m_Handlers[name + "::" + packetType] = Activator.CreateInstance(handlerType);
            }
        }

        public static void RegisterServerCommunicationPacketHandler(string name, Type packetType, Type handlerType, params Type[] sinkTypes)
        {
            if (handlerType.GetInterface("Platform.CSS.Communication.PacketHandler.ICommunicationPacketHandler") != null)
            {
                CommunicationPacketBase.RegisterCommunicationPacketType(name, packetType, sinkTypes);
                m_Handlers[name + "::" + packetType] = Activator.CreateInstance(handlerType);
            }
        }

        public static bool StartAllCommunicationServerChannels()
        {
            foreach (string text in m_Channels.Keys)
            {
                StartCommunicationServerChannels(text);
            }
            return true;
        }

        public static bool StartCommunicationServerChannels()
        {
            return StartCommunicationServerChannels(CSSConfig.CommunicationServerDefaultSetting);
        }

        public static bool StartCommunicationServerChannels(string settingName)
        {
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable == null)
            {
                return false;
            }
            foreach (IServerChannel channel in hashtable.Values)
            {
                try
                {
                    channel.StartChannel();
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            return true;
        }

        public static bool StopAllCommunicationClients()
        {
            foreach (string text in m_Channels.Keys)
            {
                StopCommunicationClients(text);
            }
            return true;
        }

        public static bool StopAllCommunicationServerChannels()
        {
            foreach (string text in m_Channels.Keys)
            {
                StopCommunicationServerChannels(text);
            }
            return true;
        }

        public static bool StopCommunicationClients()
        {
            return StopCommunicationClients(CSSConfig.CommunicationServerDefaultSetting);
        }

        public static bool StopCommunicationClients(string settingName)
        {
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable == null)
            {
                return false;
            }
            foreach (IServerChannel channel in hashtable.Values)
            {
                try
                {
                    channel.StopClients();
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            return true;
        }

        public static bool StopCommunicationServerChannels()
        {
            return StopCommunicationServerChannels(CSSConfig.CommunicationServerDefaultSetting);
        }

        public static bool StopCommunicationServerChannels(string settingName)
        {
            Hashtable hashtable = m_Channels[settingName] as Hashtable;
            if (hashtable == null)
            {
                return false;
            }
            foreach (IServerChannel channel in hashtable.Values)
            {
                try
                {
                    channel.StopChannel();
                    continue;
                }
                catch
                {
                    continue;
                }
            }
            return true;
        }
    }
}
