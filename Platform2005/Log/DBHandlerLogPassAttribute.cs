namespace Platform.Log
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class DBHandlerLogPassAttribute : Attribute
    {
    }
}
