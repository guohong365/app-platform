namespace Platform
{
    using System;
    using System.IO;
    using System.Text;

    public sealed class MultiTextWriter : TextWriter
    {
        public TextWriter[] m_Writers;

        public MultiTextWriter(TextWriter[] writers)
        {
            this.m_Writers = writers;
        }

        public override void Close()
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Flush();
                writer.Close();
            }
        }

        public override void Write(bool value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(char value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(decimal value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(double value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(int value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(long value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(object value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(float value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(string value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(char[] buffer)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(buffer);
                writer.Flush();
            }
        }

        public override void Write(uint value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(ulong value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(value);
                writer.Flush();
            }
        }

        public override void Write(string format, params object[] arg)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(format, arg);
                writer.Flush();
            }
        }

        public override void Write(string format, object arg0)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(format, arg0);
                writer.Flush();
            }
        }

        public override void Write(string format, object arg0, object arg1)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(format, arg0, arg1);
                writer.Flush();
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(buffer, index, count);
                writer.Flush();
            }
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.Write(format, arg0, arg1, arg2);
                writer.Flush();
            }
        }

        public override void WriteLine()
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine();
                writer.Flush();
            }
        }

        public override void WriteLine(bool value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(char value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(decimal value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(double value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(int value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(long value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(object value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(float value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(string value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(char[] buffer)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(buffer);
                writer.Flush();
            }
        }

        public override void WriteLine(uint value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(ulong value)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(value);
                writer.Flush();
            }
        }

        public override void WriteLine(string format, params object[] arg)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(format, arg);
                writer.Flush();
            }
        }

        public override void WriteLine(string format, object arg0)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(format, arg0);
                writer.Flush();
            }
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(format, arg0, arg1);
                writer.Flush();
            }
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(buffer, index, count);
                writer.Flush();
            }
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            foreach (TextWriter writer in this.m_Writers)
            {
                writer.WriteLine(format, arg0, arg1, arg2);
                writer.Flush();
            }
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.Unicode;
            }
        }
    }
}
