namespace Platform.StateKeeper
{
    using System;

    public interface IStateChangedSink
    {
        void OnStateChanged(Guid guid, object state);
    }
}
