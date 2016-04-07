namespace Platform.CSS.Communication
{
    using Platform.Utils;
    using System;
    using System.Collections;

    public sealed class CommunicationPacketSetting
    {
        private Type m_PackageHandlerType;
        private Type[] m_PackageSinkTypes;
        private Type m_PackageType;

        private CommunicationPacketSetting(Type packageType, Type packageHandlerType, Type[] packageSinkTypes)
        {
            this.m_PackageType = packageType;
            this.m_PackageHandlerType = packageHandlerType;
            this.m_PackageSinkTypes = packageSinkTypes;
        }

        public static CommunicationPacketSetting GetCommunicationPacketSetting(string init)
        {
            try
            {
                string[] textArray = init.Split(new char[] { ':' });
                Type typeFromName = TypeUtility.GetTypeFromName(textArray[0]);
                if (typeFromName == null)
                {
                    return null;
                }
                Type packageHandlerType = null;
                if ((textArray.Length > 1) && (textArray[1].Trim().Length > 0))
                {
                    packageHandlerType = TypeUtility.GetTypeFromName(textArray[1]);
                }
                ArrayList list = new ArrayList();
                for (int i = 2; i < textArray.Length; i++)
                {
                    list.Add(TypeUtility.GetTypeFromName(textArray[i]));
                }
                return new CommunicationPacketSetting(typeFromName, packageHandlerType, list.ToArray(typeof(Type)) as Type[]);
            }
            catch
            {
                return null;
            }
        }

        public static CommunicationPacketSetting[] GetCommunicationPacketSettings(string inits)
        {
            CommunicationPacketSetting[] settingArray;
            if (inits == null)
            {
                 throw new Exception("GetCommunicationInfos ²ÎÊýÅäÖÃ´íÎó£¡");
            }
            try
            {
                string[] textArray = inits.Split(new char[] { ',' });
                ArrayList list = new ArrayList();
                foreach (string text in textArray)
                {
                    CommunicationPacketSetting communicationPacketSetting = GetCommunicationPacketSetting(text);
                    if (communicationPacketSetting != null)
                    {
                        list.Add(communicationPacketSetting);
                    }
                }
                settingArray = list.ToArray(typeof(CommunicationPacketSetting)) as CommunicationPacketSetting[];
            }
            catch
            {
                throw new Exception("GetCommunicationInfos ²ÎÊýÅäÖÃ´íÎó£¡");
            }
            return settingArray;
        }

        public Type PackageHandlerType
        {
            get
            {
                return this.m_PackageHandlerType;
            }
        }

        public Type[] PackageSinkTypes
        {
            get
            {
                return this.m_PackageSinkTypes;
            }
        }

        public Type PackageType
        {
            get
            {
                return this.m_PackageType;
            }
        }
    }
}
