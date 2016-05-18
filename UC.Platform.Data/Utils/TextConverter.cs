using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using UC.Platform.Data.DBHelper;

namespace UC.Platform.Data.Utils
{
    public sealed class TextConverter
    {
        private static readonly Hashtable m_Converters = new Hashtable();
        private static readonly Hashtable m_TypeConverters = new Hashtable();

        static TextConverter()
        {
            RegisterConverter(typeof(TextConverter));
        }

        [TextConverterMethod(typeof(byte[]))]
        public static object Base64ByteArrayConverter(Type type, string text)
        {
            return System.Convert.FromBase64String(text);
        }

        [TextConverterMethod(typeof(bool))]
        public static object BooleanConverter(Type type, string text)
        {
            return bool.Parse(text);
        }

        [TextConverterMethod(typeof(char))]
        public static object CharConverter(Type type, string text)
        {
            return char.Parse(text);
        }

        public static object Convert(Type type, string text, string converter)
        {
            TextConvertHandler handler;
            if (converter == null)
            {
                if (type == null)
                {
                    return text;
                }
                if (type.IsEnum)
                {
                    handler = m_TypeConverters[typeof(System.Enum)] as TextConvertHandler;
                }
                else
                {
                    handler = m_TypeConverters[type] as TextConvertHandler;
                }
            }
            else
            {
                handler = m_Converters[converter] as TextConvertHandler;
            }
            if (handler == null)
            {
                return text;
            }
            return handler(type, text);
        }

        [TextConverterMethod(typeof(DateTime))]
        public static object DateTimeConverter(Type type, string text)
        {
            return DateTime.Parse(text);
        }

        [TextConverterMethod(typeof(System.Enum))]
        public static object EnumConverter(Type type, string text)
        {
            return System.Enum.Parse(type, text, true);
        }

        [TextConverterMethod(typeof(Guid))]
        public static object GuidConverter(Type type, string text)
        {
            return new Guid(text);
        }

        [TextConverterMethod(typeof(short))]
        public static object Int16Converter(Type type, string text)
        {
            return short.Parse(text);
        }

        [TextConverterMethod(typeof(int))]
        public static object Int32Converter(Type type, string text)
        {
            return int.Parse(text);
        }

        [TextConverterMethod(typeof(long))]
        public static object Int64Converter(Type type, string text)
        {
            return long.Parse(text);
        }

        [TextConverterMethod(typeof(sbyte))]
        public static object Int8Converter(Type type, string text)
        {
            return sbyte.Parse(text);
        }

        [TextConverterMethod(typeof(IPAddress))]
        public static object IPAddressConverter(Type type, string text)
        {
            return IPAddress.Parse(text);
        }

        public static void RegisterConverter(Type type)
        {
            foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (info.IsStatic)
                {
                    TextConverterMethodAttribute[] customAttributes = info.GetCustomAttributes(typeof(TextConverterMethodAttribute), false) as TextConverterMethodAttribute[];
                    if ((customAttributes.Length > 0) && (info.ReturnType == typeof(object)))
                    {
                        ParameterInfo[] parameters = info.GetParameters();
                        if (((parameters.Length == 2) && (parameters[0].ParameterType == typeof(Type))) && (parameters[1].ParameterType == typeof(string)))
                        {
                            m_Converters[info.Name] = Delegate.CreateDelegate(typeof(TextConvertHandler), info);
                            if (customAttributes[0].ConvertType != null)
                            {
                                m_TypeConverters[customAttributes[0].ConvertType] = m_Converters[info.Name];
                            }
                        }
                    }
                }
            }
        }

        public static void RegisterConverter(string converterName, TextConvertHandler handler)
        {
            m_Converters[converterName] = handler;
        }

        [TextConverterMethod(typeof(string))]
        public static object StringConverter(Type type, string text)
        {
            return text;
        }

        [TextConverterMethod(typeof(Encoding))]
        public static object TextEncodingConverter(Type type, string text)
        {
            return Encoding.GetEncoding(text);
        }

        [TextConverterMethod(typeof(Type))]
        public static object TypeConverter(Type type, string text)
        {
            return TypeUtility.GetTypeFromName(text);
        }

        [TextConverterMethod(typeof(ushort))]
        public static object UInt16Converter(Type type, string text)
        {
            return ushort.Parse(text);
        }

        [TextConverterMethod(typeof(uint))]
        public static object UInt32Converter(Type type, string text)
        {
            return uint.Parse(text);
        }

        [TextConverterMethod(typeof(ulong))]
        public static object UInt64Converter(Type type, string text)
        {
            return ulong.Parse(text);
        }

        [TextConverterMethod(typeof(byte))]
        public static object UInt8Converter(Type type, string text)
        {
            return byte.Parse(text);
        }
    }
}
