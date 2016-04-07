namespace Platform.Module
{
    using Platform.ExceptionHandling;
    using Platform.Log;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public sealed class PlatformModuleLoader
    {
        private static ArrayList m_Assemblies = new ArrayList();
        private static ArrayList m_AssemblyPaths = new ArrayList();
        private static ArrayList m_ModuleList = new ArrayList();
        private static Hashtable m_ModulesTable = new Hashtable();

        public static event PlatformAssemblyLoadedHandler OnAssemblyLoaded;

        public static event PlatformAssemblyLoadingHandler OnAssemblyLoading;

        public static event PlatformModuleLoadedHandler OnModuleLoaded;

        public static event PlatformModuleLoadingHandler OnModuleLoading;

        public static void CleanModules()
        {
            m_ModulesTable.Clear();
            m_Assemblies.Clear();
            for (int i = 0; i < m_ModuleList.Count; i++)
            {
                IPlatformModule2 module = m_ModuleList[i] as IPlatformModule2;
                if (module != null)
                {
                    try
                    {
                        module.PlatformModuleDispose();
                    }
                    catch
                    {
                    }
                }
            }
            m_ModuleList.Clear();
        }

        private static Assembly GetModuleAssembly(string asmFilePath)
        {
            foreach (Assembly assembly in m_Assemblies)
            {
                if (string.Compare(assembly.Location, asmFilePath, true) == 0)
                {
                    return assembly;
                }
            }
            return null;
        }

        public static IPlatformModule GetModuleByName(string muduleName)
        {
            return (m_ModulesTable[muduleName] as IPlatformModule);
        }

        private static void LoadModuleAssembly(Assembly asm)
        {
            try
            {
                AssemblyModuleAttribute[] customAttributes = asm.GetCustomAttributes(typeof(AssemblyModuleAttribute), false) as AssemblyModuleAttribute[];
                if (customAttributes.Length < 1)
                {
                    PlatformLogSink.Write("加载模块：" + asm.Location + " 失败！未找到模块类型。");
                }
                else
                {
                    foreach (AssemblyModuleAttribute attribute in customAttributes)
                    {
                        IPlatformModule module;
                        if (attribute.Type.GetInterface("Platform.Module.IPlatformModule") == null)
                        {
                            PlatformLogSink.Write("注册模块：" + attribute.ModuleName + " 未实现加载接口！");
                        }
                        else if (m_ModulesTable.ContainsKey(attribute.ModuleName))
                        {
                            PlatformLogSink.Write("注册模块：" + attribute.ModuleName + " 已经加载！");
                        }
                        else if ((OnModuleLoading != null) && !OnModuleLoading(asm, attribute.Type))
                        {
                            PlatformLogSink.Write("注册模块：" + attribute.ModuleName + " OnModuleLoading 执行失败！");
                        }
                        else
                        {
                            module = null;
                            try
                            {
                                module = Activator.CreateInstance(attribute.Type) as IPlatformModule;
                                if (module != null)
                                {
                                    goto Label_0140;
                                }
                            }
                            catch (Exception exception)
                            {
                                PlatformLogSink.Write("注册模块：" + attribute.ModuleName + " 创建实例失败！" + ((exception.InnerException == null) ? exception.Message : exception.InnerException.Message));
                            }
                        }
                        goto Label_01D3;
                    Label_0140:
                        if (module.PlatformModuleInitialize())
                        {
                            m_ModulesTable[attribute.ModuleName] = module;
                            m_ModuleList.Add(module);
                        }
                        else
                        {
                            IDisposable disposable = module as IDisposable;
                            if (disposable != null)
                            {
                                try
                                {
                                    disposable.Dispose();
                                }
                                catch
                                {
                                }
                            }
                            PlatformLogSink.Write("注册模块：" + attribute.Type.FullName + " 失败！");
                            goto Label_01D3;
                        }
                        if (OnModuleLoaded != null)
                        {
                            OnModuleLoaded(asm, module);
                        }
                        PlatformLogSink.Write("注册模块：" + attribute.Type.FullName + " 成功！");
                    Label_01D3: ;
                    }
                    m_Assemblies.Add(asm);
                    PlatformLogSink.Write("加载模块：" + asm.Location + " 成功！");
                }
            }
            catch (Exception exception2)
            {
                PlatformLogSink.Write("加载模块：" + asm.Location + " 失败！" + ((exception2.InnerException == null) ? exception2.Message : exception2.InnerException.Message));
                ExceptionHelper.HandleException(exception2);
            }
        }

        private static void LoadModuleAssembly(string strFileName)
        {
            try
            {
                if ((GetModuleAssembly(strFileName) == null) && ((OnAssemblyLoading == null) || OnAssemblyLoading(strFileName)))
                {
                    Assembly asm = Assembly.LoadFrom(strFileName);
                    if (asm == null)
                    {
                        PlatformLogSink.Write("加载模块文件：" + strFileName + " 失败！");
                    }
                    else if ((OnAssemblyLoaded == null) || OnAssemblyLoaded(asm))
                    {
                        LoadModuleAssembly(asm);
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
            }
        }

        public static void LoadModules(string basePath)
        {
            try
            {
                foreach (ModuleData data in ModuleAssemblies.m_ModuleInfos)
                {
                    string moduleFullPath = ModuleConfig.GetModuleFullPath(basePath, data.Path);
                    if (moduleFullPath == null)
                    {
                        PlatformLogSink.Write("加载模块：" + data.Path + " 失败！无法找到文件！");
                        continue;
                    }
                    LoadModuleAssembly(moduleFullPath);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
            }
        }

        public static ArrayList Assemblies
        {
            get
            {
                return m_Assemblies;
            }
        }

        public static ArrayList ModuleList
        {
            get
            {
                return m_ModuleList;
            }
        }

        public static Hashtable Modules
        {
            get
            {
                return m_ModulesTable;
            }
        }
    }
}
