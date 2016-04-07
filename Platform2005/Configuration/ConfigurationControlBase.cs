namespace Platform.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Forms;

    public class ConfigurationControlBase : UserControl
    {
        private Container components;

        public ConfigurationControlBase()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        public virtual void LoadSettings(System.Type settingType, FieldInfo[] settingField)
        {
        }
    }
}
