namespace Platform.StateKeeper
{
    using System;
    using System.Collections;

    public class StateKeeperHelper
    {
        private OnStateChangedEventHandler m_OnStateChangedEventHandler;
        private ArrayList m_Sinks = new ArrayList();

        public StateKeeperHelper()
        {
            this.m_OnStateChangedEventHandler = new OnStateChangedEventHandler(this.OnStateChanged);
        }

        private void OnStateChanged(Guid guid, object state)
        {
            foreach (IStateChangedSink sink in this.m_Sinks)
            {
                try
                {
                    sink.OnStateChanged(guid, state);
                    continue;
                }
                catch
                {
                    continue;
                }
            }
        }

        public void RegisterStateChangedSink(IStateChangedSink sink)
        {
            if (sink != null)
            {
                this.m_Sinks.Add(sink);
            }
        }

        public void SetStateKeeped(IStateKeeperItem item)
        {
            if (item != null)
            {
                item.OnStateChanged += this.m_OnStateChangedEventHandler;
            }
        }
    }
}
