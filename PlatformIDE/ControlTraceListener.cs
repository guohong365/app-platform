namespace PlatformIDE
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public class ControlTraceListener : TraceListener
    {
        public Control m_Control;

        public ControlTraceListener(Control control)
        {
            this.m_Control = control;
        }

        public override void Write(object o)
        {
            this.m_Control.Text = this.m_Control.Text + o.ToString();
        }

        public override void Write(string message)
        {
            this.m_Control.Text = this.m_Control.Text + message;
        }

        public override void Write(object o, string category)
        {
            this.m_Control.Text = this.m_Control.Text + o.ToString();
        }

        public override void Write(string message, string category)
        {
            this.m_Control.Text = this.m_Control.Text + message;
        }

        public override void WriteLine(object o)
        {
            this.m_Control.Text = this.m_Control.Text + o.ToString() + "\r\n";
        }

        public override void WriteLine(string message)
        {
            this.m_Control.Text = this.m_Control.Text + message + "\r\n";
        }

        public override void WriteLine(object o, string category)
        {
            this.m_Control.Text = this.m_Control.Text + o.ToString() + "\r\n";
        }

        public override void WriteLine(string message, string category)
        {
            this.m_Control.Text = this.m_Control.Text + message + "\r\n";
        }
    }
}
