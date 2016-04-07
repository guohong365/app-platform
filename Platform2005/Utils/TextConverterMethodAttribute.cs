namespace Platform.Utils
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TextConverterMethodAttribute : Attribute
    {
        private Type m_ConvertType;

        public TextConverterMethodAttribute()
        {
            this.m_ConvertType = null;
        }

        public TextConverterMethodAttribute(Type convertType)
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
