namespace Platform.Utils
{
    using System;
    using System.Collections;

    public sealed class DateTimeUtility
    {
        private static Hashtable m_TimeKeepers = new Hashtable();

        public static DateTime GetCurrentTime(string timeName)
        {
            PlatformDateTimeEx ex = m_TimeKeepers[timeName] as PlatformDateTimeEx;
            if (ex == null)
            {
                throw new Exception("无效时间名称");
            }
            return ex.Now;
        }

        public static void SetCurrentTime(string timeName, DateTime currentTime)
        {
            PlatformDateTimeEx ex = m_TimeKeepers[timeName] as PlatformDateTimeEx;
            if (ex == null)
            {
                ex = new PlatformDateTimeEx();
                m_TimeKeepers[timeName] = ex;
            }
            ex.Now = currentTime;
        }
    }
}
