namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BroadcastRemotingClassAttribute : Attribute
    {
        private string m_Name;
        private string m_Namespace;

        public BroadcastRemotingClassAttribute()
        {
            this.m_Name = null;
            this.m_Namespace = null;
        }

        public BroadcastRemotingClassAttribute(string name)
        {
            this.m_Name = name;
            this.m_Namespace = null;
        }

        public BroadcastRemotingClassAttribute(string name, string nameSpace)
        {
            this.m_Name = name;
            this.m_Namespace = nameSpace;
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public string Namespace
        {
            get
            {
                return this.m_Namespace;
            }
        }
    }
}
