namespace Platform.Threading
{
    using Platform.Configuration;
    using System;

    public sealed class ThreadingConfig
    {
        [ConfigItem("/PlatformSettings", "ThreadPoolLimitation", null)]
        public static int ThreadPoolLimitation = 0x400;

        private static void BeforeInitialize()
        {
        }
    }
}
