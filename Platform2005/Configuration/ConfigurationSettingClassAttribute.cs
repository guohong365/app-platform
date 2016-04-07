namespace Platform.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationSettingClassAttribute : Attribute
    {
        private Type m_ControlType;
        private string m_DisplayName;

        public ConfigurationSettingClassAttribute(string dispName, Type settingControlType)
        {
            this.m_DisplayName = dispName;
            if (((settingControlType != null) && (settingControlType != typeof(ConfigurationControlBase))) && settingControlType.IsSubclassOf(typeof(ConfigurationControlBase)))
            {
                throw new Exception("无效配置设置控件类型！");
            }
            if (settingControlType == null)
            {
                settingControlType = typeof(ConfigurationControlBase);
            }
            else
            {
                this.m_ControlType = settingControlType;
            }
        }

        public Type ControlType
        {
            get
            {
                return this.m_ControlType;
            }
            set
            {
                this.m_ControlType = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.m_DisplayName;
            }
            set
            {
                this.m_DisplayName = value;
            }
        }
    }
}
