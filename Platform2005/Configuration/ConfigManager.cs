namespace Platform.Configuration
{
    using Platform.ExceptionHandling;
    using System;
    using System.Collections;
    using System.Data;
    using System.Reflection;

    public sealed class ConfigManager
    {
        private static ArrayList m_FillAssemblies = new ArrayList();
        private static ConfigHelper m_Helper = new ConfigHelper();
        private static ArrayList m_LoadAssemblies = new ArrayList();

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!m_LoadAssemblies.Contains(args.LoadedAssembly))
            {
                LoadConfig(args.LoadedAssembly, false);
            }
            if (!m_FillAssemblies.Contains(args.LoadedAssembly))
            {
                FillConfig(args.LoadedAssembly);
            }
        }

        public static void FillAssembliesConfig()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                FillConfig(assembly);
            }
        }

        public static void FillConfig(Assembly asm)
        {
            if (!m_FillAssemblies.Contains(asm))
            {
                m_FillAssemblies.Add(asm);
                AssemblyConfigTypeAttribute[] customAttributes = asm.GetCustomAttributes(typeof(AssemblyConfigTypeAttribute), false) as AssemblyConfigTypeAttribute[];
                if (customAttributes.Length >= 1)
                {
                    foreach (AssemblyConfigTypeAttribute attribute in customAttributes)
                    {
                        FillConfig(attribute.Type);
                    }
                }
            }
        }

        public static void FillConfig(Type type)
        {
            MethodInfo info = type.GetMethod("BeforeInitialize", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[0], null);
            if (info != null)
            {
                try
                {
                    info.Invoke(null, null);
                }
                catch (Exception exception)
                {
                    ExceptionHelper.HandleException(exception, new string[] { "类型名称--" + type.FullName });
                }
            }
            try
            {
                m_Helper.FillConfig(type);
            }
            catch (Exception exception2)
            {
                ExceptionHelper.HandleException(exception2, new string[] { "类型名称--" + type.FullName });
            }
            MethodInfo info2 = type.GetMethod("AfterInitialize", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[0], null);
            if (info2 != null)
            {
                try
                {
                    info2.Invoke(null, null);
                }
                catch (Exception exception3)
                {
                    ExceptionHelper.HandleException(exception3, new string[] { "类型名称--" + type.FullName });
                }
            }
        }

        public static string GetValue(string sectionName, string name)
        {
            return m_Helper.GetValue(sectionName, name);
        }

        public static Hashtable GetValues(string sectionName)
        {
            return m_Helper.GetValues(sectionName);
        }

        public static void InstallAutoLoadConfig()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(ConfigManager.CurrentDomain_AssemblyLoad);
        }

        public static void LoadAssembliesConfig(bool replaceOlder)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadConfig(assembly, replaceOlder);
            }
        }

        public static void LoadConfig(ConfigHelper helper, bool replaceOlder)
        {
            m_Helper.AddConfigHelper(helper, replaceOlder);
        }

        public static void LoadConfig(Assembly asm, bool replaceOlder)
        {
            if (!m_LoadAssemblies.Contains(asm))
            {
                m_LoadAssemblies.Add(asm);
                m_Helper.LoadConfig(asm, false, replaceOlder);
            }
        }

        public static void LoadConfig(string configFileName, bool replaceOlder)
        {
            m_Helper.LoadConfig(configFileName, false, replaceOlder);
        }

        public static void LoadConfig(DataTable dt, string sectionField, string keyField, string valueField, string descriptionField, bool replaceOlder)
        {
            m_Helper.LoadConfig(dt, sectionField, keyField, valueField, descriptionField, false, replaceOlder);
        }

        public static void RegisterConverter(Type type)
        {
            ConfigConverters.RegisterConverter(type);
        }

        public static void RegisterConverter(string converterName, ConfigConvertCollectionHandler handler)
        {
            ConfigConverters.RegisterConverter(converterName, handler);
        }

        public static void RegisterConverter(string converterName, ConfigConvertHandler handler)
        {
            ConfigConverters.RegisterConverter(converterName, handler);
        }

        public static bool Save(string configName)
        {
            return m_Helper.Save(configName);
        }

        public static void SetValue(string sectionName, string name, string configText)
        {
            m_Helper.SetValue(sectionName, name, configText);
        }

        public static void SetValue(string sectionName, string name, string configText, string description)
        {
            m_Helper.SetValue(sectionName, name, configText, description);
        }

        public static void SetValues(string sectionName, Hashtable values)
        {
            m_Helper.SetValues(sectionName, values);
        }

        public static ConfigHelper Holder
        {
            get
            {
                return m_Helper;
            }
        }
    }
}
