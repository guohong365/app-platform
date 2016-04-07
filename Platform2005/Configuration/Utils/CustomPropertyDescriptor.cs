namespace Platform.Configuration.Utils
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private static Hashtable m_Editors = new Hashtable();
        private ConfigItem m_Item;

        public CustomPropertyDescriptor(ConfigItem item, Attribute[] attributes) : base(item.ConfigName, attributes)
        {
            this.m_Item = item;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetEditor(Type editorBaseType)
        {
            Type type = m_Editors[this.m_Item.ConfigUIType] as Type;
            if (type != null)
            {
                return (Activator.CreateInstance(type) as UITypeEditor);
            }
            return base.GetEditor(editorBaseType);
        }

        public override object GetValue(object component)
        {
            return this.m_Item.ConfigValue;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            if (value == null)
            {
                this.m_Item.ConfigValue = "";
            }
            else
            {
                this.m_Item.ConfigValue = value.ToString();
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override string Category
        {
            get
            {
                return this.m_Item.ConfigGroup;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return this.m_Item.Type;
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return base.Converter;
            }
        }

        public override string Description
        {
            get
            {
                return this.m_Item.ConfigDescription;
            }
        }

        public override string DisplayName
        {
            get
            {
                return this.m_Item.ConfigName;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        public override bool IsLocalizable
        {
            get
            {
                return true;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ConfigItem Item
        {
            get
            {
                return this.m_Item;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return this.m_Item.Type;
            }
        }
    }
}
