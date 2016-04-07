namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public class Remote2ndClassAssemblyAttribute : Attribute
    {
        private System.Type m_Type;

        public Remote2ndClassAssemblyAttribute(System.Type type)
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
