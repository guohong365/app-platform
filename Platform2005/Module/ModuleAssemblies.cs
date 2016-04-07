namespace Platform.Module
{
    using Platform.Configuration;
    using System;
    using System.Collections;

    internal sealed class ModuleAssemblies
    {
        [ConfigCollectionItem("/PlatformModules", "PlatformModulesConverter")]
        public static ArrayList m_ModuleInfos = null;

        private static void AddModuleList(ArrayList list, ModuleData data)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ModuleData data2 = list[i] as ModuleData;
                if ((data2.Order >= data.Order) && (data2.Order > data.Order))
                {
                    list.Insert(i, data);
                    return;
                }
            }
            list.Add(data);
        }

        private static void BeforeInitialize()
        {
            ConfigManager.RegisterConverter(typeof(ModuleAssemblies));
        }

        [ConverterMethod]
        private static object PlatformModulesConverter(Hashtable values, ConfigCollectionItemAttribute attr)
        {
            ArrayList list = new ArrayList();
            foreach (DictionaryEntry entry in values)
            {
                ConfigValueItem item = entry.Value as ConfigValueItem;
                ModuleData moduleData = ModuleData.GetModuleData(entry.Key as string, item.Value);
                if (moduleData != null)
                {
                    AddModuleList(list, moduleData);
                }
            }
            return list;
        }
    }
}
