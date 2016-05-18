using System;

namespace UC.Platform.Data.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FillFieldAttribute : Attribute
    {
        private string m_ConverterName;
        private object m_DefaultValue;
        private string m_FieldName;

        public FillFieldAttribute()
        {
            this.m_FieldName = null;
            this.m_ConverterName = null;
        }

        public FillFieldAttribute(string fieldName)
        {
            this.m_FieldName = fieldName;
            this.m_ConverterName = null;
        }

        public FillFieldAttribute(string fieldName, string converterName)
        {
            this.m_FieldName = fieldName;
            this.m_ConverterName = converterName;
        }

        public string ConverterName
        {
            get
            {
                return this.m_ConverterName;
            }
            set
            {
                this.m_ConverterName = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                return this.m_DefaultValue;
            }
            set
            {
                this.m_DefaultValue = value;
            }
        }

        public string FieldName
        {
            get
            {
                return this.m_FieldName;
            }
            set
            {
                this.m_FieldName = value;
            }
        }
    }
}
