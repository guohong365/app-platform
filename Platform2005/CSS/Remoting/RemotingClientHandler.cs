namespace Platform.CSS.Remoting
{
    using System;
    using System.Data;

    [RemotingClass("RemotingServerHandler", "Platform.CSS.Remoting")]
    public sealed class RemotingClientHandler
    {
        [RemotingMethod("{1BA1C17E-2992-4A06-A71F-6539D1EB2FD4}")]
        public static DataSet GetDataSet(string tableName)
        {
            object[] parameters = new object[] { tableName };
            byte[] parametersDirect = new byte[1];
            return (DataSet) RemotingClient.RemoteExecute("{1BA1C17E-2992-4A06-A71F-6539D1EB2FD4}", parametersDirect, parameters);
        }

        [RemotingMethod("{C5749343-DEF9-4BCA-A86A-7D1BFE1B097A}")]
        public static DateTime GetPlatformCurrentDateTime()
        {
            object[] parameters = new object[0];
            return (DateTime) RemotingClient.RemoteExecute("{C5749343-DEF9-4BCA-A86A-7D1BFE1B097A}", new byte[0], parameters);
        }
    }
}
