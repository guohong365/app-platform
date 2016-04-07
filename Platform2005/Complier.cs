namespace Platform
{
    using Microsoft.CSharp;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Reflection;

    public sealed class Complier
    {
        public static Assembly Compile(string[] fileNames)
        {
            return Compile(fileNames, null, null, false, false);
        }

        public static Assembly Compile(string codes)
        {
            return Compile(codes, null, null, false, false);
        }

        public static Assembly Compile(string[] fileNames, string outDllName, string[] referenceAssemblies, bool debug, bool executeAble)
        {
            if (referenceAssemblies == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                ArrayList list = new ArrayList();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.Location != null)
                    {
                        list.Add(assembly.Location);
                    }
                }
                referenceAssemblies = list.ToArray(typeof(string)) as string[];
            }
            CompilerParameters options = new CompilerParameters(referenceAssemblies);
            options.GenerateExecutable = executeAble;
            options.IncludeDebugInformation = debug;
            if (outDllName != null)
            {
                options.GenerateInMemory = false;
                options.OutputAssembly = outDllName;
            }
            else
            {
                options.GenerateInMemory = true;
            }
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromFile(options, fileNames);
            provider.Dispose();
            if (results.Errors.Count >= 1)
            {
                int num = 0;
                int num2 = 0;
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning)
                    {
                        num2++;
                        continue;
                    }
                    num++;
                }
                if (num > 0)
                {
                    return null;
                }
            }
            return results.CompiledAssembly;
        }

        public static Assembly Compile(string codes, string outDllName, string[] referenceAssemblies, bool debug, bool executeAble)
        {
            if (referenceAssemblies == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                ArrayList list = new ArrayList();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.Location != null)
                    {
                        list.Add(assembly.Location);
                    }
                }
                referenceAssemblies = list.ToArray(typeof(string)) as string[];
            }
            CompilerParameters options = new CompilerParameters(referenceAssemblies);
            options.GenerateExecutable = executeAble;
            options.IncludeDebugInformation = debug;
            if (outDllName != null)
            {
                options.GenerateInMemory = false;
                options.OutputAssembly = outDllName;
            }
            else
            {
                options.GenerateInMemory = true;
            }
            CodeDomProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(options, new string[] { codes });
            provider.Dispose();
            if (results.Errors.Count >= 1)
            {
                int num = 0;
                int num2 = 0;
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning)
                    {
                        num2++;
                        continue;
                    }
                    num++;
                }
                if (num > 0)
                {
                    return null;
                }
            }
            return results.CompiledAssembly;
        }
    }
}
