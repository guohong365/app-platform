namespace Platform.Caching
{
    using System;
    using System.Data;

    public interface ICacheAdapter
    {
        object ExecuteScale(string sql);
        DataSet Fill(string sql);
        bool InitializeCacheEnvironment();
        bool SaveCacheData(string tableName, DataSet ds);
    }
}
