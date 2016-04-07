namespace Platform.Configuration
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;

    public sealed class ConfigConverters
    {
        private static Hashtable m_CollectionConverters = new Hashtable();
        private static Hashtable m_CollectionTypeConverters = new Hashtable();
        private static Hashtable m_Converters = new Hashtable();
        private static Hashtable m_TypeConverters = new Hashtable();

        static ConfigConverters()
        {
            RegisterConverter(typeof(ConfigConverters));
        }

        [ConverterMethod(typeof(ArrayList))]
        private static object ArrayListConverter(Hashtable values, ConfigCollectionItemAttribute attr)
        {
            ArrayList list = new ArrayList();
            foreach (DictionaryEntry entry in values)
            {
                string key = (string) entry.Key;
                if ((attr.KeyStart == null) || key.StartsWith(attr.KeyStart))
                {
                    string configText = ((ConfigValueItem) entry.Value).Value;
                    list.Add(Convert(attr.CollectionItemType, configText, attr.ItemConverter));
                }
            }
            return list;
        }

        [ConverterMethod(typeof(byte[]))]
        private static object Base64ByteArrayConverter(Type type, string configText)
        {
            return System.Convert.FromBase64String(configText);
        }

        [ConverterMethod(typeof(bool))]
        private static object BooleanConverter(Type type, string configText)
        {
            return bool.Parse(configText);
        }

        [ConverterMethod(typeof(char))]
        private static object CharConverter(Type type, string configText)
        {
            return char.Parse(configText);
        }

        public static object Convert(Type type, Hashtable values, ConfigCollectionItemAttribute attr)
        {
            ConfigConvertCollectionHandler handler;
            if (attr.Converter == null)
            {
                handler = m_CollectionTypeConverters[type] as ConfigConvertCollectionHandler;
            }
            else
            {
                handler = m_CollectionConverters[attr.Converter] as ConfigConvertCollectionHandler;
            }
            if (handler == null)
            {
                return Activator.CreateInstance(type);
            }
            return handler(values, attr);
        }

        public static object Convert(Type type, string configText, string converter)
        {
            ConfigConvertHandler handler;
            if (converter == null)
            {
                if (type == null)
                {
                    return configText;
                }
                if (type.IsEnum)
                {
                    handler = m_TypeConverters[typeof(System.Enum)] as ConfigConvertHandler;
                }
                else
                {
                    handler = m_TypeConverters[type] as ConfigConvertHandler;
                }
            }
            else
            {
                handler = m_Converters[converter] as ConfigConvertHandler;
            }
            if (handler == null)
            {
                return configText;
            }
            return handler(type, configText);
        }

        [ConverterMethod(typeof(DateTime))]
        private static object DateTimeConverter(Type type, string configText)
        {
            return DateTime.Parse(configText);
        }

        [ConverterMethod(typeof(System.Enum))]
        private static object EnumConverter(Type type, string configText)
        {
            return System.Enum.Parse(type, configText, true);
        }

        [ConverterMethod(typeof(Guid))]
        private static object GuidConverter(Type type, string configText)
        {
            return new Guid(configText);
        }

        [ConverterMethod(typeof(Hashtable))]
        private static object HashTableConverter(Hashtable values, ConfigCollectionItemAttribute attr)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DictionaryEntry entry in values)
            {
                string key = (string) entry.Key;
                if ((attr.KeyStart == null) || key.StartsWith(attr.KeyStart))
                {
                    string configText = ((ConfigValueItem) entry.Value).Value;
                    hashtable[key] = Convert(attr.CollectionItemType, configText, attr.ItemConverter);
                }
            }
            return hashtable;
        }

        [ConverterMethod]
        private static object HexStringToBytesConverter(Type type, string configText)
        {
            if (configText.Length < 2)
            {
                return null;
            }
            string[] textArray = configText.ToUpper().Replace("0X", "").Replace(" ", "").Split(new char[] { ',' });
            byte[] buffer = new byte[textArray.Length];
            for (int i = 0; i < textArray.Length; i++)
            {
                buffer[i] = byte.Parse(textArray[i], NumberStyles.HexNumber);
            }
            return buffer;
        }

        [ConverterMethod(typeof(short))]
        private static object Int16Converter(Type type, string configText)
        {
            return short.Parse(configText);
        }

        [ConverterMethod(typeof(int))]
        private static object Int32Converter(Type type, string configText)
        {
            return int.Parse(configText);
        }

        [ConverterMethod(typeof(long))]
        private static object Int64Converter(Type type, string configText)
        {
            return long.Parse(configText);
        }

        [ConverterMethod(typeof(sbyte))]
        private static object Int8Converter(Type type, string configText)
        {
            return sbyte.Parse(configText);
        }

        [ConverterMethod(typeof(IPAddress))]
        private static object IPAddressConverter(Type type, string configText)
        {
            return IPAddress.Parse(configText);
        }

        [ConverterMethod]
        private static object PathConverter(Type type, string configText)
        {
            return Path.GetFullPath(configText.Replace("/", @"\"));
        }

        public static void RegisterConverter(Type type)
        {
            foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (info.IsStatic)
                {
                    ConverterMethodAttribute[] customAttributes = info.GetCustomAttributes(typeof(ConverterMethodAttribute), false) as ConverterMethodAttribute[];
                    if ((customAttributes.Length > 0) && (info.ReturnType == typeof(object)))
                    {
                        ParameterInfo[] parameters = info.GetParameters();
                        if (parameters.Length == 2)
                        {
                            if ((parameters[0].ParameterType == typeof(Type)) && (parameters[1].ParameterType == typeof(string)))
                            {
                                m_Converters[info.Name] = Delegate.CreateDelegate(typeof(ConfigConvertHandler), info);
                                if (customAttributes[0].ConvertType != null)
                                {
                                    m_TypeConverters[customAttributes[0].ConvertType] = m_Converters[info.Name];
                                }
                            }
                            else if ((parameters[0].ParameterType == typeof(Hashtable)) && (parameters[1].ParameterType == typeof(ConfigCollectionItemAttribute)))
                            {
                                m_CollectionConverters[info.Name] = Delegate.CreateDelegate(typeof(ConfigConvertCollectionHandler), info);
                                if (customAttributes[0].ConvertType != null)
                                {
                                    m_CollectionTypeConverters[customAttributes[0].ConvertType] = m_CollectionConverters[info.Name];
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void RegisterConverter(string converterName, ConfigConvertCollectionHandler handler)
        {
            m_CollectionConverters[converterName] = handler;
        }

        public static void RegisterConverter(string converterName, ConfigConvertHandler handler)
        {
            m_Converters[converterName] = handler;
        }

        [ConverterMethod(typeof(string))]
        private static object StringConverter(Type type, string configText)
        {
            return configText;
        }

        [ConverterMethod(typeof(Encoding))]
        private static object TextEncodingConverter(Type type, string configText)
        {
            return Encoding.GetEncoding(configText);
        }

        [ConverterMethod(typeof(Type))]
        private static object TypeConverter(Type type, string configText)
        {
            return TypeUtility.GetTypeFromName(configText);
        }

        [ConverterMethod(typeof(ushort))]
        private static object UInt16Converter(Type type, string configText)
        {
            return ushort.Parse(configText);
        }

        [ConverterMethod(typeof(uint))]
        private static object UInt32Converter(Type type, string configText)
        {
            return uint.Parse(configText);
        }

        [ConverterMethod(typeof(ulong))]
        private static object UInt64Converter(Type type, string configText)
        {
            return ulong.Parse(configText);
        }

        [ConverterMethod(typeof(byte))]
        private static object UInt8Converter(Type type, string configText)
        {
            return byte.Parse(configText);
        }
    }
}
