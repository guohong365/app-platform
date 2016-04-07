namespace Platform.CSS.Server
{
    using Platform.Identity;
    using System;
    using System.Runtime.CompilerServices;

    public delegate void OnUserLoginFinishHandler(IUser user, byte[] key, byte[] iv);
}
