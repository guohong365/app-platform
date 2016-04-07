namespace Platform.Log
{
    using Platform.Configuration;
    using System;

    public sealed class LogConfig
    {
        [ConfigItem("/PlatformSettings", "WriteDBHelperLog", null)]
        public static bool WriteDBHelperLog = true;
        [ConfigItem("/PlatformSettings", "WriteExceptionLog", null)]
        public static bool WriteExceptionLog = true;
        [ConfigItem("/PlatformSettings", "WriteLiveUpdateLog", null)]
        public static bool WriteLiveUpdateLog = true;
        [ConfigItem("/PlatformSettings", "WriteLoginLog", null)]
        public static bool WriteLoginLog = true;
        [ConfigItem("/PlatformSettings", "WriteOperationLog", null)]
        public static bool WriteOperationLog = true;
        [ConfigItem("/PlatformSettings", "WritePlatformLog", null)]
        public static bool WritePlatformLog = true;

        private static void AfterInitialize()
        {
        }

        private static void BeforeInitialize()
        {
        }
    }
}
