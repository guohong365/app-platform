namespace Platform.Serialization
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class SerialDataHelper
    {
        private static Hashtable m_SerialTable = new Hashtable();

        public static void Deserial(byte[] input)
        {
            if (input.Length >= 1)
            {
                byte num = input[0];
                SerialDataInfo info = m_SerialTable[num] as SerialDataInfo;
                if (info != null)
                {
                    info.Deserial.Invoke(null, new object[] { input });
                }
            }
        }

        public static void RegisterSerialDataType(Type type)
        {
            SerialDataInfo serialDataInfo = SerialDataInfo.GetSerialDataInfo(type);
            if (serialDataInfo != null)
            {
                m_SerialTable[serialDataInfo.SerialNumber] = serialDataInfo;
            }
        }

        public static byte[] Serial(byte serialNumber)
        {
            SerialDataInfo info = m_SerialTable[serialNumber] as SerialDataInfo;
            if (info == null)
            {
                return null;
            }
            return (info.Serial.Invoke(null, null) as byte[]);
        }

        public static byte[] Serial(Type type)
        {
            SerialDataAttribute[] customAttributes = type.GetCustomAttributes(typeof(SerialDataAttribute), false) as SerialDataAttribute[];
            if (customAttributes.Length < 1)
            {
                return null;
            }
            return Serial(customAttributes[0].SerialNumber);
        }

        private class SerialDataInfo
        {
            private static Type[] BytesTypes = new Type[] { typeof(byte[]) };
            public MethodInfo Deserial;
            private static Type[] EmptyTypes = new Type[0];
            public MethodInfo Serial;
            public byte SerialNumber;

            private SerialDataInfo(byte serialNumber, MethodInfo serial, MethodInfo deserial)
            {
                this.SerialNumber = serialNumber;
                this.Serial = serial;
                this.Deserial = deserial;
            }

            public static SerialDataHelper.SerialDataInfo GetSerialDataInfo(Type type)
            {
                SerialDataAttribute[] customAttributes = type.GetCustomAttributes(typeof(SerialDataAttribute), false) as SerialDataAttribute[];
                if (customAttributes.Length >= 1)
                {
                    MethodInfo method = type.GetMethod("Serial", EmptyTypes);
                    MethodInfo deserial = type.GetMethod("Deserial", BytesTypes);
                    if ((method != null) && (deserial != null))
                    {
                        return new SerialDataHelper.SerialDataInfo(customAttributes[0].SerialNumber, method, deserial);
                    }
                }
                return null;
            }
        }
    }
}
