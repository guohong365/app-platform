namespace Platform.Identity
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public delegate bool PlatformRemoveUserHandler(IUser user, byte[] userIdentities, out byte[] userData, out int error, out string[] msg);
}
