namespace Platform.StateKeeper
{
    using System;
    using System.Runtime.CompilerServices;

    public interface IStateKeeperItem
    {
        event OnStateChangedEventHandler OnStateChanged;

        Guid ItemGuid { get; }
    }
}
