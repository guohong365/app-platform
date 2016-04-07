namespace Platform.CSS.Remoting
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate bool BeforeRemoteCallEventHandler(Guid packetID, string methodName, object[] parameters);
}
