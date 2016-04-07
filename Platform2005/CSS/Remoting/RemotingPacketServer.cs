namespace Platform.CSS.Remoting
{
    using Platform.CSS;
    using Platform.CSS.Communication;
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Communication.PacketHandler;
    using Platform.Identity;
    using Platform.Log;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class RemotingPacketServer : ICommunicationPacketHandler
    {
        private Hashtable m_RemoteMethodTable = new Hashtable();

        public static event AfterRemoteCallEventHandler AfterRemoteCall;

        public static event BeforeRemoteCallEventHandler BeforeRemoteCall;

        public CommunicationPacketBase HandlePacket(Guid packetID, CommunicationPacketBase packet)
        {
            RemotingPacket packet2 = packet as RemotingPacket;
            long ticks = DateTime.Now.Ticks;
            bool hasException = false;
            try
            {
                if (BeforeRemoteCall != null)
                {
                    BeforeRemoteCall(packetID, packet2.FullMethodName, packet2.Parameters);
                }
            }
            catch
            {
            }
            try
            {
                packet2.ReturnResult = this.Invoke(packet2.FullMethodName, packet2.Parameters);
            }
            catch (Exception exception)
            {
                hasException = true;
                throw exception;
            }
            finally
            {
                try
                {
                    long num2 = DateTime.Now.Ticks;
                    IUser currentThreadUser = UserManager.GetCurrentThreadUser();
                    string userInfo = (currentThreadUser == null) ? packet2.IPEndPoint.ToString() : currentThreadUser.ToString();
                    TimeSpan span = new TimeSpan(num2 - ticks);
                    OperationLogSink.Write(userInfo, packet2.FullMethodName, "执行时间 " + span.TotalSeconds + " 秒");
                }
                catch
                {
                }
                try
                {
                    if (AfterRemoteCall != null)
                    {
                        AfterRemoteCall(packetID, packet2.FullMethodName, hasException);
                    }
                }
                catch
                {
                }
            }
            for (int i = 0; i < packet2.ParametersDirect.Length; i++)
            {
                if (packet2.ParametersDirect[i] == 0)
                {
                    packet2.Parameters[i] = null;
                }
            }
            return packet2;
        }

        public void InstanceLoadAssemblyRemotingAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                this.InstanceRegisterRemotingAssembly(assembly);
            }
        }

        public void InstanceRegisterRemotingAssembly(Assembly asm)
        {
            int registedCount = 0;
            RemoteClassAssemblyAttribute[] customAttributes = asm.GetCustomAttributes(typeof(RemoteClassAssemblyAttribute), false) as RemoteClassAssemblyAttribute[];
            if (customAttributes.Length >= 1)
            {
                foreach (RemoteClassAssemblyAttribute attribute in customAttributes)
                {
                    this.InstanceRegisterRemotingClass(attribute.Type, out registedCount);
                }
            }
        }

        public void InstanceRegisterRemotingClass(Type type)
        {
            int num;
            this.InstanceRegisterRemotingClass(type, out num);
        }

        public void InstanceRegisterRemotingClass(Type type, out int registedCount)
        {
            registedCount = 0;
            RemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(RemotingClassAttribute), false) as RemotingClassAttribute[];
            if (customAttributes.Length >= 1)
            {
                string name = customAttributes[0].Name;
                switch (name)
                {
                    case null:
                    case "":
                        name = type.Name;
                        break;
                }
                string text2 = customAttributes[0].Namespace;
                if ((text2 == null) || (text2 == ""))
                {
                    text2 = type.Namespace;
                }
                object obj2 = Activator.CreateInstance(type);
                foreach (MethodInfo info in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    UnRemotingAttribute[] attributeArray2 = info.GetCustomAttributes(typeof(UnRemotingAttribute), false) as UnRemotingAttribute[];
                    if (attributeArray2.Length <= 0)
                    {
                        string key = null;
                        RemotingMethodAttribute[] attributeArray3 = info.GetCustomAttributes(typeof(RemotingMethodAttribute), false) as RemotingMethodAttribute[];
                        if ((attributeArray3 != null) && (attributeArray3.Length > 0))
                        {
                            key = attributeArray3[0].FullName;
                        }
                        if (key == null)
                        {
                            ParameterInfo[] parameters = info.GetParameters();
                            if (parameters == null)
                            {
                                goto Label_01D6;
                            }
                            string text4 = "";
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                text4 = text4 + parameters[i].ParameterType.Name;
                                if (i < (parameters.Length - 1))
                                {
                                    text4 = text4 + ",";
                                }
                            }
                            key = text2 + "+" + name + "::" + info.Name + "(" + text4 + ")";
                        }
                        RemoteCallItem item = new RemoteCallItem(info.IsStatic ? null : obj2, info);
                        if (this.m_RemoteMethodTable.ContainsKey(key))
                        {
                            TraceHelper.WriteLine("错误：存在重复的调用接口：" + key);
                        }
                        this.m_RemoteMethodTable[key] = item;
                        registedCount++;
                    Label_01D6: ;
                    }
                }
                TraceHelper.WriteLine(string.Concat(new object[] { "加载Remote接口处理：", type.FullName, "完成！接口数：", (int)registedCount }));
            }
        }

        private object Invoke(string fullMethodName, object[] parameters)
        {
            RemoteCallItem item = this.m_RemoteMethodTable[fullMethodName] as RemoteCallItem;
            if (item == null)
            {
                throw new Exception("无效调用方法：" + fullMethodName);
            }
            return item.Method.Invoke(item.Instance, parameters);
        }

        public static void LoadAssemblyRemotingAssemblies()
        {
            LoadAssemblyRemotingAssemblies(CSSConfig.CommunicationServerDefaultSetting);
        }

        public static void LoadAssemblyRemotingAssemblies(string settingName)
        {
            RemotingPacketServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(RemotingPacket)) as RemotingPacketServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceLoadAssemblyRemotingAssemblies();
            }
        }

        public static void RegisterRemotingAssembly(Assembly asm)
        {
            RegisterRemotingAssembly(CSSConfig.CommunicationServerDefaultSetting, asm);
        }

        public static void RegisterRemotingAssembly(string settingName, Assembly asm)
        {
            RemotingPacketServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(RemotingPacket)) as RemotingPacketServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceRegisterRemotingAssembly(asm);
            }
        }

        public static void RegisterRemotingClass(Type type)
        {
            RegisterRemotingClass(CSSConfig.CommunicationServerDefaultSetting, type);
        }

        public static void RegisterRemotingClass(string settingName, Type type)
        {
            RemotingPacketServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(RemotingPacket)) as RemotingPacketServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceRegisterRemotingClass(type);
            }
        }

        public static void RegisterRemotingClass(Type type, out int registedCount)
        {
            RegisterRemotingClass(CSSConfig.CommunicationServerDefaultSetting, type, out registedCount);
        }

        public static void RegisterRemotingClass(string settingName, Type type, out int registedCount)
        {
            RemotingPacketServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(RemotingPacket)) as RemotingPacketServer;
            if (communicationPacketHandler == null)
            {
                registedCount = 0;
            }
            else
            {
                communicationPacketHandler.InstanceRegisterRemotingClass(type, out registedCount);
            }
        }

        public bool CacheResult
        {
            get
            {
                return true;
            }
        }

        public bool IsBroadcastHandler
        {
            get
            {
                return false;
            }
        }

        internal sealed class RemoteCallItem
        {
            public object Instance;
            public MethodInfo Method;

            public RemoteCallItem(object instance, MethodInfo method)
            {
                this.Method = method;
                this.Instance = instance;
            }
        }
    }
}
