namespace Platform.Identity
{
    using System;

    public interface IPermissionAttribute
    {
        bool CheckPermission(byte[] permission);
    }
}
