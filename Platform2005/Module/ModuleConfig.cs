namespace Platform.Module
{
    using Platform.Configuration;
    using Platform.IO;
    using System;
    using System.IO;

    public sealed class ModuleConfig
    {
        private static string[] m_ModulePaths = null;
        [ConfigItem("/PlatformSettings", "ModulePaths", null)]
        private static string ModulePaths = ".";

        private static void AfterInitialize()
        {
            if (ModulePaths != null)
            {
                m_ModulePaths = ModulePaths.Split(new char[] { ';' });
            }
        }

        public static string GetModuleFullPath(string basePath, string moduleFile)
        {
            if (moduleFile != null)
            {
                moduleFile = moduleFile.Trim();
                if (moduleFile == "")
                {
                    return null;
                }
                moduleFile = moduleFile.Replace("/", @"\");
                if (Path.IsPathRooted(moduleFile))
                {
                    moduleFile = Path.GetFullPath(moduleFile);
                    if (File.Exists(moduleFile))
                    {
                        return moduleFile;
                    }
                    return null;
                }
                if (m_ModulePaths != null)
                {
                    foreach (string text in m_ModulePaths)
                    {
                        string fullPath = PathUtility.GetFullPath(text + @"\" + moduleFile, basePath);
                        if (File.Exists(fullPath))
                        {
                            return fullPath;
                        }
                    }
                    moduleFile = Path.GetFileName(moduleFile);
                    foreach (string text3 in m_ModulePaths)
                    {
                        string path = PathUtility.GetFullPath(text3 + @"\" + moduleFile, basePath);
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }
            return null;
        }
    }
}
