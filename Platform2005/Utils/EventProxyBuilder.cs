namespace Platform.Utils
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class EventProxyBuilder
    {
        private static MethodInfo m_OnEventMethod = typeof(EventProxySink).GetMethod("OnEventInvoke", new Type[] { typeof(object[]) });
        private static Hashtable m_Types = new Hashtable();

        public static void CreateEventProxy(EventProxySink sink, object eventObject, EventInfo eventInfo)
        {
            if (eventInfo != null)
            {
                object target = Activator.CreateInstance(GetEventProxyType(eventInfo), new object[] { sink });
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, eventInfo.EventHandlerType.FullName, false);
                eventInfo.AddEventHandler(eventObject, handler);
            }
        }

        private static Type CreateEventProxyType(EventInfo eventInfo)
        {
            TypeBuilder builder = AssemblyBuilderHelper.CreateTypeBuilder();
            FieldBuilder field = builder.DefineField("m_Target", typeof(EventProxySink), FieldAttributes.Private);
            ILGenerator iLGenerator = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(EventProxySink) }).GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            MethodInfo method = eventInfo.EventHandlerType.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }
            MethodBuilder builder4 = builder.DefineMethod(eventInfo.EventHandlerType.FullName, MethodAttributes.Private, method.ReturnType, parameterTypes);
            for (int j = 0; j < parameters.Length; j++)
            {
                builder4.DefineParameter(j + 1, parameters[j].Attributes, parameters[j].Name);
            }
            ILGenerator generator2 = builder4.GetILGenerator();
            generator2.DeclareLocal(typeof(object[]));
            generator2.Emit(OpCodes.Ldc_I4, parameters.Length);
            generator2.Emit(OpCodes.Newarr, typeof(object));
            generator2.Emit(OpCodes.Stloc_0);
            for (int k = 0; k < parameters.Length; k++)
            {
                generator2.Emit(OpCodes.Ldloc_0);
                generator2.Emit(OpCodes.Ldc_I4, k);
                generator2.Emit(OpCodes.Ldarg, (int) (k + 1));
                generator2.Emit(OpCodes.Stelem_Ref);
            }
            generator2.Emit(OpCodes.Ldarg_0);
            generator2.Emit(OpCodes.Ldfld, field);
            generator2.Emit(OpCodes.Ldloc_0);
            generator2.Emit(OpCodes.Callvirt, m_OnEventMethod);
            generator2.Emit(OpCodes.Ret);
            return builder.CreateType();
        }

        public static Type GetEventProxyType(EventInfo eventInfo)
        {
            if (eventInfo == null)
            {
                return null;
            }
            lock (m_Types.SyncRoot)
            {
                Type type = m_Types[eventInfo.EventHandlerType] as Type;
                if (type == null)
                {
                    type = CreateEventProxyType(eventInfo);
                    m_Types[eventInfo.EventHandlerType] = type;
                }
                return type;
            }
        }
    }
}
