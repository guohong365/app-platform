namespace Platform.Tracing
{
    using System;
    using System.Diagnostics;

    public sealed class ConsoleListener : TraceListener
    {
        public override void Write(object o)
        {
            Console.Write(o);
        }

        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void Write(object o, string category)
        {
            Console.Write(o);
        }

        public override void Write(string message, string category)
        {
            Console.Write(message);
        }

        public override void WriteLine(object o)
        {
            Console.WriteLine(o);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public override void WriteLine(object o, string category)
        {
            Console.WriteLine(o);
        }

        public override void WriteLine(string message, string category)
        {
            Console.WriteLine(message);
        }
    }
}
