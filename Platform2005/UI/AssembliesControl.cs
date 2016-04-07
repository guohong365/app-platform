namespace Platform.UI
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    public class AssembliesControl : UserControl
    {
        private Button button_Refresh;
        private Container components = null;
        private Panel panel1;
        private RichTextBox richTextBox_OutPut;

        public AssembliesControl()
        {
            this.InitializeComponent();
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.richTextBox_OutPut.Text = "";
                string text = "";
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    text = ((text + "\r\n--------------------") + "\r\n程序集名称：" + assembly.FullName) + "\r\n程序集路径：" + assembly.Location;
                }
                this.richTextBox_OutPut.Text = text;
            }
            catch
            {
            }
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
            this.panel1 = new Panel();
            this.richTextBox_OutPut = new RichTextBox();
            this.button_Refresh = new Button();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.button_Refresh);
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Location = new Point(0, 0xf1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x128, 0x17);
            this.panel1.TabIndex = 0;
            this.richTextBox_OutPut.BackColor = SystemColors.Control;
            this.richTextBox_OutPut.Dock = DockStyle.Fill;
            this.richTextBox_OutPut.Location = new Point(0, 0);
            this.richTextBox_OutPut.Name = "richTextBox_OutPut";
            this.richTextBox_OutPut.ReadOnly = true;
            this.richTextBox_OutPut.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox_OutPut.Size = new Size(0x128, 0xf1);
            this.richTextBox_OutPut.TabIndex = 1;
            this.richTextBox_OutPut.Text = "";
            this.button_Refresh.Dock = DockStyle.Right;
            this.button_Refresh.FlatStyle = FlatStyle.Popup;
            this.button_Refresh.Location = new Point(0xdd, 0);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.TabIndex = 0;
            this.button_Refresh.Text = "刷新";
            this.button_Refresh.Click += new EventHandler(this.button_Refresh_Click);
            base.Controls.Add(this.richTextBox_OutPut);
            base.Controls.Add(this.panel1);
            base.Name = "AssembliesControl";
            base.Size = new Size(0x128, 0x108);
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}
