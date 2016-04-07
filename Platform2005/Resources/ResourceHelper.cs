namespace Platform.Resources
{
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Resources;

    public sealed class ResourceHelper
    {
        private static Hashtable m_Values = new Hashtable();

        public static void Clear()
        {
            m_Values.Clear();
        }

        public static Image GetImage(string name)
        {
            return (m_Values[name] as Image);
        }

        public static int GetInt32(string name)
        {
            try
            {
                return (int) m_Values[name];
            }
            catch
            {
                return 0;
            }
        }

        public static string GetString(string name)
        {
            return (m_Values[name] as string);
        }

        public static object GetValue(string name)
        {
            return m_Values[name];
        }

        public static int Open(string resourceFileName)
        {
            if (resourceFileName == null)
            {
                return -2;
            }
            if (File.Exists(resourceFileName))
            {
                return -2;
            }
            string extension = Path.GetExtension(resourceFileName.ToLower());
            try
            {
                IResourceReader reader = null;
                if (extension == ".resources")
                {
                    reader = new ResourceReader(resourceFileName);
                }
                else if (extension == ".resx")
                {
                    reader = new ResXResourceReader(resourceFileName);
                }
                else
                {
                    return -2;
                }
                if (reader != null)
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        m_Values[entry.Key] = entry.Value;
                    }
                    reader.Close();
                    return 0;
                }
                return -2;
            }
            catch
            {
                return -3;
            }
        }

        public static int Open(Assembly asm, string resourceName)
        {
            if ((asm == null) || (resourceName == null))
            {
                return -4;
            }
            try
            {
                Stream manifestResourceStream = Platform.Utils.ResourceUtility.GetManifestResourceStream(resourceName, asm);
                if (manifestResourceStream == null)
                {
                    manifestResourceStream = Platform.Utils.ResourceUtility.GetManifestResourceStream(resourceName + ".resources", asm);
                }
                if (manifestResourceStream == null)
                {
                    return -2;
                }
                ResourceSet set = new ResourceSet(manifestResourceStream);
                foreach (DictionaryEntry entry in set)
                {
                    m_Values[entry.Key] = entry.Value;
                }
                manifestResourceStream.Close();
                set.Close();
                return 0;
            }
            catch
            {
                return -3;
            }
        }

        public static int Save(string fileName)
        {
            try
            {
                string text = Path.GetExtension(fileName).ToLower();
                IResourceWriter writer = null;
                if (text == ".resources")
                {
                    writer = new ResourceWriter(fileName);
                }
                else if (text == ".resx")
                {
                    writer = new ResXResourceWriter(fileName);
                }
                else
                {
                    return -2;
                }
                if (writer != null)
                {
                    foreach (DictionaryEntry entry in m_Values)
                    {
                        writer.AddResource((string) entry.Key, entry.Value);
                    }
                    writer.Close();
                }
                return 0;
            }
            catch
            {
                return -3;
            }
        }

        public static bool SetValue(string name, object value)
        {
            if (!m_Values.ContainsKey(name))
            {
                return false;
            }
            m_Values[name] = value;
            return true;
        }
    }
}
