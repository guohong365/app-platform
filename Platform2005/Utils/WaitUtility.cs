namespace Platform.Utils
{
    using System;
    using System.Threading;

    public class WaitUtility
    {
        private ManualResetEvent m_WaitEvent = new ManualResetEvent(true);

        public bool Wait()
        {
            return this.m_WaitEvent.WaitOne(-1, false);
        }

        public bool Wait(int timeout)
        {
            return this.m_WaitEvent.WaitOne(timeout, false);
        }
    }
}
