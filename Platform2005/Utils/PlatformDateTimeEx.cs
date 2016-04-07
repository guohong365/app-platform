namespace Platform.Utils
{
    using System;

    public sealed class PlatformDateTimeEx
    {
        private long m_BaseTimeTicks;

        public DateTime Now
        {
            get
            {
                return new DateTime(DateTime.Now.Ticks - this.m_BaseTimeTicks);
            }
            set
            {
                this.m_BaseTimeTicks = DateTime.Now.Ticks - value.Ticks;
            }
        }
    }
}
