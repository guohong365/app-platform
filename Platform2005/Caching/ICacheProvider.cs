namespace Platform.Caching
{
    using System;
    using System.Data;

    public interface ICacheProvider
    {
        DataSet GetTableData(string tableName);
        DataSet GetTables();
    }
}
