namespace Platform.Configuration
{
    using Microsoft.Win32;
    using Platform.ExceptionHandling;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public sealed class ConfigHelper
    {
        private Hashtable m_values;

        public ConfigHelper()
        {
            this.m_values = new Hashtable();
        }

        public ConfigHelper(string strConfigFileName)
        {
            this.m_values = new Hashtable();
            this.LoadConfig(strConfigFileName);
        }

        public ConfigHelper(Assembly asm, string resourceName)
        {
            this.m_values = new Hashtable();
            this.LoadConfig(asm, resourceName, false);
        }

        public ConfigHelper(DataTable dt, string sectionField, string keyField, string valueField)
        {
            this.m_values = new Hashtable();
            this.LoadConfig(dt, sectionField, keyField, valueField, null, false);
        }

        public ConfigHelper(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField)
        {
            this.m_values = new Hashtable();
            this.LoadConfig(dt, sectionField, keyField, valueField, descriptionField, false);
        }

        public void Add(string sectionName, Hashtable values)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                this.m_values[sectionName] = hashtable;
            }
            foreach (DictionaryEntry entry in values)
            {
                hashtable[entry.Key] = entry.Value;
            }
        }

        public void Add(string sectionName, string name, string configText)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                this.m_values[sectionName] = hashtable;
            }
            ConfigValueItem item = hashtable[name] as ConfigValueItem;
            if (item == null)
            {
                item = new ConfigValueItem(configText, "");
                hashtable[name] = item;
            }
            else
            {
                item.Value = configText;
            }
        }

        public void Add(string sectionName, string name, string configText, string description)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                this.m_values[sectionName] = hashtable;
            }
            ConfigValueItem item = hashtable[name] as ConfigValueItem;
            if (item == null)
            {
                item = new ConfigValueItem(configText, description);
                hashtable[name] = item;
            }
            else
            {
                item.Value = configText;
                item.Description = (description == null) ? "" : description;
            }
        }

        public ConfigHelper AddConfigHelper(ConfigHelper helper)
        {
            return this.AddConfigHelper(helper, true);
        }

        public ConfigHelper AddConfigHelper(ConfigHelper helper, bool replaceOlder)
        {
            foreach (DictionaryEntry entry in helper.m_values)
            {
                Hashtable hashtable = this.m_values[entry.Key] as Hashtable;
                if (hashtable == null)
                {
                    hashtable = new Hashtable();
                    this.m_values[entry.Key] = hashtable;
                }
                foreach (DictionaryEntry entry2 in (Hashtable) entry.Value)
                {
                    if (!replaceOlder && hashtable.ContainsKey(entry2.Key))
                    {
                        continue;
                    }
                    hashtable[entry2.Key] = entry2.Value;
                }
            }
            return this;
        }

        public void Clear()
        {
            this.m_values.Clear();
        }

        public bool Contains(string sectionName, string name)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                return false;
            }
            return hashtable.ContainsKey(name);
        }

        private static XmlNode CreateXmlDocumentSection(XmlDocument doc, string sectionName)
        {
            string[] textArray = sectionName.Trim(new char[] { '/' }).Split(new char[] { '/' });
            XmlNode documentElement = doc.DocumentElement;
            for (int i = 0; i < textArray.Length; i++)
            {
                if (textArray[i].Length >= 1)
                {
                    XmlNode newChild = documentElement[textArray[i]];
                    if (newChild == null)
                    {
                        newChild = doc.CreateElement(textArray[i]);
                        documentElement.AppendChild(newChild);
                    }
                    documentElement = newChild;
                }
            }
            return documentElement;
        }

        public void FillConfig(object obj)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                bool flag = false;
                if (type == typeof(Type).GetType())
                {
                    flag = true;
                    type = obj as Type;
                }
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if ((fields != null) && (fields.Length >= 1))
                {
                    foreach (FieldInfo info in fields)
                    {
                        if (!flag || info.IsStatic)
                        {
                            ConfigItemAttribute[] customAttributes = info.GetCustomAttributes(typeof(ConfigItemAttribute), false) as ConfigItemAttribute[];
                            ConfigCollectionItemAttribute[] attributeArray2 = info.GetCustomAttributes(typeof(ConfigCollectionItemAttribute), false) as ConfigCollectionItemAttribute[];
                            if (customAttributes.Length > 0)
                            {
                                string sectionName = customAttributes[0].SectionName;
                                string key = customAttributes[0].Key;
                                if (key == null)
                                {
                                    key = info.Name;
                                }
                                string configText = this.GetValue(sectionName, key);
                                if (configText != null)
                                {
                                    try
                                    {
                                        object obj2 = ConfigConverters.Convert(info.FieldType, configText, customAttributes[0].Converter);
                                        info.SetValue(info.IsStatic ? null : obj, obj2);
                                    }
                                    catch (Exception exception)
                                    {
                                        ExceptionHelper.HandleException(exception);
                                    }
                                }
                            }
                            else if (attributeArray2.Length > 0)
                            {
                                string text4 = attributeArray2[0].SectionName;
                                Hashtable values = this.GetValues(text4);
                                if (values != null)
                                {
                                    try
                                    {
                                        object obj3 = ConfigConverters.Convert(info.FieldType, values, attributeArray2[0]);
                                        info.SetValue(info.IsStatic ? null : obj, obj3);
                                    }
                                    catch (Exception exception2)
                                    {
                                        ExceptionHelper.HandleException(exception2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static ConfigHelper GetConfigHelper(string strConfigFileName)
        {
            try
            {
                return new ConfigHelper(strConfigFileName);
            }
            catch
            {
                return null;
            }
        }

        public static ConfigHelper GetConfigHelper(Assembly asm, string resourceName)
        {
            try
            {
                return new ConfigHelper(asm, resourceName);
            }
            catch
            {
                return null;
            }
        }

        public static ConfigHelper GetConfigHelper(DataTable dt, string sectionField, string keyField, string valueField)
        {
            try
            {
                return new ConfigHelper(dt, sectionField, keyField, valueField);
            }
            catch
            {
                return null;
            }
        }

        public static ConfigHelper GetConfigHelper(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField)
        {
            try
            {
                return new ConfigHelper(dt, sectionField, keyField, valueField, descriptionField);
            }
            catch
            {
                return null;
            }
        }

        public string GetValue(string sectionName, string name)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            ConfigValueItem item = hashtable[name] as ConfigValueItem;
            if (item == null)
            {
                return null;
            }
            return item.Value;
        }

        public object GetValue(string sectionName, string name, Type type)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                return null;
            }
            ConfigValueItem item = hashtable[name] as ConfigValueItem;
            if (item == null)
            {
                return null;
            }
            return Convert.ChangeType(item.Value, type);
        }

        public Hashtable GetValues(string sectionName)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable d = this.m_values[sectionName] as Hashtable;
            if (d == null)
            {
                return null;
            }
            return new Hashtable(d);
        }

        public bool LoadConfig(Assembly asm)
        {
            return this.LoadConfig(asm, false, false);
        }

        public bool LoadConfig(string strConfigFileName)
        {
            return this.LoadConfig(strConfigFileName, false);
        }

        public bool LoadConfig(Assembly asm, bool clear)
        {
            return this.LoadConfig(asm, clear, false);
        }

        public bool LoadConfig(Assembly asm, string resourceName)
        {
            return this.LoadConfig(asm, resourceName, false, false);
        }

        public bool LoadConfig(string strConfigFileName, bool clear)
        {
            this.LoadConfig(strConfigFileName, clear, true);
            return true;
        }

        public bool LoadConfig(Assembly asm, bool clear, bool replaceOlder)
        {
            AssemblyConfigResourceAttribute[] customAttributes = asm.GetCustomAttributes(typeof(AssemblyConfigResourceAttribute), false) as AssemblyConfigResourceAttribute[];
            if (customAttributes.Length < 1)
            {
                return false;
            }
            foreach (AssemblyConfigResourceAttribute attribute in customAttributes)
            {
                this.LoadConfig(asm, attribute.ResourceName, clear, replaceOlder);
            }
            return true;
        }

        public bool LoadConfig(Assembly asm, string resourceName, bool clear)
        {
            return this.LoadConfig(asm, resourceName, clear, false);
        }

        public bool LoadConfig(string strConfigFileName, bool clear, bool replaceOlder)
        {
            if (clear)
            {
                this.m_values.Clear();
            }
            XmlDocument document = new XmlDocument();
            document.Load(strConfigFileName);
            XmlElement documentElement = document.DocumentElement;
            if (documentElement == null)
            {
                return false;
            }
            this.LoadNodes(documentElement.ChildNodes, "/", replaceOlder);
            return true;
        }

        public bool LoadConfig(DataTable dt, string sectionField, string keyField, string valueField)
        {
            return this.LoadConfig(dt, sectionField, keyField, valueField, null, false);
        }

        public bool LoadConfig(Assembly asm, string resourceName, bool clear, bool replaceOlder)
        {
            if (clear)
            {
                this.m_values.Clear();
            }
            if (resourceName == null)
            {
                resourceName = asm.GetName().Name + ".config";
            }
            Stream manifestResourceStream = ResourceUtility.GetManifestResourceStream(resourceName, asm);
            if (manifestResourceStream == null)
            {
                return false;
            }
            XmlDocument document = new XmlDocument();
            document.Load(manifestResourceStream);
            XmlElement documentElement = document.DocumentElement;
            if (documentElement == null)
            {
                return false;
            }
            this.LoadNodes(documentElement.ChildNodes, "/", replaceOlder);
            return true;
        }

        public bool LoadConfig(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField)
        {
            return this.LoadConfig(dt, sectionField, keyField, valueField, descriptionField, false);
        }

        public bool LoadConfig(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField, bool clear)
        {
            return this.LoadConfig(dt, sectionField, keyField, valueField, descriptionField, false, true);
        }

        public bool LoadConfig(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField, bool clear, bool replaceOlder)
        {
            if (clear)
            {
                this.m_values.Clear();
            }
            string key = null;
            string text2 = null;
            string description = null;
            string sectionName = null;
            foreach (DataRow row in dt.Rows)
            {
                sectionName = row[sectionField] as string;
                key = row[keyField] as string;
                text2 = row[valueField] as string;
                if (descriptionField == null)
                {
                    description = "";
                }
                else
                {
                    description = row[descriptionField] as string;
                }
                if ((key != null) && (text2 != null))
                {
                    sectionName = RegularSectionName(sectionName);
                    Hashtable hashtable = this.m_values[sectionName] as Hashtable;
                    if (hashtable == null)
                    {
                        hashtable = new Hashtable();
                        this.m_values[sectionName] = hashtable;
                    }
                    if (replaceOlder || !hashtable.ContainsKey(key))
                    {
                        hashtable[key] = new ConfigValueItem(text2, description);
                    }
                }
            }
            return true;
        }

        private void LoadNodes(XmlNodeList nodes, string sectionName, bool replaceOlder)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = null;
            hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                this.m_values[sectionName] = hashtable;
            }
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "add")
                {
                    string key = null;
                    string text2 = null;
                    string description = null;
                    if (node.Attributes["key"] != null)
                    {
                        key = node.Attributes["key"].Value;
                    }
                    if (node.Attributes["value"] != null)
                    {
                        text2 = node.Attributes["value"].Value;
                    }
                    if (node.Attributes["description"] != null)
                    {
                        description = node.Attributes["description"].Value;
                    }
                    if (((key != null) && (text2 != null)) && (replaceOlder || !hashtable.ContainsKey(key)))
                    {
                        hashtable[key] = new ConfigValueItem(text2, description);
                    }
                    continue;
                }
                this.LoadNodes(node.ChildNodes, sectionName + "/" + node.Name, replaceOlder);
            }
        }

        private void LoadReg(RegistryKey reg, string regPath, bool replaceOlder)
        {
            string text = RegularSectionName(regPath);
            Hashtable hashtable = this.m_values[text] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                this.m_values[text] = hashtable;
            }
            foreach (string text2 in reg.GetValueNames())
            {
                string text3 = reg.GetValue(text2) as string;
                if ((text3 != null) && (replaceOlder || !hashtable.ContainsKey(text2)))
                {
                    hashtable[text2] = new ConfigValueItem(text3, "");
                }
            }
            foreach (string text4 in reg.GetSubKeyNames())
            {
                this.LoadReg(reg.OpenSubKey(text4), regPath + "/" + text4, replaceOlder);
            }
        }

        public bool LoadRegConfig(RegistryKey reg, string regPath)
        {
            return this.LoadRegConfig(reg, regPath, false);
        }

        public bool LoadRegConfig(RegistryKey reg, string regPath, bool clear)
        {
            return this.LoadRegConfig(reg, regPath, false, true);
        }

        public bool LoadRegConfig(RegistryKey reg, string regPath, bool clear, bool replaceOlder)
        {
            if (clear)
            {
                this.m_values.Clear();
            }
            RegistryKey key = reg.OpenSubKey(regPath, false);
            if (key == null)
            {
                return false;
            }
            this.LoadReg(key, "", replaceOlder);
            return true;
        }

        public static string RegularSectionName(string sectionName)
        {
            if (sectionName == null)
            {
                sectionName = "/";
            }
            else if (!sectionName.StartsWith("/"))
            {
                sectionName = "/" + sectionName;
            }
            return sectionName.TrimEnd(new char[] { '/' });
        }

        public void Remove(string sectionName)
        {
            sectionName = RegularSectionName(sectionName);
            this.m_values.Remove(sectionName);
        }

        public void Remove(string sectionName, string name)
        {
            sectionName = RegularSectionName(sectionName);
            Hashtable hashtable = this.m_values[sectionName] as Hashtable;
            if (hashtable != null)
            {
                hashtable.Remove(name);
            }
        }

        public bool Save(string configName)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement newChild = doc.CreateElement("configuration");
            doc.AppendChild(newChild);
            foreach (DictionaryEntry entry in this.m_values)
            {
                Hashtable hashtable = entry.Value as Hashtable;
                if (hashtable != null)
                {
                    string sectionName = RegularSectionName(entry.Key as string);
                    if (!sectionName.StartsWith("/#"))
                    {
                        XmlNode node = CreateXmlDocumentSection(doc, sectionName);
                        foreach (DictionaryEntry entry2 in hashtable)
                        {
                            ConfigValueItem item = entry2.Value as ConfigValueItem;
                            XmlElement element2 = doc.CreateElement("add");
                            element2.SetAttribute("key", entry2.Key as string);
                            element2.SetAttribute("value", item.Value);
                            element2.SetAttribute("description", item.Description);
                            node.AppendChild(element2);
                        }
                    }
                }
            }
            doc.Save(configName);
            return true;
        }

        public void SetValue(string sectionName, string name, string configText)
        {
            this.Add(sectionName, name, configText);
        }

        public void SetValue(string sectionName, string name, string configText, string description)
        {
            this.Add(sectionName, name, configText, description);
        }

        public void SetValues(string sectionName, Hashtable values)
        {
            this.Add(sectionName, values);
        }

        public Hashtable this[string sectionName]
        {
            get
            {
                return this.GetValues(sectionName);
            }
            set
            {
                this.Add(sectionName, value);
            }
        }

        public object this[string sectionName, string name, Type type]
        {
            get
            {
                return this.GetValue(sectionName, name, type);
            }
            set
            {
                this.Add(sectionName, name, value as string);
            }
        }

        public string this[string sectionName, string name]
        {
            get
            {
                return this.GetValue(sectionName, name);
            }
            set
            {
                this.Add(sectionName, name, value);
            }
        }

        public Hashtable Values
        {
            get
            {
                return this.m_values;
            }
        }
    }
}
