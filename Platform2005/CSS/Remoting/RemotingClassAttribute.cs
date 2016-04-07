namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RemotingClassAttribute : Attribute
    {
        private string m_Name;
        private string m_Namespace;

        public RemotingClassAttribute()
        {
            this.m_Name = null;
            this.m_Namespace = null;
        }

        public RemotingClassAttribute(string name)
        {
            this.m_Name = name;
            this.m_Namespace = null;
        }

        public RemotingClassAttribute(string name, string nameSpace)
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
