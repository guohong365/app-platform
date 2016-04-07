namespace Platform.Utils
{
    using System;

    public interface EventProxySink
    {
        void OnEventInvoke(object[] eventArgs);
    }
}
