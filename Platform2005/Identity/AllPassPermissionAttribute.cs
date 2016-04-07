namespace Platform.Identity
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true)]
    public sealed class AllPassPermissionAttribute : Attribute, IPermissionAttribute
    {
        private int[] m_Permissions;

        public AllPassPermissionAttribute(params int[] permissions)
        {
            this.m_Permissions = permissions;
        }

        public bool CheckPermission(byte[] permission)
        {
            if (this.m_Permissions != null)
            {
                if (permission == null)
                {
                    return true;
                }
                for (int i = 0; i < this.m_Permissions.Length; i++)
                {
                    int index = this.m_Permissions[i] >> 3;
                    int num3 = this.m_Permissions[i] & 7;
                    if (permission.Length <= index)
                    {
                        return false;
                    }
                    if ((permission[index] & (((int) 1) << num3)) == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int[] Permissions
        {
            get
            {
                return this.m_Permissions;
            }
        }
    }
}
