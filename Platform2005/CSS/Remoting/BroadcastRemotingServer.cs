namespace Platform.CSS.Remoting
{
    using Platform.CSS;
    using Platform.CSS.Communication;
    using Platform.CSS.Communication.Packet;
    using Platform.CSS.Communication.PacketHandler;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class BroadcastRemotingServer : ICommunicationPacketHandler
    {
        private Hashtable m_RemoteMethodTable = new Hashtable();

        public void Clear()
        {
        }

        public CommunicationPacketBase HandlePacket(Guid packetID, CommunicationPacketBase packet)
        {
            BroadcastRemotingPacket packet2 = packet as BroadcastRemotingPacket;
            this.Invoke(packet2.FullName, packet2.Parameters);
            return null;
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
            BoradcastRemoteClassAssemblyAttribute[] customAttributes = asm.GetCustomAttributes(typeof(BoradcastRemoteClassAssemblyAttribute), false) as BoradcastRemoteClassAssemblyAttribute[];
            if (customAttributes.Length >= 1)
            {
                foreach (BoradcastRemoteClassAssemblyAttribute attribute in customAttributes)
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
            BroadcastRemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(BroadcastRemotingClassAttribute), false) as BroadcastRemotingClassAttribute[];
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
                    if (info.ReturnType == typeof(void))
                    {
                        string key = null;
                        BroadcastRemotingMethodAttribute[] attributeArray2 = info.GetCustomAttributes(typeof(BroadcastRemotingMethodAttribute), false) as BroadcastRemotingMethodAttribute[];
                        if ((attributeArray2 != null) && (attributeArray2.Length > 0))
                        {
                            key = attributeArray2[0].FullName;
                        }
                        ParameterInfo[] parameters = info.GetParameters();
                        if (parameters != null)
                        {
                            bool flag = false;
                            string text4 = "";
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].IsOut)
                                {
                                    flag = true;
                                }
                                if (parameters[i].ParameterType.IsByRef)
                                {
                                    flag = true;
                                }
                                text4 = text4 + parameters[i].ParameterType.Name;
                                if (i < (parameters.Length - 1))
                                {
                                    text4 = text4 + ",";
                                }
                            }
                            if (key == null)
                            {
                                key = text2 + "+" + name + "::" + info.Name + "(" + text4 + ")";
                            }
                            if (flag)
                            {
                                 TraceHelper.WriteLine("错误：广播方法参数不符合规则：" + key);
                            }
                            else
                            {
                                RemoteCallItem item = new RemoteCallItem(info.IsStatic ? null : obj2, info);
                                if (this.m_RemoteMethodTable.ContainsKey(key))
                                {
                                    TraceHelper.WriteLine("错误：存在重复的广播调用接口：" + key);
                                }
                                this.m_RemoteMethodTable[key] = item;
                                registedCount++;
                            }
                        }
                    }
                }
                 TraceHelper.WriteLine(string.Concat(new object[] { "加载BroadcastRemote接口处理：", type.FullName, "完成！接口数：", (int) registedCount }));
            }
        }

        private object Invoke(string fullName, object[] parameters)
        {
            RemoteCallItem item = this.m_RemoteMethodTable[fullName] as RemoteCallItem;
            if (item == null)
            {
                throw new Exception("无效广播调用方法：" + fullName);
            }
            return item.Method.Invoke(item.Instance, parameters);
        }

        public static void LoadAssemblyRemotingAssemblies()
        {
            LoadAssemblyRemotingAssemblies(CSSConfig.CommunicationBroadcastServerDefaultSetting);
        }

        public static void LoadAssemblyRemotingAssemblies(string settingName)
        {
            BroadcastRemotingServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(BroadcastRemotingPacket)) as BroadcastRemotingServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceLoadAssemblyRemotingAssemblies();
            }
        }

        public static void RegisterRemotingAssembly(Assembly asm)
        {
            RegisterRemotingAssembly(CSSConfig.CommunicationBroadcastServerDefaultSetting, asm);
        }

        public static void RegisterRemotingAssembly(string settingName, Assembly asm)
        {
            BroadcastRemotingServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(BroadcastRemotingPacket)) as BroadcastRemotingServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceRegisterRemotingAssembly(asm);
            }
        }

        public static void RegisterRemotingClass(Type type)
        {
            RegisterRemotingClass(CSSConfig.CommunicationBroadcastServerDefaultSetting, type);
        }

        public static void RegisterRemotingClass(string settingName, Type type)
        {
            BroadcastRemotingServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(BroadcastRemotingPacket)) as BroadcastRemotingServer;
            if (communicationPacketHandler != null)
            {
                communicationPacketHandler.InstanceRegisterRemotingClass(type);
            }
        }

        public static void RegisterRemotingClass(Type type, out int registedCount)
        {
            RegisterRemotingClass(CSSConfig.CommunicationBroadcastServerDefaultSetting, type, out registedCount);
        }

        public static void RegisterRemotingClass(string settingName, Type type, out int registedCount)
        {
            BroadcastRemotingServer communicationPacketHandler = ServerCommunication.GetCommunicationPacketHandler(settingName, typeof(BroadcastRemotingPacket)) as BroadcastRemotingServer;
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
                return false;
            }
        }

        public bool IsBroadcastHandler
        {
            get
            {
                return true;
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
