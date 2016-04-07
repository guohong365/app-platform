namespace Platform.Configuration
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate object ConfigConvertHandler(Type type, string configText);
}
