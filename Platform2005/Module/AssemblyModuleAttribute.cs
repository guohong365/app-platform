namespace Platform.Module
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class AssemblyModuleAttribute : Attribute
    {
        private string m_ModuleName;
        private System.Type m_Type;

        public AssemblyModuleAttribute(System.Type type) : this(type.FullName, type)
        {
        }

        public AssemblyModuleAttribute(string moduleName, System.Type type)
        {
            this.m_Type = type;
            this.m_ModuleName = moduleName;
        }

        public string ModuleName
        {
            get
            {
                return this.m_ModuleName;
            }
        }

        public System.Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
    }
}
