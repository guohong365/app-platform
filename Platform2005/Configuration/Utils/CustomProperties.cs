namespace Platform.Configuration.Utils
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    public class CustomProperties : ICustomTypeDescriptor
    {
        private IList m_List;
        private PropertyDescriptorCollection m_PropsCollection = null;

        public CustomProperties(IList list)
        {
            this.m_List = list;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(typeof(ConfigItem));
        }

        public string GetClassName()
        {
            return null;
        }

        public string GetComponentName()
        {
            return null;
        }

        public TypeConverter GetConverter()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return PropertyDescriptorCollection.Empty;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (this.m_PropsCollection == null)
            {
                ArrayList list = new ArrayList();
                foreach (ConfigItem item in this.m_List)
                {
                    CustomPropertyDescriptor descriptor = new CustomPropertyDescriptor(item, attributes);
                    list.Add(descriptor);
                }
                this.m_PropsCollection = new PropertyDescriptorCollection(list.ToArray(typeof(CustomPropertyDescriptor)) as CustomPropertyDescriptor[]);
            }
            return this.m_PropsCollection;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (pd != null)
            {
                return ((CustomPropertyDescriptor) pd).Item;
            }
            return this;
        }
    }
}
