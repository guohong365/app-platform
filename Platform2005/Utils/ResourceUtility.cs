namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;

    public class ResourceUtility
    {
        private static Hashtable m_AssemblyResourceNames = new Hashtable();

        public static string GetManifestResourceName(string resourceName, Assembly asm)
        {
            if ((resourceName != null) && (asm != null))
            {
                string[] manifestResourceNames = m_AssemblyResourceNames[asm] as string[];
                if (manifestResourceNames == null)
                {
                    manifestResourceNames = asm.GetManifestResourceNames();
                    m_AssemblyResourceNames[asm] = manifestResourceNames;
                }
                if (manifestResourceNames != null)
                {
                    foreach (string text in manifestResourceNames)
                    {
                        if (text == resourceName)
                        {
                            return text;
                        }
                    }
                    foreach (string text2 in manifestResourceNames)
                    {
                        int index = text2.IndexOf('.');
                        if ((index >= 0) && (text2.Substring(index + 1) == resourceName))
                        {
                            return text2;
                        }
                    }
                }
            }
            return null;
        }

        public static Stream GetManifestResourceStream(string resourceName, Assembly asm)
        {
            string manifestResourceName = GetManifestResourceName(resourceName, asm);
            if (manifestResourceName == null)
            {
                return null;
            }
            return asm.GetManifestResourceStream(manifestResourceName);
        }
    }
}
