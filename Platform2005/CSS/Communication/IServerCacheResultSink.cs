namespace Platform.CSS.Communication
{
    using Platform.Identity;
    using Platform.IO;
    using System;
    using System.Runtime.InteropServices;

    public interface IServerCacheResultSink
    {
        void GetResult(IUser user, Guid packetID, int threadID, out MemoryStream result);
        void SaveResult(IUser user, Guid packetID, int threadID, MemoryStream result);
    }
}
