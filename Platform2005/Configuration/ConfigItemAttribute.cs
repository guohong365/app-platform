namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConfigItemAttribute : Attribute
    {
        private string m_Converter;
        private string m_Key;
        private string m_SectionName;

        public ConfigItemAttribute()
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(null);
            this.m_Key = null;
            this.m_Converter = null;
        }

        public ConfigItemAttribute(string configKey)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(null);
            this.m_Key = configKey;
            this.m_Converter = null;
        }

        public ConfigItemAttribute(string configKey, string converter)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(null);
            this.m_Key = configKey;
            this.m_Converter = converter;
        }

        public ConfigItemAttribute(string sectionName, string configKey, string converter)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Key = configKey;
            this.m_Converter = converter;
        }

        public string Converter
        {
            get
            {
                return this.m_Converter;
            }
            set
            {
                this.m_Converter = value;
            }
        }

        public string Key
        {
            get
            {
                return this.m_Key;
            }
            set
            {
                this.m_Key = value;
            }
        }

        public string SectionName
        {
            get
            {
                return this.m_SectionName;
            }
            set
            {
                this.m_SectionName = value;
            }
        }
    }
}
