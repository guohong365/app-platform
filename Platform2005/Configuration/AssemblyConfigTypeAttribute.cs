namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class AssemblyConfigTypeAttribute : Attribute
    {
        private System.Type m_Type;

        public AssemblyConfigTypeAttribute(System.Type type)
        {
            this.m_Type = type;
        }

        public System.Type Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
    }
}
