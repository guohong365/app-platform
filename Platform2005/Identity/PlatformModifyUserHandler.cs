namespace Platform.Identity
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public delegate bool PlatformModifyUserHandler(IUser user, byte[] modifyData, out byte[] modifyRcData);
}
