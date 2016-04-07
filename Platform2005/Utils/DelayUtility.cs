namespace Platform.Utils
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    public sealed class DelayUtility
    {
        public static void DelayDoEvent(int ticks)
        {
            DateTime now = DateTime.Now;
            while (true)
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - now);
                if (span.TotalMilliseconds >= ticks)
                {
                    return;
                }
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }
    }
}
