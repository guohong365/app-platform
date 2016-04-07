namespace PlatformIDE
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public sealed class ControlTextWriter : TextWriter
    {
        public Control m_Control;

        public ControlTextWriter(Control control)
        {
            this.m_Control = control;
        }

        public override void Close()
        {
        }

        public override void Write(bool value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(char value)
        {
            this.m_Control.Text = this.m_Control.Text + value;
        }

        public override void Write(decimal value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(double value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(int value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(long value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(object value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(float value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(string value)
        {
            this.m_Control.Text = this.m_Control.Text + value;
        }

        public override void Write(char[] buffer)
        {
        }

        public override void Write(uint value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(ulong value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString();
        }

        public override void Write(string format, params object[] arg)
        {
        }

        public override void Write(string format, object arg0)
        {
        }

        public override void Write(string format, object arg0, object arg1)
        {
        }

        public override void Write(char[] buffer, int index, int count)
        {
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
        }

        public override void WriteLine()
        {
            this.m_Control.Text = this.m_Control.Text + "\r\n";
        }

        public override void WriteLine(bool value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(char value)
        {
            this.m_Control.Text = this.m_Control.Text + value + "\r\n";
        }

        public override void WriteLine(decimal value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(double value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(int value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(long value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(object value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(float value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(string value)
        {
            this.m_Control.Text = this.m_Control.Text + value + "\r\n";
        }

        public override void WriteLine(char[] buffer)
        {
        }

        public override void WriteLine(uint value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(ulong value)
        {
            this.m_Control.Text = this.m_Control.Text + value.ToString() + "\r\n";
        }

        public override void WriteLine(string format, params object[] arg)
        {
        }

        public override void WriteLine(string format, object arg0)
        {
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
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
