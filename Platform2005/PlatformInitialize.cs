namespace Platform
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;

    public sealed class PlatformInitialize
    {
        private static ArrayList m_AssemblyPaths = new ArrayList();

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!args.LoadedAssembly.GlobalAssemblyCache)
            {
                string location = args.LoadedAssembly.Location;
                if (location != null)
                {
                    try
                    {
                        string item = Path.GetDirectoryName(location).ToUpper();
                        if (!m_AssemblyPaths.Contains(item))
                        {
                            m_AssemblyPaths.Add(item);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] textArray = args.Name.Split(new char[] { ',' });
            foreach (string text in m_AssemblyPaths)
            {
                string path = text + @"\" + textArray[0] + ".dll";
                if (File.Exists(path))
                {
                    return Assembly.LoadFile(path);
                }
            }
            return null;
        }

        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(PlatformInitialize.CurrentDomain_AssemblyLoad);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(PlatformInitialize.CurrentDomain_AssemblyResolve);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.GlobalAssemblyCache)
                {
                    string location = assembly.Location;
                    if (location != null)
                    {
                        try
                        {
                            string item = Path.GetDirectoryName(location).ToUpper();
                            if (m_AssemblyPaths.Contains(item))
                            {
                                break;
                            }
                            m_AssemblyPaths.Add(item);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}
