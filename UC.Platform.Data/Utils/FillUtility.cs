using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Xml;
using UC.Platform.Data.DBHelper;

namespace UC.Platform.Data.Utils
{
    public sealed class FillUtility
    {
        public static void FillFields(object obj, DataRow row)
        {
            FillFields(obj, row, true, true);
        }

        public static void FillFields(object obj, XmlNode node)
        {
            FillFields(obj, node, true, true);
        }

        public static void FillFields(object obj, DataRow row, Hashtable map)
        {
            FillFields(obj, row, true, map, true);
        }

        public static void FillFields(object obj, XmlNode node, Hashtable map)
        {
            FillFields(obj, node, true, map, true);
        }

        public static void FillFields(object obj, DataRow row, bool declaredOnly, bool useAttribute)
        {
            FillFields(obj, row, declaredOnly, null, useAttribute);
        }

        public static void FillFields(object obj, DataRow row, bool declaredOnly, Hashtable map)
        {
            FillFields(obj, row, declaredOnly, map, true);
        }

        public static void FillFields(object obj, DataRow row, Hashtable map, bool useAttribute)
        {
            FillFields(obj, row, true, map, useAttribute);
        }

        public static void FillFields(object obj, XmlNode node, bool declaredOnly, bool useAttribute)
        {
            FillFields(obj, node, declaredOnly, null, useAttribute);
        }

        public static void FillFields(object obj, XmlNode node, bool declaredOnly, Hashtable map)
        {
            FillFields(obj, node, declaredOnly, map, true);
        }

        public static void FillFields(object obj, XmlNode node, Hashtable map, bool useAttribute)
        {
            FillFields(obj, node, true, map, useAttribute);
        }

