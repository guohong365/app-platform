namespace Platform.Tracing
{
    using System;

    public sealed class TraceHelper
    {
        public static void WriteLine(string logMessage)
        {
            Console.WriteLine(logMessage);
        }

        public static void WriteMessage(string logMessage)
        {
            Console.Write(logMessage);
        }
    }
}
