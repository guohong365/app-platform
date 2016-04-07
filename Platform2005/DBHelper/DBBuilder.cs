using System;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Platform.DBHelper
{


    internal sealed class DBBuilder
    {
        private static readonly Hashtable _connectionTypeMap = new Hashtable();
        private static readonly DBTypeInfo[] _dbTypeInfos =
        {
            new DBTypeInfo(typeof(OdbcDataAdapter), typeof(OdbcCommand), typeof(OdbcCommandBuilder), typeof(OdbcParameter)),
            new DBTypeInfo(typeof(OleDbDataAdapter), typeof(OleDbCommand), typeof(OleDbCommandBuilder), typeof(OleDbParameter)),
            new DBTypeInfo(typeof(OracleDataAdapter), typeof(OracleCommand), typeof(OracleCommandBuilder), typeof(OracleParameter)),
            new DBTypeInfo(typeof(SqlDataAdapter), typeof(SqlCommand), typeof(SqlCommandBuilder), typeof(SqlParameter)),
            new DBTypeInfo(typeof(MySqlDataAdapter), typeof(MySqlCommand), typeof(MySqlCommandBuilder), typeof(MySqlParameter)),
            new DBTypeInfo(typeof(SQLiteDataAdapter), typeof(SQLiteCommand), typeof(SQLiteCommandBuilder), typeof(SQLiteParameter)) 
        };

        static DBBuilder()
        {
            _connectionTypeMap[typeof(OdbcConnection)] = DBType.ODBC;
            _connectionTypeMap[typeof(OleDbConnection)] = DBType.OLEDB;
            _connectionTypeMap[typeof(OracleConnection)] = DBType.ORACLE;
            _connectionTypeMap[typeof(SqlConnection)] = DBType.SQLSERVER;
            _connectionTypeMap[typeof (MySqlConnection)] = DBType.MYSQL;
            _connectionTypeMap[typeof (SQLiteConnection)] = DBType.SQLITE;
        }

        public static IDbDataAdapter CreateAdapter(IDbConnection connection)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbDataAdapter adapter = Activator.CreateInstance(info.AdapterType) as IDbDataAdapter;
            if (adapter == null)
            {
                throw new NullReferenceException(string.Format("创建DBAdapter失败[{0}]", info.AdapterType.Name));
            }
            adapter.SelectCommand = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if (adapter.SelectCommand == null)
            {
                throw new NullReferenceException("创建SelectCommand失败！");
            }
            adapter.SelectCommand.Connection = connection;
            if ((connection.GetType() != typeof(OracleConnection)) && !connection.GetType().IsSubclassOf(typeof(OracleConnection)))
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            }
            Activator.CreateInstance(info.BuilderType, new object[] { adapter });
            return adapter;
        }

        public static IDbDataAdapter CreateAdapter(IDbConnection connection, string tableName)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbDataAdapter adapter = Activator.CreateInstance(info.AdapterType) as IDbDataAdapter;
            if (adapter == null) throw new DataException();
            adapter.SelectCommand = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if(adapter.SelectCommand==null) throw new DataException();
            adapter.SelectCommand.Connection = connection;
            adapter.SelectCommand.CommandText = "SELECT * FROM " + tableName;
            if ((connection.GetType() != typeof(OracleConnection)) && !connection.GetType().IsSubclassOf(typeof(OracleConnection)))
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            }
            object obj2 = Activator.CreateInstance(info.BuilderType, new object[] { adapter });
            try
            {
                adapter.InsertCommand = info.GetInsertCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.InsertCommand = null;
            }
            try
            {
                adapter.UpdateCommand = info.GetUpdateCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.UpdateCommand = null;
            }
            try
            {
                adapter.DeleteCommand = info.GetDeleteCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.DeleteCommand = null;
            }
            return adapter;
        }

        public static IDbDataAdapter CreateAdapter(IDbConnection connection, string tableName, IDbTransaction transaction)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbDataAdapter adapter = Activator.CreateInstance(info.AdapterType) as IDbDataAdapter;
            if (adapter == null) throw new DataException();

            adapter.SelectCommand = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if (adapter.SelectCommand == null) throw new DataException();
            
                adapter.SelectCommand.Connection = connection;
                adapter.SelectCommand.Transaction = transaction;
                adapter.SelectCommand.CommandText = "SELECT * FROM " + tableName;
            
            if ((connection.GetType() != typeof(OracleConnection)) && !connection.GetType().IsSubclassOf(typeof(OracleConnection)))
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            }
            object obj2 = Activator.CreateInstance(info.BuilderType, new object[] { adapter });
            try
            {
                adapter.InsertCommand = info.GetInsertCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.InsertCommand = null;
            }
            try
            {
                adapter.UpdateCommand = info.GetUpdateCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.UpdateCommand = null;
            }
            try
            {
                adapter.DeleteCommand = info.GetDeleteCommand.Invoke(obj2, new object[] { true }) as IDbCommand;
            }
            catch
            {
                adapter.DeleteCommand = null;
            }
            return adapter;
        }

        public static IDbCommand CreateCommand(IDbConnection connection)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbCommand command = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if (command != null)
            {
                command.Connection = connection;
                return command;
            }
            return null;
        }

        public static IDbCommand CreateCommand(IDbConnection connection, IDbTransaction transaction)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbCommand command = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if (command != null)
            {
                command.Connection = connection;
                command.Transaction = transaction;
                return command;
            }
            return null;
        }

        public static IDbDataParameter CreateParameter(IDbConnection connection)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            return (Activator.CreateInstance(info.ParameterType) as IDbDataParameter);
        }

        public static IDbDataParameter[] CreateParameter(IDbConnection connection, int count)
        {
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbDataParameter[] parameterArray = new IDbDataParameter[count];
            for (int i = 0; i < count; i++)
            {
                parameterArray[i] = Activator.CreateInstance(info.ParameterType) as IDbDataParameter;
            }
            return parameterArray;
        }

        public static bool DeriveParameters(IDbCommand cmd)
        {
            bool flag2;
            if ((cmd == null) || (cmd.Connection == null))
            {
                return false;
            }
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(cmd.Connection)];
            bool flag = false;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                    flag = true;
                }
                info.DerivedParameters.Invoke(null, new object[] { cmd });
                flag2 = true;
            }
            catch (Exception)
            {
                flag2 = false;
            }
            finally
            {
                if (flag)
                {
                    cmd.Connection.Close();
                }
            }
            return flag2;
        }

        public static IDbCommand DeriveParameters(IDbConnection connection, string procName)
        {
            IDbCommand command2;
            DBTypeInfo info = _dbTypeInfos[(int) GetDBType(connection)];
            IDbCommand command = Activator.CreateInstance(info.CommandType) as IDbCommand;
            if (command == null) return null;
            command.Connection = connection;
            command.CommandText = procName;
            command.CommandType = CommandType.StoredProcedure;
            bool flag = false;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    flag = true;
                }
                info.DerivedParameters.Invoke(null, new object[] { command });
                command2 = command;
            }
            catch (Exception)
            {
                command2 = null;
            }
            finally
            {
                if (flag)
                {
                    connection.Close();
                }
            }
            return command2;
        }

        public static DBType GetDBType(IDbConnection connection)
        {
            if (connection != null)
            {
                Type key = connection.GetType();
                if (_connectionTypeMap.ContainsKey(key))
                {
                    return (DBType) _connectionTypeMap[key];
                }
            }
            return DBType.ERROR;
        }

        internal sealed class DBTypeInfo
        {
            public Type AdapterType;
            public Type BuilderType;
            public Type CommandType;
            public MethodInfo DerivedParameters;
            public MethodInfo GetDeleteCommand;
            public MethodInfo GetInsertCommand;
            public MethodInfo GetUpdateCommand;
            public Type ParameterType;

            public DBTypeInfo(Type adapterType, Type commandType, Type builderType, Type paramType)
            {
                AdapterType = adapterType;
                CommandType = commandType;
                BuilderType = builderType;
                ParameterType = paramType;
                GetDeleteCommand = BuilderType.GetMethod("GetDeleteCommand", new[] { typeof(bool) });
                GetInsertCommand = BuilderType.GetMethod("GetInsertCommand", new[] { typeof(bool) });
                GetUpdateCommand = BuilderType.GetMethod("GetUpdateCommand", new[] { typeof(bool) });
                DerivedParameters = BuilderType.GetMethod("DeriveParameters", new[] { CommandType });
            }
        }
    }
}
