using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace UC.Platform.Data
{
  public class DatabaseFactory : IDatabaseProviderFactory
  {
    private readonly DbProviderFactory _factory;
    private readonly string _connectionString;
    private readonly string _providerName;

    private readonly List<DbConnection> _connectionPool = new List<DbConnection>();

    public DatabaseFactory(ConnectionStringSettings connectionString)
      : this(connectionString.ProviderName, connectionString.ConnectionString)
    {
    }

    public DatabaseFactory(string providerName, string connectionString)
    {
      _providerName = providerName;
      _connectionString = connectionString;
      _factory = DbProviderFactories.GetFactory(providerName);
    }

    public DbCommand CreateCommand(DbTransaction transaction)
    {
      DbCommand command = _factory.CreateCommand();
      if (command == null) return null;
      command.Connection = transaction == null ? CreateConnection() : transaction.Connection;
      command.Transaction = transaction;
      return command;
    }

    public DbCommand CreateCommand(DbConnection connection)
    {
      DbCommand command = _factory.CreateCommand();
      if (command == null) return null;
      command.Connection = connection ?? CreateConnection();
      return command;
    }

    public DbCommandBuilder CreateCommandBuilder()
    {
      DbCommandBuilder builder = _factory.CreateCommandBuilder();
      return builder;
    }

    public DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter)
    {
      DbCommandBuilder builder = _factory.CreateCommandBuilder();
      if (builder != null)
      {
        builder.DataAdapter = adapter;
      }
      return builder;
    }

    public DbConnection CreateConnection()
    {
      DbConnection connection =
        _connectionPool.FirstOrDefault(
          conn => conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
      if (connection != null)
      {
        return connection;
      }
      connection = _factory.CreateConnection();
      if (connection != null)
      {
        connection.ConnectionString = ConnectionString;
      }
      return connection;
    }

    public DbConnectionStringBuilder CreateConnectionStringBuilder()
    {
      return _factory.CreateConnectionStringBuilder();
    }

    public DbDataAdapter CreateDataAdapter()
    {
      return CreateDataAdapter((DbConnection) null);
    }

    public DbDataAdapter CreateDataAdapter(DbConnection connection)
    {
      DbDataAdapter adapter = _factory.CreateDataAdapter();
      if (adapter == null) return null;
      adapter.AcceptChangesDuringUpdate = true;
      adapter.ContinueUpdateOnError = false;
      adapter.MissingSchemaAction = MissingSchemaAction.Add;
      adapter.MissingMappingAction = MissingMappingAction.Passthrough;
      adapter.SelectCommand = CreateCommand(connection);
      return adapter;
    }

    public DbDataAdapter CreateDataAdapter(string tableName, DbTransaction transaction)
    {
      DbDataAdapter adapter = CreateDataAdapter(transaction == null ? null : transaction.Connection);
      if (adapter == null) return null;
      adapter.SelectCommand.CommandText = string.Format("select * from {0}", tableName);
      adapter.SelectCommand.CommandType = CommandType.Text;
      DbCommandBuilder builder = CreateCommandBuilder(adapter);
      if (builder == null) return adapter;
      adapter.InsertCommand = builder.GetInsertCommand(true);
      adapter.UpdateCommand = builder.GetUpdateCommand(true);
      adapter.DeleteCommand = builder.GetDeleteCommand(true);
      return adapter;
    }

    public DbDataAdapter CreateDataAdapter(string tableName)
    {
      return CreateDataAdapter(tableName, null);
    }

    public DbParameter CreateParameter()
    {
      return _factory.CreateParameter();
    }

    public DbParameter[] CreateParameters(int count)
    {
      var parameters = new DbParameter[count];
      for (var i = 0; i < count; i++)
      {
        parameters[i] = CreateParameter();
      }
      return parameters;
    }



    public bool CanCreateDataSourceEnumerator
    {
      get { return _factory.CanCreateDataSourceEnumerator; }
    }

    public string ConnectionString
    {
      get { return _connectionString; }
    }

    public string ProviderName
    {
      get { return _providerName; }
    }

    public List<DbConnection> ConnectionPool
    {
      get { return _connectionPool; }
    }

    public bool DeriveParameters(DbCommand cmd)
    {
      if ((cmd == null) || (cmd.Connection == null) || cmd.CommandType != CommandType.StoredProcedure)
      {
        return false;
      }
      DbCommandBuilder builder = CreateCommandBuilder();
      MethodInfo methodInfo = builder.GetType().GetMethod("DeriveParameters", new[] {typeof (DbCommand)});
      if (methodInfo == null) return false;
      try
      {
        methodInfo.Invoke(null, new object[] {cmd});
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public DbCommand DeriveParameters(string procName)
    {
      DbCommand command = CreateCommand((DbConnection) null);
      if (command == null) return null;
      command.CommandText = procName;
      command.CommandType = CommandType.StoredProcedure;
      return DeriveParameters(command) ? command : null;
    }

  }
}
