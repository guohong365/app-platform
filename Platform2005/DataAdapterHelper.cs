using System.Data.OracleClient;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
namespace Platform
{


    internal sealed class DataAdapterHelper
    {
        public Type AdapterType;
        public Type BuilderType;
        public Type CommandType;
        private static readonly DataAdapterHelper[] _dbTypes =
        {
            new DataAdapterHelper(typeof(OdbcDataAdapter), typeof(OdbcCommand), typeof(OdbcCommandBuilder)),
            new DataAdapterHelper(typeof(OleDbDataAdapter), typeof(OleDbCommand), typeof(OleDbCommandBuilder)), 
            new DataAdapterHelper(typeof(OracleDataAdapter), typeof(OracleCommand), typeof(OracleCommandBuilder)),
            new DataAdapterHelper(typeof(SqlDataAdapter), typeof(SqlCommand), typeof(SqlCommandBuilder)) 
        };
        private readonly MethodInfo _getDeleteCommand;
        private readonly MethodInfo _getInsertCommand;
        private readonly MethodInfo _getUpdateCommand;

        public DataAdapterHelper(Type adapterType, Type commandType, Type builderType)
        {
            AdapterType = adapterType;
            CommandType = commandType;
            BuilderType = builderType;
            _getDeleteCommand = BuilderType.GetMethod("GetDeleteCommand");
            _getInsertCommand = BuilderType.GetMethod("GetInsertCommand");
            _getUpdateCommand = BuilderType.GetMethod("GetUpdateCommand");
        }

        public DbDataAdapter GetAdapter()
        {
            IDbDataAdapter adapter = Activator.CreateInstance(AdapterType) as IDbDataAdapter;
            adapter.SelectCommand = GetCommand();
            return (adapter as DbDataAdapter);
        }

        public IDbCommand GetCommand()
        {
            return (Activator.CreateInstance(CommandType) as IDbCommand);
        }

        public IDbCommand GetCommand(object builder, string name)
        {
            if (name == "SELECT")
            {
                return GetCommand();
            }
            if (name == "INSERT")
            {
                return (_getInsertCommand.Invoke(builder, null) as IDbCommand);
            }
            if (name == "DELETE")
            {
                return (_getDeleteCommand.Invoke(builder, null) as IDbCommand);
            }
            if (name == "UPDATE")
            {
                return (_getUpdateCommand.Invoke(builder, null) as IDbCommand);
            }
            return null;
        }

        public object GetCommandBuilder(DbDataAdapter adapter)
        {
            return Activator.CreateInstance(BuilderType, new object[] { adapter });
        }

        public static DataAdapterHelper GetDBTypeHelper(DBType dbType)
        {
            return _dbTypes[(int) dbType];
        }
    }
}
