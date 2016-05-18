using System;
using System.Reflection;

namespace UC.Platform.Data.Utils
{
    public sealed class ObjectUtility
    {
        public static object GetFieldValue(string fieldName, object target)
        {
            if (target == null)
            {
                return null;
            }
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (field == null)
            {
                return null;
            }
            return field.GetValue(target);
        }

        public static object GetPropertyValue(string propertyName, object target)
        {
            if (target == null)
            {
                return null;
            }
            PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property == null)
            {
                return null;
            }
            if (!property.CanRead)
            {
                return null;
            }
            return property.GetValue(target, null);
        }

        public static object GetStaticFieldValue(string fieldName, Type target)
        {
            if (target == null)
            {
                return null;
            }
            FieldInfo field = target.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return null;
            }
            return field.GetValue(null);
        }

        public static object GetStaticPropertyValue(string propertyName, Type target, object value)
        {
            if (target == null)
            {
                return null;
            }
            PropertyInfo property = target.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (property == null)
            {
                return null;
            }
            if (!property.CanRead)
            {
                return null;
            }
            return property.GetValue(null, null);
        }

        public static bool PropertiesCopy(object source, object dest, bool inhert)
        {
            Type type = source.GetType();
            if (type != dest.GetType())
            {
                return false;
            }
            PropertyInfo[] properties = inhert
                ? type.GetProperties()
                : type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo info in properties)
            {
                if (info.CanWrite)
                {
                    info.SetValue(dest, info.GetValue(source, null), null);
                }
            }
            return true;
        }

        public static bool SetFieldValue(string fieldName, object target, object value)
        {
            if (target == null)
            {
                return false;
            }
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (field == null)
            {
                return false;
            }
            field.SetValue(target, value);
            return true;
        }

        public static bool SetPropertyValue(string propertyName, object target, object value)
        {
            if (target == null)
            {
                return false;
            }
            PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property == null)
            {
                return false;
            }
            if (!property.CanWrite)
            {
                return false;
            }
            property.SetValue(target, value, null);
            return true;
        }

        public static bool SetStaticFieldValue(string fieldName, Type target, object value)
        {
            if (target == null)
            {
                return false;
            }
            FieldInfo field = target.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return false;
            }
            field.SetValue(null, value);
            return true;
        }

        public static bool SetStaticPropertyValue(string propertyName, Type target, object value)
        {
            if (target == null)
            {
                return false;
            }
            PropertyInfo property = target.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (property == null)
            {
                return false;
            }
            if (!property.CanWrite)
            {
                return false;
            }
            property.SetValue(null, value, null);
            return true;
        }
    }
}
