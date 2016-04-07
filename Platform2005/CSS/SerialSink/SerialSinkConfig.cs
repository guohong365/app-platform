namespace Platform.CSS.SerialSink
{
    using Platform.Configuration;
    using System;

    public sealed class SerialSinkConfig
    {
        [ConfigItem("/PlatformSettings", "ZipLimitation", null)]
        public static int ZipLimitation = 0x400;

        private static void BeforeInitialize()
        {
        }
    }
}
