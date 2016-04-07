namespace Platform.Identity
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public delegate IUser PlatformLoadUserHandler(byte[] userIdentities, out byte[] userData, out int error, out string[] msg);
}
