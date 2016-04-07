namespace Platform.Identity
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true)]
    public sealed class PermissionAttribute : Attribute
    {
        private int[] m_AllPassPermissions;
        private int[] m_OnePassPermissions;

        public PermissionAttribute(PermissionCheckType checkType, params int[] permissions)
        {
            if (checkType == PermissionCheckType.OnePass)
            {
                this.m_OnePassPermissions = permissions;
                this.m_AllPassPermissions = new int[0];
            }
            else
            {
                this.m_OnePassPermissions = new int[0];
                this.m_AllPassPermissions = permissions;
            }
        }

        public PermissionAttribute(int[] allPassPermissions, int[] onePassPermissions)
        {
            if (allPassPermissions == null)
            {
                this.m_AllPassPermissions = new int[0];
            }
            else
            {
                this.m_AllPassPermissions = allPassPermissions;
            }
            if (onePassPermissions == null)
            {
                this.m_OnePassPermissions = new int[0];
            }
            else
            {
                this.m_OnePassPermissions = onePassPermissions;
            }
        }

        public int[] AllPassPermissions
        {
            get
            {
                return this.m_AllPassPermissions;
            }
        }

        public int[] OnePassPermissions
        {
            get
            {
                return this.m_OnePassPermissions;
            }
        }
    }
}
