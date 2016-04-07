namespace Platform.Task
{
    using Platform.Configuration;
    using System;
    using System.Collections;

    public sealed class TaskConfig
    {
        [ConfigCollectionItem("/AutoTaskThreadLimitation", null, null, typeof(int))]
        public static Hashtable AutoTaskThreadLimitation;
        [ConfigItem("/TaskSettings", "AutoTaskThreadTotalCount", null)]
        public static int AutoTaskThreadTotalCount;
    }
}