        public static void FillFields(object obj, DataRow row, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if (((obj != null) && (row != null)) && (row.Table != null))
            {
                DataColumnCollection columns = row.Table.Columns;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (FieldInfo info in type.GetFields(bindingAttr))
                {
                    FillFieldAttribute attribute;
                    string text;
                    if (useAttribute)
                    {
                        FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                        if ((customAttributes == null) || (customAttributes.Length < 1))
                        {
                            goto Label_00BE;
                        }
                        attribute = customAttributes[0];
                    }
                    else
                    {
                        attribute = null;
                    }
                    FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                    if (text != null)
                    {
                        object obj2;
                        if (columns.Contains(text) && !row.IsNull(text))
                        {
                            obj2 = row[text];
                        }
                        else
                        {
                            obj2 = null;
                        }
                        FillValue(attribute, info, mapInfo, obj, obj2);
                    }
                Label_00BE:;
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        public static void FillFields(object obj, XmlNode node, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if ((obj != null) && (node != null))
            {
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (FieldInfo info in type.GetFields(bindingAttr))
                {
                    FillFieldAttribute attribute;
                    string text;
                    if (useAttribute)
                    {
                        FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                        if ((customAttributes == null) || (customAttributes.Length < 1))
                        {
                            goto Label_00AF;
                        }
                        attribute = customAttributes[0];
                    }
                    else
                    {
                        attribute = null;
                    }
                    FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                    if (text != null)
                    {
                        string text2 = node.Attributes[text] == null ? null : node.Attributes[text].Value.Trim();
                        FillValue(attribute, info, mapInfo, obj, text2);
                    }
                    Label_00AF:;
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        public static void FillFromFields(object obj, DataRow row)
        {
            FillFromFields(obj, row, true, true);
        }

        public static void FillFromFields(object obj, DataRow row, Hashtable map)
        {
            FillFromFields(obj, row, true, map, true);
        }

        public static void FillFromFields(object obj, DataRow row, bool declaredOnly, bool useAttribute)
        {
            FillFromFields(obj, row, declaredOnly, null, useAttribute);
        }

        public static void FillFromFields(object obj, DataRow row, bool declaredOnly, Hashtable map)
        {
            FillFromFields(obj, row, declaredOnly, map, true);
        }

        public static void FillFromFields(object obj, DataRow row, Hashtable map, bool useAttribute)
        {
            FillFromFields(obj, row, true, map, useAttribute);
        }

        public static void FillFromFields(object obj, DataRow row, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if (((obj != null) && (row != null)) && (row.Table != null))
            {
                DataColumnCollection columns = row.Table.Columns;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (FieldInfo info in type.GetFields(bindingAttr))
                {
                    FillFieldAttribute attribute;
                    string text;
                    if (useAttribute)
                    {
                        FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                        if ((customAttributes == null) || (customAttributes.Length < 1))
                        {
                            goto Label_00BC;
                        }
                        attribute = customAttributes[0];
                    }
                    else
                    {
                        attribute = null;
                    }
                    FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                    if ((text != null) && columns.Contains(text))
                    {
                        object obj2 = info.GetValue(obj);
                        FillValue(attribute, mapInfo, row, text, columns[text].DataType, obj2);
                    }
                Label_00BC:;
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        public static void FillFromProperties(object obj, DataRow row)
        {
            FillFromProperties(obj, row, true, true);
        }

        public static void FillFromProperties(object obj, DataRow row, Hashtable map)
        {
            FillFromProperties(obj, row, true, map, true);
        }

        public static void FillFromProperties(object obj, DataRow row, bool declaredOnly, bool useAttribute)
        {
            FillFromProperties(obj, row, declaredOnly, null, useAttribute);
        }

        public static void FillFromProperties(object obj, DataRow row, bool declaredOnly, Hashtable map)
        {
            FillFromProperties(obj, row, declaredOnly, map, true);
        }

        public static void FillFromProperties(object obj, DataRow row, Hashtable map, bool useAttribute)
        {
            FillFromProperties(obj, row, true, map, useAttribute);
        }

        public static void FillFromProperties(object obj, DataRow row, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if (((obj != null) && (row != null)) && (row.Table != null))
            {
                DataColumnCollection columns = row.Table.Columns;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (PropertyInfo info in type.GetProperties(bindingAttr))
                {
                    if (info.CanRead)
                    {
                        FillFieldAttribute attribute;
                        string text;
                        if (useAttribute)
                        {
                            FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                            if ((customAttributes == null) || (customAttributes.Length < 1))
                            {
                                goto Label_00C6;
                            }
                            attribute = customAttributes[0];
                        }
                        else
                        {
                            attribute = null;
                        }
                        FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                        if ((text != null) && columns.Contains(text))
                        {
                            object obj2 = info.GetValue(obj, null);
                            FillValue(attribute, mapInfo, row, text, columns[text].DataType, obj2);
                        }
                    Label_00C6:;
                    }
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        public static void FillProperties(object obj, DataRow row)
        {
            FillProperties(obj, row, true, true);
        }

        public static void FillProperties(object obj, XmlNode node)
        {
            FillProperties(obj, node, true, true);
        }

        public static void FillProperties(object obj, DataRow row, Hashtable map)
        {
            FillProperties(obj, row, true, map, true);
        }

        public static void FillProperties(object obj, XmlNode node, Hashtable map)
        {
            FillProperties(obj, node, true, map, true);
        }

        public static void FillProperties(object obj, DataRow row, bool declaredOnly, bool useAttribute)
        {
            FillProperties(obj, row, declaredOnly, null, useAttribute);
        }

        public static void FillProperties(object obj, DataRow row, bool declaredOnly, Hashtable map)
        {
            FillProperties(obj, row, declaredOnly, map, true);
        }

        public static void FillProperties(object obj, DataRow row, Hashtable map, bool useAttribute)
        {
            FillProperties(obj, row, true, map, useAttribute);
        }

        public static void FillProperties(object obj, XmlNode node, bool declaredOnly, bool useAttribute)
        {
            FillProperties(obj, node, declaredOnly, null, useAttribute);
        }

        public static void FillProperties(object obj, XmlNode node, bool declaredOnly, Hashtable map)
        {
            FillProperties(obj, node, declaredOnly, map, true);
        }

        public static void FillProperties(object obj, XmlNode node, Hashtable map, bool useAttribute)
        {
            FillProperties(obj, node, true, map, useAttribute);
        }

        public static void FillProperties(object obj, DataRow row, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if (((obj != null) && (row != null)) && (row.Table != null))
            {
                DataColumnCollection columns = row.Table.Columns;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (PropertyInfo info in type.GetProperties(bindingAttr))
                {
                    if (info.CanWrite)
                    {
                        FillFieldAttribute attribute;
                        string text;
                        if (useAttribute)
                        {
                            FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                            if ((customAttributes == null) || (customAttributes.Length < 1))
                            {
                                goto Label_00C7;
                            }
                            attribute = customAttributes[0];
                        }
                        else
                        {
                            attribute = null;
                        }
                        FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                        if (text != null)
                        {
                            object obj2;
                            if (columns.Contains(text) && !row.IsNull(text))
                            {
                                obj2 = row[text];
                            }
                            else
                            {
                                obj2 = null;
                            }
                            FillValue(attribute, info, mapInfo, obj, obj2);
                        }
                    Label_00C7:;
                    }
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        public static void FillProperties(object obj, XmlNode node, bool declaredOnly, Hashtable map, bool useAttribute)
        {
            if ((obj != null) && (node != null))
            {
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (declaredOnly)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                }
                Type type = obj.GetType();
                foreach (PropertyInfo info in type.GetProperties(bindingAttr))
                {
                    if (info.CanWrite)
                    {
                        FillFieldAttribute attribute;
                        string text;
                        if (useAttribute)
                        {
                            FillFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(FillFieldAttribute), false) as FillFieldAttribute[];
                            if ((customAttributes == null) || (customAttributes.Length < 1))
                            {
                                goto Label_00B7;
                            }
                            attribute = customAttributes[0];
                        }
                        else
                        {
                            attribute = null;
                        }
                        FillMapInfo mapInfo = GetKey(attribute, info, map, out text);
                        if (text != null)
                        {
                            string text2;
                            if (node.Attributes[text] == null)
                            {
                                text2 = null;
                            }
                            else
                            {
                                text2 = node.Attributes[text].Value.Trim();
                            }
                            FillValue(attribute, info, mapInfo, obj, text2);
                        }
                    Label_00B7:;
                    }
                }
                MethodInfo method = type.GetMethod("AfterFill", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(obj, null);
                }
            }
        }

        private static void FillValue(FillFieldAttribute attr, FieldInfo finfo, FillMapInfo mapInfo, object obj, object value)
        {
            if ((value == null) && (attr != null))
            {
                value = attr.DefaultValue;
            }
            if ((mapInfo != null) && (mapInfo.Converter != null))
            {
                value = mapInfo.Converter(value);
            }
            else if ((value != null) && (value.GetType() != finfo.FieldType))
            {
                value = TextConverter.Convert(finfo.FieldType, value.ToString(), (attr == null) ? null : attr.ConverterName);
            }
            if (value != null)
            {
                finfo.SetValue(obj, value);
            }
        }

        private static void FillValue(FillFieldAttribute attr, PropertyInfo finfo, FillMapInfo mapInfo, object obj, object value)
        {
            if ((value == null) && (attr != null))
            {
                value = attr.DefaultValue;
            }
            if ((mapInfo != null) && (mapInfo.Converter != null))
            {
                value = mapInfo.Converter(value);
            }
            else if ((value != null) && (value.GetType() != finfo.PropertyType))
            {
                value = TextConverter.Convert(finfo.PropertyType, value.ToString(), (attr == null) ? null : attr.ConverterName);
            }
            if (value != null)
            {
                finfo.SetValue(obj, value, null);
            }
        }

        private static void FillValue(FillFieldAttribute attr, FillMapInfo mapInfo, DataRow row, string key, Type rowValueType, object value)
        {
            if ((value == null) && (attr != null))
            {
                value = attr.DefaultValue;
            }
            if ((mapInfo != null) && (mapInfo.Converter != null))
            {
                value = mapInfo.Converter(value);
            }
            else if ((value != null) && (value is string))
            {
                value = TextConverter.Convert(rowValueType, value.ToString(), (attr == null) ? null : attr.ConverterName);
            }
            if ((value == null) || ((value.GetType() == typeof(byte[])) && (((byte[]) value).Length == 0)))
            {
                row[key] = DBNull.Value;
            }
            else
            {
                row[key] = value;
            }
        }

        private static FillMapInfo GetKey(FillFieldAttribute attr, FieldInfo finfo, Hashtable map, out string key)
        {
            key = null;
            if (attr != null)
            {
                key = attr.FieldName;
            }
            if (key == null)
            {
                key = finfo.Name;
            }
            key = key.Trim();
            FillMapInfo info = null;
            if (map != null)
            {
                info = map[key] as FillMapInfo;
                if (info == null)
                {
                    string text = map[key] as string;
                    if ((text != null) && (text.Trim() != ""))
                    {
                        key = text.Trim();
                    }
                    return info;
                }
                if (info.DestField != null)
                {
                    string destField = info.DestField;
                    if ((destField != null) && (destField.Trim() != ""))
                    {
                        key = destField.Trim();
                    }
                }
            }
            return info;
        }

        private static FillMapInfo GetKey(FillFieldAttribute attr, PropertyInfo finfo, Hashtable map, out string key)
        {
            key = null;
            if (attr != null)
            {
                key = attr.FieldName;
            }
            if (key == null)
            {
                key = finfo.Name;
            }
            key = key.Trim();
            FillMapInfo info = null;
            if (map != null)
            {
                info = map[key] as FillMapInfo;
                if (info == null)
                {
                    string text = map[key] as string;
                    if ((text != null) && (text.Trim() != ""))
                    {
                        key = text.Trim();
                    }
                    return info;
                }
                if (info.DestField != null)
                {
                    string destField = info.DestField;
                    if ((destField != null) && (destField.Trim() != ""))
                    {
                        key = destField.Trim();
                    }
                }
            }
            return info;
        }
    }
}
