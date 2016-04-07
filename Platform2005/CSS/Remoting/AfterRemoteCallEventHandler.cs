namespace Platform.CSS.Remoting
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void AfterRemoteCallEventHandler(Guid packetID, string methodName, bool hasException);
}
