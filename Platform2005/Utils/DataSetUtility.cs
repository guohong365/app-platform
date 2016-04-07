namespace Platform.Utils
{
    using Platform.ExceptionHandling;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Reflection;

    public sealed class DataSetUtility
    {
        private static string m_DefaultNamespace;
        private static Hashtable NamespaceTable = new Hashtable();

        public static void AddDataSetType(Type type)
        {
            if (type.IsSubclassOf(TypeUtility.DataSetType))
            {
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (info.PropertyType.IsSubclassOf(TypeUtility.DataTableType))
                    {
                        DataSetNamespaceData data = NamespaceTable[type.Namespace] as DataSetNamespaceData;
                        if (data == null)
                        {
                            data = new DataSetNamespaceData(type.Namespace);
                            NamespaceTable[type.Namespace] = data;
                        }
                        data.AddDataSetType(info.Name, type);
                    }
                }
            }
        }

        public static bool ChangeDBColumnDataType(DataTable table, DataColumn column, Type newDataType)
        {
            FieldInfo field = typeof(DataColumn).GetField("_storage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            object obj2 = field.GetValue(column);
            if (obj2 == null)
            {
                column.DataType = newDataType;
                return true;
            }
            field.SetValue(column, null);
            MethodInfo method = TypeUtility.GetTypeFromName("System.Data;System.Data.Common.DataStorage").GetMethod("CreateStorage", BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            column.DataType = newDataType;
            object obj3 = method.Invoke(null, new object[] { column, newDataType });
            field.SetValue(column, obj3);
            typeof(DataColumn).GetField("maxLength", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).SetValue(column, -1);
            object[] objArray = obj2.GetType().GetField("values", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetValue(obj2) as object[];
            if (objArray != null)
            {
                object[] objArray2 = new object[objArray.Length];
                for (int i = 0; i < objArray2.Length; i++)
                {
                    objArray2[i] = DBNull.Value;
                }
                obj3.GetType().GetField("values", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).SetValue(obj3, objArray2);
            }
            table.AcceptChanges();
            return true;
        }

        public static bool ChangeDBColumnDataType(DataTable table, string column, Type newDataType)
        {
            if (!table.Columns.Contains(column))
            {
                return false;
            }
            return ChangeDBColumnDataType(table, table.Columns[column], newDataType);
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
                            if (table.Rows.Count >= 1)
                            {
                                DataTable table2 = info.GetValue(set, null) as DataTable;
                                Type type2 = table.Rows[0].GetType();
                                MethodInfo method = info.PropertyType.GetMethod("New" + info.Name + "Row", new Type[0]);
                                MethodInfo info3 = info.PropertyType.GetMethod("Add" + info.Name + "Row", new Type[] { type2 });
                                type2.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    object dest = method.Invoke(table2, null);
                                    ObjectUtility.PropertiesCopy(table.Rows[i], dest, false);
                                    info3.Invoke(table2, new object[] { dest });
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < ds.Tables.Count; j++)
                    {
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static bool CopyDataTable(DataTable srcTable, DataTable destTable)
        {
            try
            {
                if (!destTable.GetType().IsSubclassOf(TypeUtility.DataTableType))
                {
                    foreach (DataColumn column in srcTable.Columns)
                    {
                        if (!destTable.Columns.Contains(column.ColumnName))
                        {
                            destTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType, column.Expression, column.ColumnMapping));
                        }
                    }
                }
                foreach (DataRow row in srcTable.Rows)
                {
                    DataRow row2 = destTable.NewRow();
                    foreach (DataColumn column2 in srcTable.Columns)
                    {
                        if (destTable.Columns.Contains(column2.ColumnName))
                        {
                            continue;
                        }
                        if (row.IsNull(column2))
                        {
                            row2[column2.ColumnName] = DBNull.Value;
                            continue;
                        }
                        row2[column2.ColumnName] = row[column2];
                    }
                    destTable.Rows.Add(row2);
                }
                destTable.AcceptChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DataSet GetDataSetByName(string name)
        {
            return GetDataSetByName(m_DefaultNamespace, name);
        }

        public static DataSet GetDataSetByName(string strNamespace, string name)
        {
            if ((strNamespace == null) || (name == null))
            {
                return null;
            }
            DataSetNamespaceData data = NamespaceTable[strNamespace] as DataSetNamespaceData;
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

        public static string GetDataSetChangesXml(DataSet sourceDataSet)
        {
            return GetDataSetXml(sourceDataSet, true);
        }

        public static byte[] GetDataSetChangesXml(DataSet[] sourceDataSets)
        {
            return GetDataSetXml(sourceDataSets, true);
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
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
                        changes.WriteXml(writer);
                    }
                    else
                    {
                        changes.WriteXml(writer, XmlWriteMode.DiffGram);
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                return null;
            }
        }

        public static bool GetRowFieldValue(DataRow row, string fieldName, ref object value)
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
            return HasDataSet(m_DefaultNamespace, name);
        }

        public static bool HasDataSet(string strNamespace, string name)
        {
            if ((strNamespace == null) || (name == null))
            {
                return false;
            }
            DataSetNamespaceData data = NamespaceTable[strNamespace] as DataSetNamespaceData;
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

        public static bool InitializeDataSetTable(Assembly asm, string NameSpace)
        {
            if (asm == null)
            {
                return false;
            }
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(TypeUtility.DataSetType) && (type.Namespace == NameSpace))
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
            m_DefaultNamespace = name;
        }

        public static DataSet SearchDataSetByName(string name)
        {
            foreach (DataSetNamespaceData data in NamespaceTable.Values)
            {
                Type dataSetType = data.GetDataSetType(name);
                if (dataSetType != null)
                {
                    return (Activator.CreateInstance(dataSetType) as DataSet);
                }
            }
            return null;
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
                for (int i = 0; i < textArray.Length; i++)
                {
                    if (!SetDataSetXml(textArray[i], desertDataSets[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
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
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
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
            object obj2 = (valueForNull == null) ? DBNull.Value : valueForNull;
            if (value == null)
            {
                row[fieldName] = obj2;
                return true;
            }
            if (nullValues != null)
            {
                for (int i = 0; i < nullValues.Length; i++)
                {
                    if (value.Equals(nullValues[i]))
                    {
                        row[fieldName] = obj2;
                        return true;
                    }
                }
            }
            row[fieldName] = value;
            return true;
        }

        internal class DataSetNamespaceData
        {
            public Hashtable DataSetTypeTable = new Hashtable();
            public string Namespace;

            public DataSetNamespaceData(string name)
            {
                this.Namespace = name;
            }

            public void AddDataSetType(string name, Type type)
            {
                this.DataSetTypeTable[name.ToUpper()] = type;
            }

            public Type GetDataSetType(string name)
            {
                return (this.DataSetTypeTable[name.ToUpper()] as Type);
            }

            public bool HasDataSetType(string name)
            {
                return this.DataSetTypeTable.ContainsKey(name.ToUpper());
            }
        }
    }
}
