namespace Platform.Utils
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Class)]
    public class StackTracePassAttribute : Attribute
    {
    }
}
