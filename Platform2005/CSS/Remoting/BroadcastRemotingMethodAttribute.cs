namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BroadcastRemotingMethodAttribute : Attribute
    {
        private string m_FullName;

        public BroadcastRemotingMethodAttribute()
        {
            this.m_FullName = null;
        }

        public BroadcastRemotingMethodAttribute(string fullName)
        {
            this.m_FullName = fullName;
        }

        public string FullName
        {
            get
            {
                return this.m_FullName;
            }
            set
            {
                this.m_FullName = value;
            }
        }
    }
}
