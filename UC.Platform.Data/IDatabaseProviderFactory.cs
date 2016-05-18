using System.Data.Common;

namespace UC.Platform.Data
{
    public interface IDatabaseProviderFactory  
    {
        DbCommandBuilder CreateCommandBuilder();
        DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter);
        DbConnection CreateConnection();
        DbConnectionStringBuilder CreateConnectionStringBuilder();
        DbDataAdapter CreateDataAdapter();
        DbParameter CreateParameter();
        bool CanCreateDataSourceEnumerator { get; }
        string ConnectionString { get; }
        string ProviderName { get; }
        DbCommand CreateCommand(DbTransaction transaction);
        DbDataAdapter CreateDataAdapter(string tableName, DbTransaction transaction);
        DbDataAdapter CreateDataAdapter(string tableName);
        DbParameter[] CreateParameters(int count);
        bool DeriveParameters(DbCommand cmd);
        DbCommand DeriveParameters(string procName);
        DbCommand CreateCommand(DbConnection connection);
        DbDataAdapter CreateDataAdapter(DbConnection connection);
    }
}