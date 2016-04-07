namespace PlatformIDE.NavBars
{
    using Platform.CSS.Packet;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxItem(true)]
    public class FieldDefineControl : UserControl
    {
        private CheckBox checkBox_Enable;
        private CheckBox checkBox_Fixed;
        private ComboBox comboBox_DataType;
        private Container components;
        private static int index = 0;
        private bool Init;
        public FieldDefine m_Define;
        private TextBox textBox_Length;
        private TextBox textBox_Name;

        public FieldDefineControl()
        {
            this.m_Define = new FieldDefine(index++);
            this.components = null;
            this.Init = false;
            this.InitializeComponent();
            this.comboBox_DataType.DataSource = FieldDefine.DataTypeNames;
            this.checkBox_Enable.Text = (this.m_Define.Index + 1).ToString("D2");
        }

        public FieldDefineControl(IContainer container)
        {
            this.m_Define = new FieldDefine(index++);
            this.components = null;
            this.Init = false;
            container.Add(this);
            this.InitializeComponent();
            this.comboBox_DataType.DataSource = FieldDefine.DataTypeNames;
            this.checkBox_Enable.Text = (this.m_Define.Index + 1).ToString("D2");
        }

        private void checkBox_Enable_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.Init)
            {
                try
                {
                    this.Init = true;
                    this.m_Define.Enable = this.checkBox_Enable.Checked;
                    if (this.checkBox_Enable.Checked)
                    {
                        this.textBox_Name.Enabled = true;
                        this.comboBox_DataType.Enabled = true;
                        int defaultLen = this.m_Define.GetDefaultLen();
                        if (defaultLen == 0)
                        {
                            this.checkBox_Fixed.Enabled = true;
                            this.textBox_Length.Text = this.m_Define.Length.ToString();
                            if (this.m_Define.Length == 0)
                            {
                                this.textBox_Length.Enabled = false;
                                this.checkBox_Fixed.Checked = true;
                            }
                            else
                            {
                                this.textBox_Length.Enabled = true;
                                this.checkBox_Fixed.Checked = false;
                            }
                        }
                        else
                        {
                            this.textBox_Length.Text = defaultLen.ToString();
                            this.m_Define.Length = defaultLen;
                            this.textBox_Length.Enabled = false;
                            this.checkBox_Fixed.Checked = true;
                            this.checkBox_Fixed.Enabled = false;
                        }
                    }
                    else
                    {
                        this.textBox_Name.Enabled = false;
                        this.comboBox_DataType.Enabled = false;
                        this.textBox_Length.Enabled = false;
                        this.checkBox_Fixed.Enabled = false;
                    }
                }
                finally
                {
                    this.Init = false;
                }
            }
        }

        private void checkBox_Fixed_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.Init)
            {
                try
                {
                    this.Init = true;
                    this.m_Define.Fixed = this.checkBox_Fixed.Checked;
                    if (!this.checkBox_Fixed.Checked)
                    {
                        this.textBox_Length.Enabled = false;
                    }
                    else
                    {
                        this.textBox_Length.Enabled = true;
                    }
                }
                finally
                {
                    this.Init = false;
                }
            }
        }

        public void Clear()
        {
            try
            {
                this.m_Define.Clear();
                this.Initialize();
            }
            catch
            {
            }
        }

        private void comboBox_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Init)
            {
                try
                {
                    this.Init = true;
                    this.m_Define.DataType = this.comboBox_DataType.SelectedIndex;
                    int defaultLen = this.m_Define.GetDefaultLen();
                    this.m_Define.Length = defaultLen;
                    this.textBox_Length.Text = defaultLen.ToString();
                    if (defaultLen != 0)
                    {
                        this.textBox_Length.Enabled = false;
                        this.checkBox_Fixed.Checked = true;
                        this.checkBox_Fixed.Enabled = false;
                    }
                    else
                    {
                        this.textBox_Length.Enabled = false;
                        this.checkBox_Fixed.Enabled = true;
                        this.checkBox_Fixed.Checked = false;
                    }
                }
                finally
                {
                    this.Init = false;
                }
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

        public string GetDefine(string head, int count)
        {
            this.m_Define.Enable = this.checkBox_Enable.Checked;
            this.m_Define.Name = this.textBox_Name.Text;
            this.m_Define.DataType = this.comboBox_DataType.SelectedIndex;
            this.m_Define.Fixed = this.checkBox_Fixed.Checked;
            try
            {
                this.m_Define.Length = int.Parse(this.textBox_Length.Text);
            }
            catch
            {
                this.m_Define.Length = 0;
            }
            return this.m_Define.GetDefine(head, count);
        }

        public string GetRelease(string head)
        {
            return this.m_Define.GetRelease(head);
        }

        private void Initialize()
        {
            try
            {
                this.Init = true;
                int defaultLen = this.m_Define.GetDefaultLen();
                if (defaultLen != 0)
                {
                    this.m_Define.Length = defaultLen;
                }
                this.checkBox_Enable.Checked = this.m_Define.Enable;
                this.textBox_Name.Text = this.m_Define.Name;
                this.comboBox_DataType.SelectedIndex = this.m_Define.DataType;
                this.textBox_Length.Text = this.m_Define.Length.ToString();
                this.checkBox_Fixed.Checked = this.m_Define.Fixed;
                if (!this.m_Define.Enable)
                {
                    this.textBox_Name.Enabled = false;
                    this.comboBox_DataType.Enabled = false;
                    this.textBox_Length.Enabled = false;
                    this.checkBox_Fixed.Enabled = false;
                }
                else
                {
                    this.textBox_Name.Enabled = true;
                    this.comboBox_DataType.Enabled = true;
                    if (defaultLen != 0)
                    {
                        this.textBox_Length.Enabled = false;
                        this.checkBox_Fixed.Enabled = false;
                        this.checkBox_Fixed.Checked = true;
                    }
                    else
                    {
                        this.checkBox_Fixed.Enabled = true;
                        this.checkBox_Fixed.Checked = this.m_Define.Fixed;
                        if (this.m_Define.Fixed)
                        {
                            this.textBox_Length.Enabled = true;
                        }
                        else
                        {
                            this.textBox_Length.Enabled = false;
                        }
                    }
                }
            }
            finally
            {
                this.Init = false;
            }
        }

        public void Initialize(PacketFieldAttribute attr)
        {
            try
            {
                this.m_Define.Initialize(attr);
                this.Initialize();
            }
            catch
            {
            }
        }

        public void Initialize(string s)
        {
            try
            {
                this.m_Define.Initialize(s);
                this.Initialize();
            }
            catch
            {
            }
        }

        private void InitializeComponent()
        {
            this.checkBox_Enable = new CheckBox();
            this.textBox_Name = new TextBox();
            this.comboBox_DataType = new ComboBox();
            this.textBox_Length = new TextBox();
            this.checkBox_Fixed = new CheckBox();
            base.SuspendLayout();
            this.checkBox_Enable.CheckAlign = ContentAlignment.BottomLeft;
            this.checkBox_Enable.Dock = DockStyle.Left;
            this.checkBox_Enable.Location = new Point(0, 0);
            this.checkBox_Enable.Name = "checkBox_Enable";
            this.checkBox_Enable.Size = new Size(40, 0x11);
            this.checkBox_Enable.TabIndex = 0;
            this.checkBox_Enable.Text = "00";
            this.checkBox_Enable.TextAlign = ContentAlignment.BottomCenter;
            this.checkBox_Enable.CheckedChanged += new EventHandler(this.checkBox_Enable_CheckedChanged);
            this.textBox_Name.Dock = DockStyle.Fill;
            this.textBox_Name.Enabled = false;
            this.textBox_Name.Location = new Point(40, 0);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new Size(90, 0x15);
            this.textBox_Name.TabIndex = 0;
            this.textBox_Name.Text = "";
            this.textBox_Name.TextChanged += new EventHandler(this.textBox_Name_TextChanged);
            this.comboBox_DataType.Dock = DockStyle.Right;
            this.comboBox_DataType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_DataType.Enabled = false;
            this.comboBox_DataType.Location = new Point(130, 0);
            this.comboBox_DataType.Name = "comboBox_DataType";
            this.comboBox_DataType.Size = new Size(0x48, 20);
            this.comboBox_DataType.TabIndex = 0;
            this.comboBox_DataType.SelectedIndexChanged += new EventHandler(this.comboBox_DataType_SelectedIndexChanged);
            this.textBox_Length.Dock = DockStyle.Right;
            this.textBox_Length.Enabled = false;
            this.textBox_Length.Location = new Point(0xca, 0);
            this.textBox_Length.Name = "textBox_Length";
            this.textBox_Length.Size = new Size(0x20, 0x15);
            this.textBox_Length.TabIndex = 0;
            this.textBox_Length.Text = "";
            this.textBox_Length.TextChanged += new EventHandler(this.textBox_Length_TextChanged);
            this.checkBox_Fixed.CheckAlign = ContentAlignment.BottomLeft;
            this.checkBox_Fixed.Dock = DockStyle.Right;
            this.checkBox_Fixed.Enabled = false;
            this.checkBox_Fixed.Location = new Point(0xea, 0);
            this.checkBox_Fixed.Name = "checkBox_Fixed";
            this.checkBox_Fixed.Size = new Size(0x10, 0x11);
            this.checkBox_Fixed.TabIndex = 0;
            this.checkBox_Fixed.TextAlign = ContentAlignment.BottomCenter;
            this.checkBox_Fixed.CheckedChanged += new EventHandler(this.checkBox_Fixed_CheckedChanged);
            base.Controls.Add(this.textBox_Name);
            base.Controls.Add(this.comboBox_DataType);
            base.Controls.Add(this.checkBox_Enable);
            base.Controls.Add(this.textBox_Length);
            base.Controls.Add(this.checkBox_Fixed);
            base.Name = "FieldDefineControl";
            base.Size = new Size(250, 0x11);
            base.ResumeLayout(false);
        }

        public bool IsEnable()
        {
            return this.m_Define.Enable;
        }

        public bool IsOK()
        {
            return this.m_Define.IsOK();
        }

        public string Serial()
        {
            return this.m_Define.Serial();
        }

        private void textBox_Length_TextChanged(object sender, EventArgs e)
        {
            if (!this.Init)
            {
                try
                {
                    int num = int.Parse(this.textBox_Length.Text);
                    this.m_Define.Length = num;
                }
                catch
                {
                    this.textBox_Length.Text = this.m_Define.Length.ToString();
                }
            }
        }

        private void textBox_Name_TextChanged(object sender, EventArgs e)
        {
            if (!this.Init)
            {
                this.m_Define.Name = this.textBox_Name.Text;
            }
        }
    }
}
