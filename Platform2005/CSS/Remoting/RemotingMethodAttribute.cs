namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RemotingMethodAttribute : Attribute
    {
        private string m_FullName;

        public RemotingMethodAttribute()
        {
        }

        public RemotingMethodAttribute(string fullName)
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
