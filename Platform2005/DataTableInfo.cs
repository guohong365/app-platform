namespace Platform
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.Data.OracleClient;
    using System.Data.SqlClient;

    public sealed class DataTableInfo
    {
        private DataAdapterHelper AdapterHelper;
        private DbDataAdapter m_Adapter;
        private string m_CreateTableSql;
        private static Type[] m_DBMapTypes = new Type[] { typeof(long), typeof(bool), typeof(int), typeof(double), typeof(short), typeof(sbyte), typeof(DateTime), typeof(string), typeof(byte[]) };
        private static DBTypeData[][] m_DBTypeDatas = new DBTypeData[][] { new DBTypeData[] { new DBTypeData(OdbcType.BigInt, "[bigint]", 8), new DBTypeData(OdbcType.Bit, "[bit]", 1), new DBTypeData(OdbcType.Int, "[int]", 4), new DBTypeData(OdbcType.Double, "[float]", 8), new DBTypeData(OdbcType.SmallInt, "[smallint]", 2), new DBTypeData(OdbcType.TinyInt, "[tinyint]", 1), new DBTypeData(OdbcType.DateTime, "[datetime]", 8), new DBTypeData(OdbcType.NVarChar, "[nvarchar](4000)", 0xfa0), new DBTypeData(OdbcType.Image, "[image]", 0) }, new DBTypeData[] { new DBTypeData(OleDbType.BigInt, "[bigint]", 8), new DBTypeData(OleDbType.Boolean, "[bit]", 1), new DBTypeData(OleDbType.Integer, "[int]", 4), new DBTypeData(OleDbType.Double, "[float]", 8), new DBTypeData(OleDbType.SmallInt, "[smallint]", 2), new DBTypeData(OleDbType.TinyInt, "[tinyint]", 1), new DBTypeData(OleDbType.Date, "[datetime]", 8), new DBTypeData(OleDbType.VarWChar, "[nvarchar](4000)", 0xfa0), new DBTypeData(OleDbType.VarBinary, "[image]", 0) }, new DBTypeData[] { new DBTypeData(OracleType.Int32, "[bigint]", 8), new DBTypeData(OracleType.Byte, "[bit]", 1), new DBTypeData(OracleType.Int32, "[int]", 4), new DBTypeData(OracleType.Float, "[float]", 8), new DBTypeData(OracleType.Int16, "[smallint]", 2), new DBTypeData(OracleType.Byte, "[tinyint]", 1), new DBTypeData(OracleType.DateTime, "[datetime]", 8), new DBTypeData(OracleType.NVarChar, "[nvarchar](4000)", 0xfa0), new DBTypeData(OracleType.LongRaw, "[image]", 0) }, new DBTypeData[] { new DBTypeData(SqlDbType.BigInt, "[bigint]", 8), new DBTypeData(SqlDbType.Bit, "[bit]", 1), new DBTypeData(SqlDbType.Int, "[int]", 4), new DBTypeData(SqlDbType.Float, "[float]", 8), new DBTypeData(SqlDbType.SmallInt, "[smallint]", 2), new DBTypeData(SqlDbType.TinyInt, "[tinyint]", 1), new DBTypeData(SqlDbType.DateTime, "[datetime]", 8), new DBTypeData(SqlDbType.NVarChar, "[nvarchar](4000)", 0xfa0), new DBTypeData(SqlDbType.Image, "[image]", 0) } };
        private string m_DropTableSql;
        private string m_TableName;

        private DataTableInfo(string tableName, string createSql, string dropSql, DbDataAdapter adapter, DataAdapterHelper dbAdaperHelper)
        {
            this.m_TableName = tableName;
            this.m_CreateTableSql = createSql;
            this.m_DropTableSql = dropSql;
            this.m_Adapter = adapter;
            this.AdapterHelper = dbAdaperHelper;
        }

        public static DataTableInfo GetDataTableInfo(DataSet ds, DBType dbType)
        {
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables.Count < 1)
            {
                return null;
            }
            return GetDataTableInfo(ds.Tables[0], ds.Tables[0].TableName, dbType);
        }

        public static DataTableInfo GetDataTableInfo(DataTable dt, DBType dbType)
        {
            if (dt == null)
            {
                return null;
            }
            if (dt.DataSet == null)
            {
                return null;
            }
            return GetDataTableInfo(dt, dt.TableName, dbType);
        }

        public static DataTableInfo[] GetDataTableInfo(Type dataType, DBType dbType)
        {
            int num = (int) dbType;
            if ((num < 0) || (num > 3))
            {
                return null;
            }
            if (dataType.IsSubclassOf(TypeUtility.DataSetType))
            {
                Type[] nestedTypes = dataType.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                ArrayList list = new ArrayList();
                foreach (Type type in nestedTypes)
                {
                    if (type.IsSubclassOf(TypeUtility.DataRowType))
                    {
                        DataTableInfo[] dataTableInfo = GetDataTableInfo(type, dbType);
                        if (dataTableInfo != null)
                        {
                            list.Add(dataTableInfo[0]);
                        }
                    }
                }
                if (list.Count >= 1)
                {
                    return (list.ToArray(typeof(DataTableInfo)) as DataTableInfo[]);
                }
                return null;
            }
            if (dataType.IsSubclassOf(TypeUtility.DataTableType))
            {
                MethodInfo method = dataType.GetMethod("get_Item");
                if (method == null)
                {
                    return null;
                }
                return GetDataTableInfo(method.ReturnType, dbType);
            }
            if (!dataType.IsSubclassOf(TypeUtility.DataRowType))
            {
                return null;
            }
            string dataSetTable = dataType.Name.Substring(0, dataType.Name.Length - 3);
            DataAdapterHelper dBTypeHelper = DataAdapterHelper.GetDBTypeHelper(dbType);
            DbDataAdapter adapter = dBTypeHelper.GetAdapter();
            DataTableMapping mapping = new DataTableMapping("Table", dataSetTable);
            ((IDbDataAdapter) adapter).SelectCommand.CommandText = "SELECT * FROM " + dataSetTable;
            string dropSql = "IF EXISTS (SELECT * FROM SYSOBJECTS WHERE ID=OBJECT_ID(N'[" + dataSetTable + "]') AND OBJECTPROPERTY(ID, N'IsUserTable') = 1) DROP TABLE [" + dataSetTable + "]";
            string createSql = "CREATE TABLE [" + dataSetTable + "] (";
            PropertyInfo[] properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < properties.Length; i++)
            {
                DBTypeData dBTypeData = GetDBTypeData(dbType, properties[i].PropertyType);
                mapping.ColumnMappings.Add(properties[i].Name, properties[i].Name);
                string text4 = createSql;
                createSql = text4 + "[" + properties[i].Name + "]" + dBTypeData.Sql;
                if (i < (properties.Length - 1))
                {
                    createSql = createSql + ",";
                }
            }
            createSql = createSql + ")";
            adapter.TableMappings.Add(mapping);
            DataTableInfo info2 = new DataTableInfo(dataSetTable, createSql, dropSql, adapter, dBTypeHelper);
            return new DataTableInfo[] { info2 };
        }

        public static DataTableInfo GetDataTableInfo(DataSet ds, int tableIndex, DBType dbType)
        {
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables.Count <= tableIndex)
            {
                return null;
            }
            return GetDataTableInfo(ds.Tables[tableIndex], ds.Tables[tableIndex].TableName, dbType);
        }

        public static DataTableInfo GetDataTableInfo(DataSet ds, string tableName, DBType dbType)
        {
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables.Count < 1)
            {
                return null;
            }
            return GetDataTableInfo(ds.Tables[0], tableName, dbType);
        }

        public static DataTableInfo GetDataTableInfo(DataTable dt, string tableName, DBType dbType)
        {
            int num = (int) dbType;
            if ((num < 0) || (num > 3))
            {
                return null;
            }
            DataAdapterHelper dBTypeHelper = DataAdapterHelper.GetDBTypeHelper(dbType);
            DbDataAdapter adapter = dBTypeHelper.GetAdapter();
            DataTableMapping mapping = new DataTableMapping("Table", tableName);
            ((IDbDataAdapter) adapter).SelectCommand.CommandText = "SELECT * FROM " + tableName;
            string dropSql = "IF EXISTS (SELECT * FROM SYSOBJECTS WHERE ID=OBJECT_ID(N'[" + tableName + "]') AND OBJECTPROPERTY(ID, N'IsUserTable') = 1) DROP TABLE [" + tableName + "]";
            string createSql = "CREATE TABLE [" + tableName + "] (";
            DataColumnCollection columns = dt.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                DBTypeData dBTypeData = GetDBTypeData(dbType, columns[i].DataType);
                mapping.ColumnMappings.Add(columns[i].ColumnName, columns[i].ColumnName);
                string text3 = createSql;
                createSql = text3 + "[" + columns[i].ColumnName + "]" + dBTypeData.Sql;
                if (i < (columns.Count - 1))
                {
                    createSql = createSql + ",";
                }
            }
            createSql = createSql + ")";
            adapter.TableMappings.Add(mapping);
            return new DataTableInfo(tableName, createSql, dropSql, adapter, dBTypeHelper);
        }

        public static DataTableInfo GetDataTableInfo(DataSet ds, int tableIndex, string tableName, DBType dbType)
        {
            if (ds == null)
            {
                return null;
            }
            if (ds.Tables.Count <= tableIndex)
            {
                return null;
            }
            return GetDataTableInfo(ds.Tables[tableIndex], tableName, dbType);
        }

        private static DBTypeData GetDBTypeData(DBType dbType, Type type)
        {
            int index = (int) dbType;
            if ((index >= 0) && (index < m_DBTypeDatas.Length))
            {
                for (int i = 0; i < m_DBMapTypes.Length; i++)
                {
                    if (m_DBMapTypes[i] == type)
                    {
                        return m_DBTypeDatas[index][i];
                    }
                }
            }
            return null;
        }

        public void SetConnection(IDbConnection connection)
        {
            this.SetConnection(connection, null);
        }

        public void SetConnection(IDbConnection connection, IDbTransaction transaction)
        {
            IDbDataAdapter adapter = this.m_Adapter;
            if ((adapter != null) && (adapter.SelectCommand != null))
            {
                adapter.SelectCommand.Connection = connection;
                adapter.SelectCommand.Transaction = transaction;
                object builder = null;
                try
                {
                    if (adapter.InsertCommand == null)
                    {
                        if (builder == null)
                        {
                            builder = this.AdapterHelper.GetCommandBuilder(this.m_Adapter);
                        }
                        adapter.InsertCommand = this.AdapterHelper.GetCommand(builder, "INSERT");
                    }
                }
                catch
                {
                }
                try
                {
                    if (adapter.DeleteCommand == null)
                    {
                        if (builder == null)
                        {
                            builder = this.AdapterHelper.GetCommandBuilder(this.m_Adapter);
                        }
                        adapter.DeleteCommand = this.AdapterHelper.GetCommand(builder, "DELETE");
                    }
                }
                catch
                {
                }
                try
                {
                    if (adapter.UpdateCommand == null)
                    {
                        if (builder == null)
                        {
                            builder = this.AdapterHelper.GetCommandBuilder(this.m_Adapter);
                        }
                        adapter.UpdateCommand = this.AdapterHelper.GetCommand(builder, "UPDATE");
                    }
                }
                catch
                {
                }
                adapter.SelectCommand.Transaction = null;
            }
        }

        public DbDataAdapter Adapter
        {
            get
            {
                return this.m_Adapter;
            }
        }

        public string CreateTableSql
        {
            get
            {
                return this.m_CreateTableSql;
            }
        }

        public string DropTableSql
        {
            get
            {
                return this.m_DropTableSql;
            }
        }

        public string TableName
        {
            get
            {
                return this.m_TableName;
            }
        }

        private sealed class DBTypeData
        {
            public Enum DataType;
            public int Size;
            public string Sql;

            public DBTypeData(Enum dataType, string sql, int size)
            {
                this.DataType = dataType;
                this.Sql = sql;
                this.Size = size;
            }
        }
    }
}
