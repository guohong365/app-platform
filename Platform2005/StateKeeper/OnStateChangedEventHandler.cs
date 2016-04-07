namespace Platform.StateKeeper
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void OnStateChangedEventHandler(Guid guid, object state);
}
