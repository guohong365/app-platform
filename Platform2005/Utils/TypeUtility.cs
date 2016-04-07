namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.OleDb;
    using System.Reflection;
    using System.Data.Common;

    public sealed class TypeUtility
    {
        public static readonly Type DataRowType = typeof(DataRow);
        public static readonly Type DataSetType = typeof(DataSet);
        public static readonly Type DataTableType = typeof(DataTable);
        public static readonly Type DBAdapterType = typeof(DbDataAdapter);
        private static Hashtable m_OleDataTypeMap = new Hashtable();
        private static Hashtable m_SqlDataTypeMap = new Hashtable();
        private static Hashtable m_TypeMap = new Hashtable();

        static TypeUtility()
        {
            m_TypeMap[typeof(bool).ToString()] = "bool";
            m_TypeMap[typeof(byte).ToString()] = "byte";
            m_TypeMap[typeof(char).ToString()] = "char";
            m_TypeMap[typeof(decimal).ToString()] = "decimal";
            m_TypeMap[typeof(double).ToString()] = "double";
            m_TypeMap[typeof(float).ToString()] = "float";
            m_TypeMap[typeof(int).ToString()] = "int";
            m_TypeMap[typeof(long).ToString()] = "long";
            m_TypeMap[typeof(object).ToString()] = "object";
            m_TypeMap[typeof(sbyte).ToString()] = "sbyte";
            m_TypeMap[typeof(short).ToString()] = "short";
            m_TypeMap[typeof(string).ToString()] = "string";
            m_TypeMap[typeof(ulong).ToString()] = "ulong";
            m_TypeMap[typeof(uint).ToString()] = "uint";
            m_TypeMap[typeof(ushort).ToString()] = "ushort";
            m_TypeMap[typeof(void).ToString()] = "void";
            m_OleDataTypeMap[OleDbType.BigInt] = typeof(long);
            m_OleDataTypeMap[OleDbType.Binary] = typeof(byte[]);
            m_OleDataTypeMap[OleDbType.Boolean] = typeof(bool);
            m_OleDataTypeMap[OleDbType.BSTR] = typeof(string);
            m_OleDataTypeMap[OleDbType.Char] = typeof(string);
            m_OleDataTypeMap[OleDbType.Currency] = typeof(decimal);
            m_OleDataTypeMap[OleDbType.Date] = typeof(DateTime);
            m_OleDataTypeMap[OleDbType.DBDate] = typeof(DateTime);
            m_OleDataTypeMap[OleDbType.DBTime] = typeof(TimeSpan);
            m_OleDataTypeMap[OleDbType.DBTimeStamp] = typeof(DateTime);
            m_OleDataTypeMap[OleDbType.Decimal] = typeof(decimal);
            m_OleDataTypeMap[OleDbType.Double] = typeof(double);
            m_OleDataTypeMap[OleDbType.Empty] = typeof(void);
            m_OleDataTypeMap[OleDbType.Error] = typeof(Exception);
            m_OleDataTypeMap[OleDbType.Filetime] = typeof(DateTime);
            m_OleDataTypeMap[OleDbType.Guid] = typeof(Guid);
            m_OleDataTypeMap[OleDbType.IDispatch] = typeof(object);
            m_OleDataTypeMap[OleDbType.Integer] = typeof(int);
            m_OleDataTypeMap[OleDbType.IUnknown] = typeof(object);
            m_OleDataTypeMap[OleDbType.LongVarBinary] = typeof(byte[]);
            m_OleDataTypeMap[OleDbType.LongVarChar] = typeof(string);
            m_OleDataTypeMap[OleDbType.LongVarWChar] = typeof(string);
            m_OleDataTypeMap[OleDbType.Numeric] = typeof(decimal);
            m_OleDataTypeMap[OleDbType.PropVariant] = typeof(object);
            m_OleDataTypeMap[OleDbType.Single] = typeof(float);
            m_OleDataTypeMap[OleDbType.SmallInt] = typeof(short);
            m_OleDataTypeMap[OleDbType.TinyInt] = typeof(sbyte);
            m_OleDataTypeMap[OleDbType.UnsignedBigInt] = typeof(ulong);
            m_OleDataTypeMap[OleDbType.UnsignedInt] = typeof(uint);
            m_OleDataTypeMap[OleDbType.UnsignedSmallInt] = typeof(ushort);
            m_OleDataTypeMap[OleDbType.UnsignedTinyInt] = typeof(byte);
            m_OleDataTypeMap[OleDbType.VarBinary] = typeof(byte[]);
            m_OleDataTypeMap[OleDbType.VarChar] = typeof(string);
            m_OleDataTypeMap[OleDbType.Variant] = typeof(object);
            m_OleDataTypeMap[OleDbType.VarNumeric] = typeof(decimal);
            m_OleDataTypeMap[OleDbType.VarWChar] = typeof(string);
            m_OleDataTypeMap[OleDbType.WChar] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.BigInt] = typeof(long);
            m_SqlDataTypeMap[SqlDbType.Binary] = typeof(byte[]);
            m_SqlDataTypeMap[SqlDbType.Bit] = typeof(bool);
            m_SqlDataTypeMap[SqlDbType.Char] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.DateTime] = typeof(DateTime);
            m_SqlDataTypeMap[SqlDbType.Decimal] = typeof(decimal);
            m_SqlDataTypeMap[SqlDbType.Float] = typeof(double);
            m_SqlDataTypeMap[SqlDbType.Image] = typeof(byte[]);
            m_SqlDataTypeMap[SqlDbType.Int] = typeof(int);
            m_SqlDataTypeMap[SqlDbType.Money] = typeof(decimal);
            m_SqlDataTypeMap[SqlDbType.NChar] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.NText] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.NVarChar] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.Real] = typeof(float);
            m_SqlDataTypeMap[SqlDbType.SmallDateTime] = typeof(DateTime);
            m_SqlDataTypeMap[SqlDbType.SmallInt] = typeof(short);
            m_SqlDataTypeMap[SqlDbType.SmallMoney] = typeof(decimal);
            m_SqlDataTypeMap[SqlDbType.Text] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.Timestamp] = typeof(byte[]);
            m_SqlDataTypeMap[SqlDbType.TinyInt] = typeof(byte);
            m_SqlDataTypeMap[SqlDbType.UniqueIdentifier] = typeof(Guid);
            m_SqlDataTypeMap[SqlDbType.VarBinary] = typeof(byte[]);
            m_SqlDataTypeMap[SqlDbType.VarChar] = typeof(string);
            m_SqlDataTypeMap[SqlDbType.Variant] = typeof(object);
        }

        public static object CreateInstanceFromName(string typeName)
        {
            Type typeFromName = GetTypeFromName(typeName);
            if (typeFromName == null)
            {
                return null;
            }
            return Activator.CreateInstance(typeFromName);
        }

        public static object CreateInstanceFromName(string typeName, params object[] parameters)
        {
            var typeFromName = GetTypeFromName(typeName);
            if (typeFromName == null)
            {
                return null;
            }
            return Activator.CreateInstance(typeFromName, parameters);
        }

        public static Assembly GetAssembly(string assemblyName)
        {
            try
            {
                return Assembly.LoadWithPartialName(assemblyName);
            }
            catch
            {
                return null;
            }
        }

        public static Type GetOleDataTypeMap(OleDbType dbType)
        {
            return (m_OleDataTypeMap[dbType] as Type);
        }

        public static string GetSimpleDataTypeString(Type type)
        {
            if (type.HasElementType)
            {
                if (type.IsByRef)
                {
                    return GetSimpleDataTypeString(type.GetElementType());
                }
                if (type.IsArray)
                {
                    return (GetSimpleDataTypeString(type.GetElementType()) + "[]");
                }
                return "";
            }
            string text = type.ToString();
            string text2 = m_TypeMap[text] as string;
            if (text2 == null)
            {
                return text;
            }
            return text2;
        }

        public static Type GetTypeFromName(string typeName)
        {
            Type type = null;
            string assemblyName = "";
            if (typeName.IndexOf(";", StringComparison.Ordinal) >= 0)
            {
                var textArray = typeName.Split(new[] { ';' });
                if (textArray.Length != 2)
                {
                    return null;
                }
                assemblyName = textArray[0].Trim();
                typeName = textArray[1].Trim();
            }
            if (assemblyName != "")
            {
                try
                {
                    Assembly assembly = GetAssembly(assemblyName);
                    if (assembly != null)
                    {
                        type = assembly.GetType(typeName);
                        if (type != null)
                        {
                            return type;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly2.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
    }
}
