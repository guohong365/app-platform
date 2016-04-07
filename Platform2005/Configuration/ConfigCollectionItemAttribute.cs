namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConfigCollectionItemAttribute : Attribute
    {
        private Type m_CollectionItemType;
        private string m_Converter;
        private string m_ItemConverter;
        private string m_KeyStart;
        private string m_SectionName;

        public ConfigCollectionItemAttribute()
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(null);
            this.m_Converter = null;
            this.m_KeyStart = null;
            this.m_CollectionItemType = null;
            this.m_ItemConverter = null;
        }

        public ConfigCollectionItemAttribute(string sectionName)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = null;
            this.m_KeyStart = null;
            this.m_CollectionItemType = null;
            this.m_ItemConverter = null;
        }

        public ConfigCollectionItemAttribute(string sectionName, string converter)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = converter;
            this.m_KeyStart = null;
            this.m_CollectionItemType = null;
            this.m_ItemConverter = null;
        }

        public ConfigCollectionItemAttribute(string sectionName, string keyStart, string converter)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = converter;
            this.m_KeyStart = keyStart;
            this.m_CollectionItemType = null;
            this.m_ItemConverter = null;
        }

        public ConfigCollectionItemAttribute(string sectionName, string converter, Type collectionItemType)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = converter;
            this.m_KeyStart = null;
            this.m_CollectionItemType = collectionItemType;
            this.m_ItemConverter = null;
        }

        public ConfigCollectionItemAttribute(string sectionName, string keyStart, string converter, string itemConverter)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = converter;
            this.m_KeyStart = keyStart;
            this.m_CollectionItemType = null;
            this.m_ItemConverter = itemConverter;
        }

        public ConfigCollectionItemAttribute(string sectionName, string keyStart, string converter, Type collectionItemType)
        {
            this.m_SectionName = ConfigHelper.RegularSectionName(sectionName);
            this.m_Converter = converter;
            this.m_KeyStart = keyStart;
            this.m_CollectionItemType = collectionItemType;
            this.m_ItemConverter = null;
        }

        public Type CollectionItemType
        {
            get
            {
                return this.m_CollectionItemType;
            }
            set
            {
                this.m_CollectionItemType = value;
            }
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

        public string ItemConverter
        {
            get
            {
                return this.m_ItemConverter;
            }
            set
            {
                this.m_ItemConverter = value;
            }
        }

        public string KeyStart
        {
            get
            {
                return this.m_KeyStart;
            }
            set
            {
                this.m_KeyStart = value;
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
