namespace Platform.Utils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Assert)]
    public sealed class PlatformDateTime
    {
        private static long m_BaseTimeTicks;
        private static long m_TimeZoneUtcTicks;
        public const long Ticks16010101 = 0x701ce1722770000;

        static PlatformDateTime()
        {
            DateTime time = new DateTime(GetUtcSystemFileTime() + 0x701ce1722770000);
            m_TimeZoneUtcTicks = TimeZone.CurrentTimeZone.GetUtcOffset(time).Ticks;
        }

        [DllImport("Kernel32.dll")]
        private static extern void GetSystemTimeAsFileTime(ref long ticks);
        [DllImport("Kernel32.dll")]
        public static extern int GetTickCount();
        public static long GetUtcSystemFileTime()
        {
            long ticks = 0;
            GetSystemTimeAsFileTime(ref ticks);
            return ticks;
        }

        public static void GetUtcSystemFileTime(ref long ticks)
        {
            GetSystemTimeAsFileTime(ref ticks);
        }

        public static DateTime Now
        {
            get
            {
                return new DateTime(((GetUtcSystemFileTime() + 0x701ce1722770000) + m_TimeZoneUtcTicks) - m_BaseTimeTicks);
            }
            set
            {
                m_BaseTimeTicks = ((GetUtcSystemFileTime() + 0x701ce1722770000) + m_TimeZoneUtcTicks) - value.Ticks;
            }
        }

        public static long Ticks
        {
            get
            {
                return ((GetUtcSystemFileTime() + 0x701ce1722770000) + m_TimeZoneUtcTicks);
            }
        }

        public static long UtcTicks
        {
            get
            {
                return (GetUtcSystemFileTime() + 0x701ce1722770000);
            }
        }

        public static long UtcTicksOffset
        {
            get
            {
                return m_TimeZoneUtcTicks;
            }
        }
    }
}
