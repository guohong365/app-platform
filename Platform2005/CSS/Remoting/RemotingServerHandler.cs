namespace Platform.CSS.Remoting
{
    using Platform.DBHelper;
    using Platform.Utils;
    using System;
    using System.Data;

    [RemotingClass("RemotingServerHandler")]
    public sealed class RemotingServerHandler
    {
        [RemotingMethod("{1BA1C17E-2992-4A06-A71F-6539D1EB2FD4}")]
        public static DataSet GetDataSet(string tableName)
        {
            return DBHandler.GetDataSetOnce(tableName);
        }

        [RemotingMethod("{C5749343-DEF9-4BCA-A86A-7D1BFE1B097A}")]
        public static DateTime GetPlatformCurrentDateTime()
        {
            return PlatformDateTime.Now;
        }
    }
}
