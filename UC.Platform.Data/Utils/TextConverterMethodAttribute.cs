using System;

namespace UC.Platform.Data.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TextConverterMethodAttribute : Attribute
    {
        public TextConverterMethodAttribute()
        {
            ConvertType = null;
        }

        public TextConverterMethodAttribute(Type convertType)
        {
            ConvertType = convertType;
        }

        public Type ConvertType { get; set; }
    }
}
