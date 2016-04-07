namespace PlatformIDE.NavBars
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class CopyDllControl : UserControl
    {
        private Button button_Copy;
        private Button button_GetDestPath;
        private Button button_GetSourcePath;
        private ColumnHeader columnHeader1;
        private Container components = null;
        private Label label1;
        private Label label2;
        private ListView listView_FileList;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private TextBox textBox_DestPath;
        private TextBox textBox_SourcePath;

        public CopyDllControl()
        {
            this.InitializeComponent();
        }

        private void button_Copy_Click(object sender, EventArgs e)
        {
            string fullPath = Path.GetFullPath(this.textBox_DestPath.Text);
            string path = Path.GetFullPath(this.textBox_SourcePath.Text);
            Directory.CreateDirectory(fullPath);
            Console.WriteLine("源目录：" + path);
            Console.WriteLine("目标目录：" + fullPath);
            string[] directories = Directory.GetDirectories(path);
            Hashtable filecol = new Hashtable();
            foreach (string text3 in directories)
            {
                string text4 = text3 + @"\bin";
                this.GetCopyFile(text4, "*.exe", filecol);
                this.GetCopyFile(text4, "*.dll", filecol);
                this.GetCopyFile(text4, "*.xml", filecol);
                this.GetCopyFile(text4, "*.config", filecol);
            }
            this.Copy(filecol, fullPath);
        }

        private void button_GetDestPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path.GetFullPath(this.textBox_DestPath.Text);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_DestPath.Text = dialog.SelectedPath;
            }
        }

        private void button_GetSourcePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path.GetFullPath(this.textBox_SourcePath.Text);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_SourcePath.Text = dialog.SelectedPath;
            }
        }

        private void Copy(Hashtable files, string destPath)
        {
            foreach (DictionaryEntry entry in files)
            {
                string sourceFileName = (string)entry.Value;
                string key = (string)entry.Key;
                Console.Write("正在拷贝： " + sourceFileName);
                try
                {
                    File.Copy(sourceFileName, destPath + @"\" + key, true);
                    Console.WriteLine(" 成功！");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(" 错误！" + exception.Message);
                }
            }
        }

        private void CopyDllControl_SizeChanged(object sender, EventArgs e)
        {
            this.columnHeader1.Width = this.listView_FileList.Width - 0x19;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GetCopyFile(string path, string pattern, Hashtable filecol)
        {
            try
            {
                string[] files = Directory.GetFiles(path, pattern);
                foreach (string text in files)
                {
                    string fileName = Path.GetFileName(text);
                    if (!filecol.ContainsKey(fileName))
                    {
                        filecol[fileName] = text;
                    }
                }
            }
            catch
            {
            }
        }

        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.label1 = new Label();
            this.textBox_SourcePath = new TextBox();
            this.button_GetSourcePath = new Button();
            this.panel2 = new Panel();
            this.textBox_DestPath = new TextBox();
            this.button_GetDestPath = new Button();
            this.label2 = new Label();
            this.panel3 = new Panel();
            this.listView_FileList = new ListView();
            this.columnHeader1 = new ColumnHeader();
            this.button_Copy = new Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.textBox_SourcePath);
            this.panel1.Controls.Add(this.button_GetSourcePath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0xc0, 0x15);
            this.panel1.TabIndex = 0;
            this.label1.Dock = DockStyle.Left;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x48, 0x15);
            this.label1.TabIndex = 0;
            this.label1.Text = "源目录：";
            this.label1.TextAlign = ContentAlignment.MiddleRight;
            this.textBox_SourcePath.Dock = DockStyle.Fill;
            this.textBox_SourcePath.Location = new Point(0x48, 0);
            this.textBox_SourcePath.Name = "textBox_SourcePath";
            this.textBox_SourcePath.Size = new Size(0x60, 0x15);
            this.textBox_SourcePath.TabIndex = 1;
            this.textBox_SourcePath.Text = @"..\";
            this.button_GetSourcePath.Dock = DockStyle.Right;
            this.button_GetSourcePath.FlatStyle = FlatStyle.Popup;
            this.button_GetSourcePath.Location = new Point(0xa8, 0);
            this.button_GetSourcePath.Name = "button_GetSourcePath";
            this.button_GetSourcePath.Size = new Size(0x18, 0x15);
            this.button_GetSourcePath.TabIndex = 2;
            this.button_GetSourcePath.Text = "…";
            this.button_GetSourcePath.Click += new EventHandler(this.button_GetSourcePath_Click);
            this.panel2.Controls.Add(this.textBox_DestPath);
            this.panel2.Controls.Add(this.button_GetDestPath);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = DockStyle.Top;
            this.panel2.Location = new Point(0, 0x15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0xc0, 0x15);
            this.panel2.TabIndex = 1;
            this.textBox_DestPath.Dock = DockStyle.Fill;
            this.textBox_DestPath.Location = new Point(0x48, 0);
            this.textBox_DestPath.Name = "textBox_DestPath";
            this.textBox_DestPath.Size = new Size(0x60, 0x15);
            this.textBox_DestPath.TabIndex = 1;
            this.textBox_DestPath.Text = @"..\Output";
            this.button_GetDestPath.Dock = DockStyle.Right;
            this.button_GetDestPath.FlatStyle = FlatStyle.Popup;
            this.button_GetDestPath.Location = new Point(0xa8, 0);
            this.button_GetDestPath.Name = "button_GetDestPath";
            this.button_GetDestPath.Size = new Size(0x18, 0x15);
            this.button_GetDestPath.TabIndex = 2;
            this.button_GetDestPath.Text = "…";
            this.button_GetDestPath.Click += new EventHandler(this.button_GetDestPath_Click);
            this.label2.Dock = DockStyle.Left;
            this.label2.Location = new Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x48, 0x15);
            this.label2.TabIndex = 0;
            this.label2.Text = "目标目录：";
            this.label2.TextAlign = ContentAlignment.MiddleRight;
            this.panel3.Controls.Add(this.button_Copy);
            this.panel3.Dock = DockStyle.Bottom;
            this.panel3.Location = new Point(0, 0x178);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(0xc0, 40);
            this.panel3.TabIndex = 2;
            this.listView_FileList.BorderStyle = BorderStyle.None;
            this.listView_FileList.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
            this.listView_FileList.Dock = DockStyle.Fill;
            this.listView_FileList.GridLines = true;
            this.listView_FileList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listView_FileList.Location = new Point(0, 0x2a);
            this.listView_FileList.Name = "listView_FileList";
            this.listView_FileList.Size = new Size(0xc0, 0x14e);
            this.listView_FileList.TabIndex = 3;
            this.listView_FileList.View = View.Details;
            this.columnHeader1.Text = "文件";
            this.columnHeader1.Width = 0xa7;
            this.button_Copy.FlatStyle = FlatStyle.Popup;
            this.button_Copy.Location = new Point(0x80, 8);
            this.button_Copy.Name = "button_Copy";
            this.button_Copy.Size = new Size(0x30, 0x18);
            this.button_Copy.TabIndex = 0;
            this.button_Copy.Text = "拷贝";
            this.button_Copy.Click += new EventHandler(this.button_Copy_Click);
            base.Controls.Add(this.listView_FileList);
            base.Controls.Add(this.panel3);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.panel1);
            base.Name = "CopyDllControl";
            base.Size = new Size(0xc0, 0x1a0);
            base.SizeChanged += new EventHandler(this.CopyDllControl_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}
