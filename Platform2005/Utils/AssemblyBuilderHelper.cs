namespace Platform.Utils
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class AssemblyBuilderHelper
    {
        private static System.Reflection.Emit.AssemblyBuilder m_AssemblyBuilder;
        private static System.Reflection.Emit.ModuleBuilder m_ModuleBuilder;

        static AssemblyBuilderHelper()
        {
            AssemblyName name = new AssemblyName();
            name.Name = "DynamicAssembly " + Guid.NewGuid().ToString();
            m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            string text = "DynamicModule " + Guid.NewGuid().ToString();
            m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(text, text + ".DLL", false);
        }

        public static System.Reflection.Emit.ModuleBuilder CreateModuleBuilder(string moduleName)
        {
            return m_AssemblyBuilder.DefineDynamicModule(moduleName, false);
        }

        public static TypeBuilder CreateTypeBuilder()
        {
            return CreateTypeBuilder("DynamicType " + Guid.NewGuid().ToString(), null);
        }

        public static TypeBuilder CreateTypeBuilder(string typeName)
        {
            return CreateTypeBuilder(typeName, null);
        }

        public static TypeBuilder CreateTypeBuilder(Type parent)
        {
            return CreateTypeBuilder("DynamicType " + Guid.NewGuid().ToString(), parent);
        }

        public static TypeBuilder CreateTypeBuilder(string typeName, Type parent)
        {
            if (parent == null)
            {
                return m_ModuleBuilder.DefineType(typeName, TypeAttributes.Public);
            }
            return m_ModuleBuilder.DefineType(typeName, TypeAttributes.Public, parent);
        }

        public static void SaveAssembly(string fileName)
        {
            m_AssemblyBuilder.Save(fileName);
        }

        public static System.Reflection.Emit.AssemblyBuilder AssemblyBuilder
        {
            get
            {
                return m_AssemblyBuilder;
            }
        }

        public static System.Reflection.Emit.ModuleBuilder ModuleBuilder
        {
            get
            {
                return m_ModuleBuilder;
            }
        }
    }
}
