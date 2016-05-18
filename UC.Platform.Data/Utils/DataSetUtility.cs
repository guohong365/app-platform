using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UC.Platform.Data.Utils
{
    public sealed class DataSetUtility
    {
        private static string _defaultNamespace;
        private static readonly Hashtable _namespaceTable = new Hashtable();

        public static void AddDataSetType(Type type)
        {
            if (type.IsSubclassOf(TypeUtility.DataSetType))
            {
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (info.PropertyType.IsSubclassOf(TypeUtility.DataTableType))
                    {
                        if (type.Namespace != null)
                        {
                            DataSetNamespaceData data = _namespaceTable[type.Namespace] as DataSetNamespaceData;
                            if (data == null)
                            {
                                data = new DataSetNamespaceData(type.Namespace);
                                _namespaceTable[type.Namespace] = data;
                            }
                            data.AddDataSetType(info.Name, type);
                        }
                    }
                }
            }
        }

        public static DataSet CopyDataSet(DataSet ds)
        {
            Type type = ds.GetType();
            try
            {
                DataSet set = Activator.CreateInstance(type) as DataSet;
                if (type.IsSubclassOf(TypeUtility.DataSetType))
                {
                    foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        if (info.PropertyType.IsSubclassOf(TypeUtility.DataTableType))
                        {
                            DataTable table = info.GetValue(ds, null) as DataTable;
                            if (table != null && table.Rows.Count >= 1)
                            {
                                DataTable table2 = info.GetValue(set, null) as DataTable;
                                Type type2 = table.Rows[0].GetType();
                                MethodInfo method = info.PropertyType.GetMethod("New" + info.Name + "Row", new Type[0]);
                                MethodInfo info3 = info.PropertyType.GetMethod("Add" + info.Name + "Row", new[] { type2 });
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    object dest = method.Invoke(table2, null);
                                    ObjectUtility.PropertiesCopy(table.Rows[i], dest, false);
                                    info3.Invoke(table2, new[] { dest });
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < ds.Tables.Count; j++)
                    {
                        if (set == null) continue;
                        DataTable table3 = set.Tables.Add();
                        if (!type.IsSubclassOf(TypeUtility.DataSetType))
                        {
                            for (int m = 0; m < ds.Tables[j].Columns.Count; m++)
                            {
                                table3.Columns.Add(new DataColumn(ds.Tables[j].Columns[m].ColumnName, ds.Tables[j].Columns[m].DataType));
                            }
                        }
                        for (int k = 0; k < ds.Tables[j].Rows.Count; k++)
                        {
                            table3.Rows.Add(ds.Tables[j].Rows[k].ItemArray);
                        }
                    }
                }
                return set;
            }
            catch (Exception)
            {
                //ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static DataSet GetDataSetByName(string name)
        {
            return GetDataSetByName(_defaultNamespace, name);
        }

        public static DataSet GetDataSetByName(string strNamespace, string name)
        {
            if ((strNamespace == null) || (name == null))
            {
                return null;
            }
            DataSetNamespaceData data = _namespaceTable[strNamespace] as DataSetNamespaceData;
            if (data == null)
            {
                return null;
            }
            Type dataSetType = data.GetDataSetType(name);
            if (dataSetType == null)
            {
                return null;
            }
            return (Activator.CreateInstance(dataSetType) as DataSet);
        }

        public static byte[] GetDataSetChangesXml(DataSet[] sourceDataSets)
        {
            return GetDataSetXml(sourceDataSets, true);
        }

        public static string GetDataSetChangesXml(DataSet sourceDataSet)
        {
            return GetDataSetXml(sourceDataSet, true);
        }

        public static string GetDataSetXml(DataSet sourceDataSet)
        {
            try
            {
                StringWriter writer = new StringWriter();
                if (sourceDataSet.GetType() == TypeUtility.DataSetType)
                {
                    sourceDataSet.WriteXml(writer);
                }
                else
                {
                    sourceDataSet.WriteXml(writer, XmlWriteMode.DiffGram);
                }
                return writer.ToString();
            }
            catch (Exception)
            {
                //ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static byte[] GetDataSetXml(DataSet[] sourceDataSets)
        {
            try
            {
                string[] input = new string[sourceDataSets.Length];
                for (int i = 0; i < sourceDataSets.Length; i++)
                {
                    string dataSetXml = GetDataSetXml(sourceDataSets[i]);
                    if (dataSetXml == null)
                    {
                        return null;
                    }
                    input[i] = dataSetXml;
                }
                return ByteUtility.StringArrayToByteArray(input);
            }
            catch (Exception)
            {
               // ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static byte[] GetDataSetXml(DataSet[] sourceDataSets, bool changeOnly)
        {
            try
            {
                string[] input = new string[sourceDataSets.Length];
                for (int i = 0; i < sourceDataSets.Length; i++)
                {
                    string dataSetXml = GetDataSetXml(sourceDataSets[i], changeOnly);
                    if (dataSetXml == null)
                    {
                        return null;
                    }
                    input[i] = dataSetXml;
                }
                return ByteUtility.StringArrayToByteArray(input);
            }
            catch (Exception)
            {
                //ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static string GetDataSetXml(DataSet sourceDataSet, bool changeOnly)
        {
            try
            {
                StringWriter writer = new StringWriter();
                if (changeOnly)
                {
                    DataSet changes = sourceDataSet.GetChanges();
                    if (sourceDataSet.GetType() == TypeUtility.DataSetType)
                    {
                        if (changes != null) changes.WriteXml(writer);
                    }
                    else
                    {
                        if (changes != null) changes.WriteXml(writer, XmlWriteMode.DiffGram);
                    }
                }
                else if (sourceDataSet.GetType() == TypeUtility.DataSetType)
                {
                    sourceDataSet.WriteXml(writer);
                }
                else
                {
                    sourceDataSet.WriteXml(writer, XmlWriteMode.DiffGram);
                }
                return writer.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool GetRowFieldValue(DataRow row, string fieldName, object value)
        {
            return GetRowFieldValue(row, fieldName, ref value, null);
        }

        public static bool GetRowFieldValue(DataRow row, string fieldName, ref object value, object valueForNull)
        {
            if (!row.Table.Columns.Contains(fieldName))
            {
                return false;
            }
            if (row.IsNull(fieldName))
            {
                value = valueForNull;
                return true;
            }
            value = row[fieldName];
            return true;
        }

        public static bool HasDataSet(string name)
        {
            return HasDataSet(_defaultNamespace, name);
        }

        public static bool HasDataSet(string strNamespace, string name)
        {
            if ((strNamespace == null) || (name == null))
            {
                return false;
            }
            DataSetNamespaceData data = _namespaceTable[strNamespace] as DataSetNamespaceData;
            if (data == null)
            {
                return false;
            }
            return data.HasDataSetType(name);
        }

        public static bool InitializeDataSetTable(Assembly asm)
        {
            if (asm == null)
            {
                return false;
            }
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    AddDataSetType(type);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool InitializeDataSetTable(Assembly asm, string nameSpace)
        {
            if (asm == null)
            {
                return false;
            }
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(TypeUtility.DataSetType) && (type.Namespace == nameSpace))
                    {
                        AddDataSetType(type);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void RegisterDefaultDataSetNamespace(string name)
        {
            _defaultNamespace = name;
        }

        public static DataSet SearchDataSetByName(string name)
        {
            return (from DataSetNamespaceData data in _namespaceTable.Values select data.GetDataSetType(name) into dataSetType where dataSetType != null select (Activator.CreateInstance(dataSetType) as DataSet)).FirstOrDefault();
        }

        public static bool SetDataSetXml(byte[] xmls, DataSet[] desertDataSets)
        {
            try
            {
                string[] textArray = ByteUtility.ByteArrayToStringArray(xmls);
                if (textArray == null)
                {
                    return false;
                }
                if (textArray.Length != desertDataSets.Length)
                {
                    return false;
                }
                return !textArray.Where((t, i) => !SetDataSetXml(t, desertDataSets[i])).Any();
            }
            catch (Exception)
            {
                //ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public static bool SetDataSetXml(string strXml, DataSet desertDataSet)
        {
            try
            {
                StringReader reader = new StringReader(strXml);
                desertDataSet.Clear();
                if (desertDataSet.GetType() == TypeUtility.DataSetType)
                {
                    desertDataSet.ReadXml(reader);
                }
                else
                {
                    desertDataSet.ReadXml(reader, XmlReadMode.DiffGram);
                }
                reader.Close();
                return true;
            }
            catch (Exception)
            {
                //ExceptionHelper.HandleException(exception);
                return false;
            }
        }

        public static bool SetRowFieldValue(DataRow row, string fieldName, object value)
        {
            return SetRowFieldValue(row, fieldName, value, null, null);
        }

        public static bool SetRowFieldValue(DataRow row, string fieldName, object value, object[] nullValues)
        {
            return SetRowFieldValue(row, fieldName, value, nullValues, null);
        }

        public static bool SetRowFieldValue(DataRow row, string fieldName, object value, object[] nullValues, object valueForNull)
        {
            if (!row.Table.Columns.Contains(fieldName))
            {
                return false;
            }
            object obj2 = valueForNull ?? DBNull.Value;
            if (value == null)
            {
                row[fieldName] = obj2;
                return true;
            }
            if (nullValues != null)
            {
                if (nullValues.Contains(value))
                {
                    row[fieldName] = obj2;
                    return true;
                }
            }
            row[fieldName] = value;
            return true;
        }

        public static void XCopyObj(object srcObj, object desObj)
        {
            PropertyInfo[] properties = srcObj.GetType().GetProperties();
            PropertyInfo[] infoArray2 = desObj.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                foreach (PropertyInfo info2 in infoArray2)
                {
                    if ((info.Name == info2.Name) && info.CanWrite)
                    {
                        info2.SetValue(desObj, info.GetValue(srcObj, null), null);
                        break;
                    }
                }
            }
        }

        internal class DataSetNamespaceData
        {
            public Hashtable DataSetTypeTable = new Hashtable();
            public string Namespace;

            public DataSetNamespaceData(string name)
            {
                Namespace = name;
            }

            public void AddDataSetType(string name, Type type)
            {
                DataSetTypeTable[name.ToUpper()] = type;
            }

            public Type GetDataSetType(string name)
            {
                return (DataSetTypeTable[name.ToUpper()] as Type);
            }

            public bool HasDataSetType(string name)
            {
                return DataSetTypeTable.ContainsKey(name.ToUpper());
            }
        }
    }
}
