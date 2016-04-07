namespace Platform.CSS.Server
{
    using Platform.Configuration;
    using System;

    public sealed class LoginHandlersConfig
    {
        [ConfigItem("/PlatformSettings", "LoginLimitation", null)]
        public static int LoginLimitation = 10;

        private static void BeforeInitialize()
        {
        }
    }
}
