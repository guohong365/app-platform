namespace Platform.Configuration.Utils
{
    using Platform.Utils;
    using System;

    public class ConfigItem
    {
        private string m_ConfigDefaultValue;
        private string m_ConfigDescription;
        private string m_ConfigGroup;
        private string m_ConfigName;
        private string m_ConfigNativeName;
        private string m_ConfigType;
        private string m_ConfigUIType;
        private string m_ConfigValue;
        private string m_MatchValue;
        private System.Type m_Type;

        public ConfigItem(string matchValue, string configNativeName, string configName, string configGroup, string configType, string configUIType, string configDefaultValue, string configDescription)
        {
            this.m_ConfigNativeName = configNativeName;
            this.m_ConfigName = configName;
            this.m_ConfigGroup = configGroup;
            this.m_ConfigType = configType;
            this.m_Type = TypeUtility.GetTypeFromName(configType);
            this.m_ConfigUIType = configUIType;
            this.m_ConfigDefaultValue = configDefaultValue;
            this.m_ConfigValue = configDefaultValue;
            this.m_ConfigDescription = configDescription;
            this.m_MatchValue = matchValue;
        }

        public string ConfigDefaultValue
        {
            get
            {
                return this.m_ConfigDefaultValue;
            }
        }

        public string ConfigDescription
        {
            get
            {
                return this.m_ConfigDescription;
            }
        }

        public string ConfigGroup
        {
            get
            {
                return this.m_ConfigGroup;
            }
        }

        public string ConfigName
        {
            get
            {
                return this.m_ConfigName;
            }
        }

        public string ConfigNativeName
        {
            get
            {
                return this.m_ConfigNativeName;
            }
        }

        public string ConfigType
        {
            get
            {
                return this.m_ConfigType;
            }
        }

        public string ConfigUIType
        {
            get
            {
                return this.m_ConfigUIType;
            }
        }

        public string ConfigValue
        {
            get
            {
                return this.m_ConfigValue;
            }
            set
            {
                this.m_ConfigValue = value;
            }
        }

        public string MatchValue
        {
            get
            {
                return this.m_MatchValue;
            }
        }

        public System.Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
    }
}
