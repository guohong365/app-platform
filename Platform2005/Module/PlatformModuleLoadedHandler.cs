namespace Platform.Module
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public delegate bool PlatformModuleLoadedHandler(Assembly asm, IPlatformModule module);
}
