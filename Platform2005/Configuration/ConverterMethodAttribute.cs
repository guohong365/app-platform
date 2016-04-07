namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConverterMethodAttribute : Attribute
    {
        private Type m_ConvertType;

        public ConverterMethodAttribute()
        {
            this.m_ConvertType = null;
        }

        public ConverterMethodAttribute(Type convertType)
        {
            this.m_ConvertType = convertType;
        }

        public Type ConvertType
        {
            get
            {
                return this.m_ConvertType;
            }
            set
            {
                this.m_ConvertType = value;
            }
        }
    }
}
