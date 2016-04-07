namespace Platform.Identity
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void PlatformUserBanChangedHandler(IUser user, bool ban, string banMessage);
}
