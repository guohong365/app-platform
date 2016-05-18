using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Reflection;

namespace UC.Platform.Data.Utils
{
    public sealed class TypeUtility
    {
        public static readonly Type DataRowType = typeof(DataRow);
        public static readonly Type DataSetType = typeof(DataSet);
        public static readonly Type DataTableType = typeof(DataTable);
        public static readonly Type DBAdapterType = typeof(DbDataAdapter);
        private static readonly Hashtable _oleDataTypeMap = new Hashtable();
        private static readonly Hashtable _sqlDataTypeMap = new Hashtable();
        private static readonly Hashtable _typeMap = new Hashtable();

        static TypeUtility()
        {
            _typeMap[typeof(bool).ToString()] = "bool";
            _typeMap[typeof(byte).ToString()] = "byte";
            _typeMap[typeof(char).ToString()] = "char";
            _typeMap[typeof(decimal).ToString()] = "decimal";
            _typeMap[typeof(double).ToString()] = "double";
            _typeMap[typeof(float).ToString()] = "float";
            _typeMap[typeof(int).ToString()] = "int";
            _typeMap[typeof(long).ToString()] = "long";
            _typeMap[typeof(object).ToString()] = "object";
            _typeMap[typeof(sbyte).ToString()] = "sbyte";
            _typeMap[typeof(short).ToString()] = "short";
            _typeMap[typeof(string).ToString()] = "string";
            _typeMap[typeof(ulong).ToString()] = "ulong";
            _typeMap[typeof(uint).ToString()] = "uint";
            _typeMap[typeof(ushort).ToString()] = "ushort";
            _typeMap[typeof(void).ToString()] = "void";
            _oleDataTypeMap[OleDbType.BigInt] = typeof(long);
            _oleDataTypeMap[OleDbType.Binary] = typeof(byte[]);
            _oleDataTypeMap[OleDbType.Boolean] = typeof(bool);
            _oleDataTypeMap[OleDbType.BSTR] = typeof(string);
            _oleDataTypeMap[OleDbType.Char] = typeof(string);
            _oleDataTypeMap[OleDbType.Currency] = typeof(decimal);
            _oleDataTypeMap[OleDbType.Date] = typeof(DateTime);
            _oleDataTypeMap[OleDbType.DBDate] = typeof(DateTime);
            _oleDataTypeMap[OleDbType.DBTime] = typeof(TimeSpan);
            _oleDataTypeMap[OleDbType.DBTimeStamp] = typeof(DateTime);
            _oleDataTypeMap[OleDbType.Decimal] = typeof(decimal);
            _oleDataTypeMap[OleDbType.Double] = typeof(double);
            _oleDataTypeMap[OleDbType.Empty] = typeof(void);
            _oleDataTypeMap[OleDbType.Error] = typeof(Exception);
            _oleDataTypeMap[OleDbType.Filetime] = typeof(DateTime);
            _oleDataTypeMap[OleDbType.Guid] = typeof(Guid);
            _oleDataTypeMap[OleDbType.IDispatch] = typeof(object);
            _oleDataTypeMap[OleDbType.Integer] = typeof(int);
            _oleDataTypeMap[OleDbType.IUnknown] = typeof(object);
            _oleDataTypeMap[OleDbType.LongVarBinary] = typeof(byte[]);
            _oleDataTypeMap[OleDbType.LongVarChar] = typeof(string);
            _oleDataTypeMap[OleDbType.LongVarWChar] = typeof(string);
            _oleDataTypeMap[OleDbType.Numeric] = typeof(decimal);
            _oleDataTypeMap[OleDbType.PropVariant] = typeof(object);
            _oleDataTypeMap[OleDbType.Single] = typeof(float);
            _oleDataTypeMap[OleDbType.SmallInt] = typeof(short);
            _oleDataTypeMap[OleDbType.TinyInt] = typeof(sbyte);
            _oleDataTypeMap[OleDbType.UnsignedBigInt] = typeof(ulong);
            _oleDataTypeMap[OleDbType.UnsignedInt] = typeof(uint);
            _oleDataTypeMap[OleDbType.UnsignedSmallInt] = typeof(ushort);
            _oleDataTypeMap[OleDbType.UnsignedTinyInt] = typeof(byte);
            _oleDataTypeMap[OleDbType.VarBinary] = typeof(byte[]);
            _oleDataTypeMap[OleDbType.VarChar] = typeof(string);
            _oleDataTypeMap[OleDbType.Variant] = typeof(object);
            _oleDataTypeMap[OleDbType.VarNumeric] = typeof(decimal);
            _oleDataTypeMap[OleDbType.VarWChar] = typeof(string);
            _oleDataTypeMap[OleDbType.WChar] = typeof(string);
            _sqlDataTypeMap[SqlDbType.BigInt] = typeof(long);
            _sqlDataTypeMap[SqlDbType.Binary] = typeof(byte[]);
            _sqlDataTypeMap[SqlDbType.Bit] = typeof(bool);
            _sqlDataTypeMap[SqlDbType.Char] = typeof(string);
            _sqlDataTypeMap[SqlDbType.DateTime] = typeof(DateTime);
            _sqlDataTypeMap[SqlDbType.Decimal] = typeof(decimal);
            _sqlDataTypeMap[SqlDbType.Float] = typeof(double);
            _sqlDataTypeMap[SqlDbType.Image] = typeof(byte[]);
            _sqlDataTypeMap[SqlDbType.Int] = typeof(int);
            _sqlDataTypeMap[SqlDbType.Money] = typeof(decimal);
            _sqlDataTypeMap[SqlDbType.NChar] = typeof(string);
            _sqlDataTypeMap[SqlDbType.NText] = typeof(string);
            _sqlDataTypeMap[SqlDbType.NVarChar] = typeof(string);
            _sqlDataTypeMap[SqlDbType.Real] = typeof(float);
            _sqlDataTypeMap[SqlDbType.SmallDateTime] = typeof(DateTime);
            _sqlDataTypeMap[SqlDbType.SmallInt] = typeof(short);
            _sqlDataTypeMap[SqlDbType.SmallMoney] = typeof(decimal);
            _sqlDataTypeMap[SqlDbType.Text] = typeof(string);
            _sqlDataTypeMap[SqlDbType.Timestamp] = typeof(byte[]);
            _sqlDataTypeMap[SqlDbType.TinyInt] = typeof(byte);
            _sqlDataTypeMap[SqlDbType.UniqueIdentifier] = typeof(Guid);
            _sqlDataTypeMap[SqlDbType.VarBinary] = typeof(byte[]);
            _sqlDataTypeMap[SqlDbType.VarChar] = typeof(string);
            _sqlDataTypeMap[SqlDbType.Variant] = typeof(object);
        }

        public static Hashtable SQLDataTypeMap
        {
            get { return _sqlDataTypeMap; }
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

        public static Assembly GetAssembly(string assemblyFullName)
        {
            try
            {
                return Assembly.Load(assemblyFullName);
            }
            catch
            {
                return null;
            }
        }

        public static Type GetOleDataTypeMap(OleDbType dbType)
        {
            return (_oleDataTypeMap[dbType] as Type);
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
            string text2 = _typeMap[text] as string;
            if (text2 == null)
            {
                return text;
            }
            return text2;
        }

        public static Type GetTypeFromName(string typeName)
        {
            Type type;
            string assemblyName = "";
            if (typeName.IndexOf(";", StringComparison.Ordinal) >= 0)
            {
                var textArray = typeName.Split(';');
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
