namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class AssemblyConfigResourceAttribute : Attribute
    {
        private string m_ResourceName;

        public AssemblyConfigResourceAttribute(string configResourceName)
        {
            this.m_ResourceName = configResourceName;
        }

        public string ResourceName
        {
            get
            {
                return this.m_ResourceName;
            }
            set
            {
                this.m_ResourceName = value;
            }
        }
    }
}
