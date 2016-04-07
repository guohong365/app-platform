namespace Platform.Identity
{
    using System;
    using System.Reflection;

    public interface IAuthUser
    {
        bool CheckPass(byte[] pass);
        bool CheckPermission(IPermissionAttribute attribute);
        bool CheckPermission(int permissionID);
        bool CheckPermission(MemberInfo member);
    }
}
