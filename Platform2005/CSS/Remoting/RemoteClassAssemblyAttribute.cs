namespace Platform.CSS.Remoting
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public class RemoteClassAssemblyAttribute : Attribute
    {
        private System.Type m_Type;

        public RemoteClassAssemblyAttribute(System.Type type)
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
