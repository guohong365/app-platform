namespace Platform.Utils
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class PlatformTimeout
    {
        private long m_StartTicks;
        private long m_Timeout;

        public PlatformTimeout(long timeout)
        {
            GetSystemTimeAsFileTime(ref this.m_StartTicks);
            this.m_Timeout = timeout;
        }

        [DllImport("Kernel32.dll")]
        private static extern void GetSystemTimeAsFileTime(ref long ticks);
        public void Reset()
        {
            GetSystemTimeAsFileTime(ref this.m_StartTicks);
        }

        public bool IsTimeout
        {
            get
            {
                if (this.m_Timeout <= 0)
                {
                    return false;
                }
                long ticks = 0;
                GetSystemTimeAsFileTime(ref ticks);
                return ((ticks - this.m_StartTicks) > this.m_Timeout);
            }
        }

        public double PassSeconds
        {
            get
            {
                long ticks = 0;
                GetSystemTimeAsFileTime(ref ticks);
                return TimeSpan.FromTicks(ticks - this.m_StartTicks).TotalSeconds;
            }
        }

        public long PassTicks
        {
            get
            {
                long ticks = 0;
                GetSystemTimeAsFileTime(ref ticks);
                return (ticks - this.m_StartTicks);
            }
        }

        public TimeSpan PassTimeSpan
        {
            get
            {
                long ticks = 0;
                GetSystemTimeAsFileTime(ref ticks);
                return TimeSpan.FromTicks(ticks - this.m_StartTicks);
            }
        }
    }
}
