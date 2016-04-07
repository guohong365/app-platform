namespace Platform.Tracing
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class DebugShow : Form
    {
        private Container components;
        private RichTextBox richTextBox1;

        public DebugShow()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            this.richTextBox1 = new RichTextBox();
            base.SuspendLayout();
            this.richTextBox1.Location = new Point(0x10, 0x18);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new Size(0x100, 0xe0);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(0x124, 0x111);
            base.Controls.Add(this.richTextBox1);
            base.Name = "DebugShow";
            this.Text = "DebugShow";
            base.ResumeLayout(false);
        }

        public static void Show(string msg)
        {
            DebugShow show = new DebugShow();
            show.richTextBox1.Text = msg;
            show.ShowDialog();
        }
    }
}
