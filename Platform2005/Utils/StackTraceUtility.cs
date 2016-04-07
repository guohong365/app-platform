namespace Platform.Utils
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [StackTracePass]
    public sealed class StackTraceUtility
    {
        private static Type m_DefaultIgnoreType = typeof(StackTracePassAttribute);

        public static string GetStack()
        {
            return GetStack(new StackTrace(true), null);
        }

        public static string GetStack(Exception exp)
        {
            return GetStack(new StackTrace(exp, true), null);
        }

        public static string GetStack(params Type[] ignoreAttributesTypes)
        {
            return GetStack(new StackTrace(true), ignoreAttributesTypes);
        }

        private static string GetStack(StackTrace stack, params Type[] ignoreAttributesTypes)
        {
            string text = "";
            int frameCount = stack.FrameCount;
            bool flag = false;
            for (int i = 0; i < frameCount; i++)
            {
                StackFrame frame = stack.GetFrame(i);
                MethodBase method = frame.GetMethod();
                flag = false;
                object[] customAttributes = method.GetCustomAttributes(m_DefaultIgnoreType, false);
                object[] objArray2 = method.DeclaringType.GetCustomAttributes(m_DefaultIgnoreType, false);
                if ((customAttributes.Length <= 0) && (objArray2.Length <= 0))
                {
                    if (ignoreAttributesTypes != null)
                    {
                        foreach (Type type in ignoreAttributesTypes)
                        {
                            customAttributes = method.GetCustomAttributes(type, false);
                            objArray2 = method.DeclaringType.GetCustomAttributes(type, false);
                            if ((customAttributes.Length > 0) || (objArray2.Length > 0))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            continue;
                        }
                    }
                    string text3 = text;
                    text = text3 + "   дк" + method.DeclaringType.FullName + "::" + method.ToString();
                    string fileName = frame.GetFileName();
                    if (fileName != null)
                    {
                        object obj2 = text;
                        text = string.Concat(new object[] { obj2, " ", fileName, "(", frame.GetFileLineNumber(), ",", frame.GetFileColumnNumber(), ")" });
                    }
                    if (i < (frameCount - 1))
                    {
                        text = text + "\r\n";
                    }
                }
            }
            return text;
        }

        public static string GetStack(Exception exp, params Type[] ignoreAttributesTypes)
        {
            return GetStack(new StackTrace(exp, true), ignoreAttributesTypes);
        }
    }
}
