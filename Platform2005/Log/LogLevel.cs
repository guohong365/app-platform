namespace Platform.Log
{
    using System;

    public enum LogLevel
    {
        All = 0x7fffffff,
        IncludeIgnoreInfo = 4,
        None = 0,
        NoneUser = 1,
        User = 2
    }
}
