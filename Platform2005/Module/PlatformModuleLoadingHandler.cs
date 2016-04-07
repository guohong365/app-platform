namespace Platform.Module
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public delegate bool PlatformModuleLoadingHandler(Assembly asm, Type moduleType);
}
