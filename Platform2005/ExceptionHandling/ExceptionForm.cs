namespace Platform.ExceptionHandling
{
    using Platform.Utils;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [StackTracePass]
    public class ExceptionForm : Form
    {
        private Button button_Abort;
        private Button button_Exit;
        private Button button_Ignore;
        private Button button_Retry;
        private Button[] buttons;
        private Panel panel_Control;
        private Panel panel1;
        private PictureBox pictureBox1;
        private PropertyGrid propertyGrid_Exception;
        private RichTextBox richTextBox_MsgOut;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;

        private ExceptionForm()
        {
            this.InitializeComponent();
            base.DialogResult = DialogResult.Abort;
            this.buttons = new Button[] { this.button_Abort, this.button_Ignore, this.button_Exit, this.button_Retry };
        }

        private void button_Abort_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Abort;
            base.Close();
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Abort;
            Application.Exit();
            base.Close();
        }

        private void button_Ignore_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Ignore;
            base.Close();
        }

        private void button_Retry_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Retry;
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void ExceptionForm_Load(object sender, EventArgs e)
        {
            int width = this.button_Abort.Width;
            int num2 = 0;
            for (int i = 0; i < this.buttons.Length; i++)
            {
                if (this.buttons[i].Visible)
                {
                    num2++;
                }
            }
            int num5 = (this.panel_Control.Width - (width * num2)) / (num2 + 1);
            num2 = 0;
            for (int j = 0; j < this.buttons.Length; j++)
            {
                if (this.buttons[j].Visible)
                {
                    this.buttons[j].Left = (num2 * (num5 + width)) + num5;
                    this.buttons[j].Width = width;
                    num2++;
                }
            }
        }

        private void InitializeComponent()
        {
            this.button_Abort = new Button();
            this.panel_Control = new Panel();
            this.button_Exit = new Button();
            this.button_Retry = new Button();
            this.button_Ignore = new Button();
            this.tabControl1 = new TabControl();
            this.tabPage1 = new TabPage();
            this.richTextBox_MsgOut = new RichTextBox();
            this.tabPage2 = new TabPage();
            this.propertyGrid_Exception = new PropertyGrid();
            this.panel1 = new Panel();
            this.pictureBox1 = new PictureBox();
            this.panel_Control.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((ISupportInitialize)this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.button_Abort.FlatStyle = FlatStyle.Popup;
            this.button_Abort.ForeColor = Color.Red;
            this.button_Abort.Location = new Point(0x11, 8);
            this.button_Abort.Name = "button_Abort";
            this.button_Abort.Size = new Size(0x48, 0x18);
            this.button_Abort.TabIndex = 4;
            this.button_Abort.Text = "终止(&X)";
            this.button_Abort.Click += new EventHandler(this.button_Abort_Click);
            this.panel_Control.Controls.Add(this.button_Exit);
            this.panel_Control.Controls.Add(this.button_Retry);
            this.panel_Control.Controls.Add(this.button_Ignore);
            this.panel_Control.Controls.Add(this.button_Abort);
            this.panel_Control.Dock = DockStyle.Bottom;
            this.panel_Control.Location = new Point(0x80, 0xff);
            this.panel_Control.Name = "panel_Control";
            this.panel_Control.Size = new Size(0x15a, 40);
            this.panel_Control.TabIndex = 5;
            this.button_Exit.FlatStyle = FlatStyle.Popup;
            this.button_Exit.ForeColor = Color.Red;
            this.button_Exit.Location = new Point(0x101, 8);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new Size(0x48, 0x18);
            this.button_Exit.TabIndex = 7;
            this.button_Exit.Text = "退出(&T)";
            this.button_Exit.Click += new EventHandler(this.button_Exit_Click);
            this.button_Retry.FlatStyle = FlatStyle.Popup;
            this.button_Retry.ForeColor = Color.Green;
            this.button_Retry.Location = new Point(0xb1, 8);
            this.button_Retry.Name = "button_Retry";
            this.button_Retry.Size = new Size(0x48, 0x18);
            this.button_Retry.TabIndex = 6;
            this.button_Retry.Text = "重试(&R)";
            this.button_Retry.Click += new EventHandler(this.button_Retry_Click);
            this.button_Ignore.FlatStyle = FlatStyle.Popup;
            this.button_Ignore.ForeColor = Color.Brown;
            this.button_Ignore.Location = new Point(0x61, 8);
            this.button_Ignore.Name = "button_Ignore";
            this.button_Ignore.Size = new Size(0x48, 0x18);
            this.button_Ignore.TabIndex = 5;
            this.button_Ignore.Text = "忽略(&P)";
            this.button_Ignore.Click += new EventHandler(this.button_Ignore_Click);
            this.tabControl1.Appearance = TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Location = new Point(0x80, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(0x15a, 0xff);
            this.tabControl1.TabIndex = 9;
            this.tabPage1.Controls.Add(this.richTextBox_MsgOut);
            this.tabPage1.Location = new Point(4, 0x18);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new Size(0x152, 0xe3);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "异常信息";
            this.richTextBox_MsgOut.BackColor = SystemColors.Control;
            this.richTextBox_MsgOut.Dock = DockStyle.Fill;
            this.richTextBox_MsgOut.ForeColor = Color.Red;
            this.richTextBox_MsgOut.Location = new Point(0, 0);
            this.richTextBox_MsgOut.Name = "richTextBox_MsgOut";
            this.richTextBox_MsgOut.ReadOnly = true;
            this.richTextBox_MsgOut.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox_MsgOut.Size = new Size(0x152, 0xe3);
            this.richTextBox_MsgOut.TabIndex = 0;
            this.richTextBox_MsgOut.Text = "";
            this.tabPage2.Controls.Add(this.propertyGrid_Exception);
            this.tabPage2.Location = new Point(4, 0x18);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new Size(0x236, 0x15b);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "详细信息";
            this.propertyGrid_Exception.Dock = DockStyle.Fill;
            this.propertyGrid_Exception.HelpVisible = false;
            this.propertyGrid_Exception.LineColor = SystemColors.ScrollBar;
            this.propertyGrid_Exception.Location = new Point(0, 0);
            this.propertyGrid_Exception.Name = "propertyGrid_Exception";
            this.propertyGrid_Exception.Size = new Size(0x236, 0x15b);
            this.propertyGrid_Exception.TabIndex = 0;
            this.propertyGrid_Exception.ToolbarVisible = false;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = DockStyle.Left;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x80, 0x127);
            this.panel1.TabIndex = 10;
            this.panel1.Visible = false;
            this.pictureBox1.Dock = DockStyle.Bottom;
            this.pictureBox1.Location = new Point(0, 0x100);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x80, 0x27);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(0x1da, 0x127);
            base.ControlBox = false;
            base.Controls.Add(this.tabControl1);
            base.Controls.Add(this.panel_Control);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ExceptionForm";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "严重异常！！！";
            base.Load += new EventHandler(this.ExceptionForm_Load);
            this.panel_Control.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((ISupportInitialize)this.pictureBox1).EndInit();
            base.ResumeLayout(false);
        }

        public static DialogResult ShowException(Exception exp)
        {
            return ShowException(exp, true, true, true, false, "异常信息");
        }

        public static DialogResult ShowException(Exception exp, string caption)
        {
            return ShowException(exp, true, true, true, false, caption);
        }

        public static DialogResult ShowException(Exception exp, bool showAbort, bool showRetry, bool showIgnore)
        {
            return ShowException(exp, showAbort, showRetry, showIgnore, false, "异常信息");
        }

        public static DialogResult ShowException(Exception exp, bool showAbort, bool showRetry, bool showIgnore, string caption)
        {
            return ShowException(exp, showAbort, showRetry, showIgnore, false, caption);
        }

        public static DialogResult ShowException(Exception exp, bool showAbort, bool showRetry, bool showIgnore, bool showExit, string caption)
        {
            ExceptionHelper.HandleException(exp);
            ExceptionForm form = new ExceptionForm();
            form.button_Abort.Visible = showAbort;
            form.button_Retry.Visible = showRetry;
            form.button_Ignore.Visible = showIgnore;
            form.button_Exit.Visible = showExit;
            if (caption != null)
            {
                form.Text = caption;
            }
            form.propertyGrid_Exception.SelectedObject = exp;
            string text2 = (("异常信息：" + exp.Message) + "\r\n异常来源：" + exp.Source) + "\r\n内部异常：" + ((exp.InnerException == null) ? "无" : exp.InnerException.ToString());
            string text = text2 + "\r\n异常堆栈：" + StackTraceUtility.GetStack() + "\r\n\r\n" + exp.StackTrace;
            while (exp != null)
            {
                text = ((text + "\r\n异常原因：" + exp.Message) + "\r\n异常类型：" + exp.GetType()) + "\r\n";
                exp = exp.InnerException;
            }
            form.richTextBox_MsgOut.Text = text;
            return form.ShowDialog();
        }
    }
}
