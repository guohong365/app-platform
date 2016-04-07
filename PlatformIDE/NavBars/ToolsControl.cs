namespace PlatformIDE.NavBars
{
    using Platform.Security;
    using Platform.Utils;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class ToolsControl : UserControl
    {
        private Button button_CreateGuid;
        private Container components = null;
        private Label label1;
        private Panel panel_CalSha1;
        private Panel panel1;

        public ToolsControl()
        {
            this.InitializeComponent();
        }

        private void button_CreateGuid_Click(object sender, EventArgs e)
        {
            string text = Guid.NewGuid().ToString("B").ToUpper();
            Console.WriteLine(text);
            Clipboard.SetDataObject(text);
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
            this.button_CreateGuid = new Button();
            this.panel_CalSha1 = new Panel();
            this.label1 = new Label();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.button_CreateGuid);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0xa8, 0x18);
            this.panel1.TabIndex = 0;
            this.button_CreateGuid.Dock = DockStyle.Right;
            this.button_CreateGuid.FlatStyle = FlatStyle.Popup;
            this.button_CreateGuid.Location = new Point(80, 0);
            this.button_CreateGuid.Name = "button_CreateGuid";
            this.button_CreateGuid.Size = new Size(0x58, 0x18);
            this.button_CreateGuid.TabIndex = 4;
            this.button_CreateGuid.Text = "创建接口ID";
            this.button_CreateGuid.Click += new EventHandler(this.button_CreateGuid_Click);
            this.panel_CalSha1.AllowDrop = true;
            this.panel_CalSha1.BorderStyle = BorderStyle.FixedSingle;
            this.panel_CalSha1.Dock = DockStyle.Top;
            this.panel_CalSha1.Location = new Point(0, 0x2f);
            this.panel_CalSha1.Name = "panel_CalSha1";
            this.panel_CalSha1.Size = new Size(0xa8, 0x48);
            this.panel_CalSha1.TabIndex = 1;
            this.panel_CalSha1.DragEnter += new DragEventHandler(this.panel_CalSha1_DragEnter);
            this.panel_CalSha1.DragDrop += new DragEventHandler(this.panel_CalSha1_DragDrop);
            this.panel_CalSha1.DragOver += new DragEventHandler(this.panel_CalSha1_DragOver);
            this.label1.Dock = DockStyle.Top;
            this.label1.Location = new Point(0, 0x18);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0xa8, 0x17);
            this.label1.TabIndex = 2;
            this.label1.Text = "计算SHA1";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            base.Controls.Add(this.panel_CalSha1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.panel1);
            base.Name = "ToolsControl";
            base.Size = new Size(0xa8, 0x178);
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void panel_CalSha1_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = e.Data.GetData("FileDrop") as string[];
            if (data == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
                HashUtility utility = new HashUtility(HashType.SHA1);
                string text = "";
                foreach (string text2 in data)
                {
                    try
                    {
                        string text3 = ByteUtility.BytesToHexString(utility.GetFileHash(text2));
                        string text4 = text;
                        text = text4 + text2 + " ==== " + text3 + "\r\n";
                    }
                    catch
                    {
                    }
                }
                Console.WriteLine(text);
                Clipboard.SetDataObject(text);
            }
        }

        private void panel_CalSha1_DragEnter(object sender, DragEventArgs e)
        {
            string[] data = e.Data.GetData("FileDrop") as string[];
            if (data == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void panel_CalSha1_DragOver(object sender, DragEventArgs e)
        {
            string[] data = e.Data.GetData("FileDrop") as string[];
            if (data == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }
    }
}
