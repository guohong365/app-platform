namespace PlatformIDE.NavBars
{
    using Platform.CSS.Packet;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Forms;

    public class PacketDefineControl : UserControl
    {
        private ulong bitmap;
        private Button button_BuildCode;
        private CheckedListBox checkedListBox_Fields;
        private ComboBox comboBox_PacketType;
        private Container components;
        private Panel FieldDefines_Panel;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private FieldDefineControl[] m_Fields;
        private bool m_flag;
        private Panel panel1;
        private Panel panel5;
        private Panel panel7;
        private TextBox textBox_FieldBitmap;

        public PacketDefineControl()
        {
            int num;
            this.m_Fields = new FieldDefineControl[0x40];
            this.components = null;
            this.bitmap = (ulong)0;
            this.m_flag = false;
            this.InitializeComponent();
            base.SuspendLayout();
            for (num = 0; num < 0x40; num++)
            {
                this.m_Fields[num] = new FieldDefineControl();
                this.m_Fields[num].Dock = DockStyle.Top;
            }
            for (num = 0x3f; num >= 0; num--)
            {
                this.FieldDefines_Panel.Controls.Add(this.m_Fields[num]);
            }
            base.ResumeLayout();
        }

        private void button_BuildCode_Click(object sender, EventArgs e)
        {
            int num;
            for (num = 0; num < 0x40; num++)
            {
                if (!this.m_Fields[num].IsOK())
                {
                    MessageBox.Show("请填写好第 " + ((num + 1)).ToString() + " 域定义");
                    return;
                }
            }
            string text = "\r\n\t\t#region 下列代码由工具生成，请不要手工修改\r\n\r\n";
            string text2 = "\r\n\t\tpublic override void FreeBuffer()\r\n\t\t{\r\n";
            string text3 = "";
            int count = 0;
            for (num = 0; num < 0x40; num++)
            {
                if (this.m_Fields[num].IsEnable())
                {
                    text3 = text3 + this.m_Fields[num].GetDefine("\t\t", count);
                    text2 = text2 + "\r\n" + this.m_Fields[num].GetRelease("\t\t\t");
                    count++;
                }
            }
            text = ((text + text3 + "\r\n\r\n") + (text2 + "\r\n\t\t\tbase.FreeBuffer();\r\n\t\t}\r\n") + "\r\n\r\n") + "\t\t#endregion\r\n";
            Console.WriteLine("");
            Console.WriteLine(text);
            Clipboard.SetDataObject(text);
        }

        private void checkedListBox_Fields_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!this.m_flag)
            {
                this.m_flag = true;
                if (e.NewValue == CheckState.Checked)
                {
                    this.checkedListBox_Fields.SetItemChecked(e.Index, true);
                }
                else
                {
                    this.checkedListBox_Fields.SetItemChecked(e.Index, false);
                }
                this.m_flag = false;
                this.GetBitmap();
            }
        }

        private void Clear()
        {
            this.checkedListBox_Fields.Items.Clear();
            for (int i = 0; i < 0x40; i++)
            {
                this.m_Fields[i].Clear();
            }
        }

        private void comboBox_PacketType_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Type selectedItem = this.comboBox_PacketType.SelectedItem as System.Type;
            if (selectedItem != null)
            {
                this.LoadDefine(selectedItem);
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

        private void GetBitmap()
        {
            if (!this.m_flag)
            {
                try
                {
                    this.m_flag = true;
                    this.bitmap = (ulong)0;
                    foreach (FieldDefine define in this.checkedListBox_Fields.CheckedItems)
                    {
                        this.bitmap |= define.Bitmap;
                    }
                    this.textBox_FieldBitmap.Text = "0X" + this.bitmap.ToString("X16") + "UL";
                }
                finally
                {
                    this.m_flag = false;
                }
            }
        }

        private void GetFields(string text)
        {
            if (!this.m_flag)
            {
                try
                {
                    ulong num;
                    this.m_flag = true;
                    string s = text.Trim().Replace("0x", "").Replace("0X", "").Replace("ul", "").Replace("Ul", "").Replace("uL", "").Replace("UL", "");
                    if (s.Length == 0)
                    {
                        num = (ulong)0;
                    }
                    else
                    {
                        try
                        {
                            num = ulong.Parse(s, NumberStyles.HexNumber);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    for (int i = 0; i < this.checkedListBox_Fields.Items.Count; i++)
                    {
                        FieldDefine define = this.checkedListBox_Fields.Items[i] as FieldDefine;
                        if ((num & define.Bitmap) == define.Bitmap)
                        {
                            this.checkedListBox_Fields.SetItemChecked(i, true);
                        }
                        else
                        {
                            this.checkedListBox_Fields.SetItemChecked(i, false);
                        }
                    }
                }
                finally
                {
                    this.m_flag = false;
                }
            }
        }

        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.textBox_FieldBitmap = new TextBox();
            this.button_BuildCode = new Button();
            this.FieldDefines_Panel = new Panel();
            this.panel5 = new Panel();
            this.label1 = new Label();
            this.checkedListBox_Fields = new CheckedListBox();
            this.label2 = new Label();
            this.panel7 = new Panel();
            this.comboBox_PacketType = new ComboBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.textBox_FieldBitmap);
            this.panel1.Controls.Add(this.button_BuildCode);
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Location = new Point(0, 0x1cb);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x100, 0x15);
            this.panel1.TabIndex = 5;
            this.textBox_FieldBitmap.Dock = DockStyle.Fill;
            this.textBox_FieldBitmap.Location = new Point(0, 0);
            this.textBox_FieldBitmap.Name = "textBox_FieldBitmap";
            this.textBox_FieldBitmap.Size = new Size(0xb5, 0x15);
            this.textBox_FieldBitmap.TabIndex = 5;
            this.textBox_FieldBitmap.Text = "";
            this.textBox_FieldBitmap.TextChanged += new EventHandler(this.textBox_FieldBitmap_TextChanged);
            this.button_BuildCode.Dock = DockStyle.Right;
            this.button_BuildCode.FlatStyle = FlatStyle.Popup;
            this.button_BuildCode.Location = new Point(0xb5, 0);
            this.button_BuildCode.Name = "button_BuildCode";
            this.button_BuildCode.Size = new Size(0x4b, 0x15);
            this.button_BuildCode.TabIndex = 2;
            this.button_BuildCode.Text = "生成代码";
            this.button_BuildCode.TextAlign = ContentAlignment.TopCenter;
            this.button_BuildCode.Click += new EventHandler(this.button_BuildCode_Click);
            this.FieldDefines_Panel.AutoScroll = true;
            this.FieldDefines_Panel.Dock = DockStyle.Fill;
            this.FieldDefines_Panel.Location = new Point(0, 0x2e);
            this.FieldDefines_Panel.Name = "FieldDefines_Panel";
            this.FieldDefines_Panel.Size = new Size(0x100, 0xe8);
            this.FieldDefines_Panel.TabIndex = 7;
            this.panel5.Controls.Add(this.label4);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = DockStyle.Top;
            this.panel5.Location = new Point(0, 0x17);
            this.panel5.Name = "panel5";
            this.panel5.Size = new Size(0x100, 0x17);
            this.panel5.TabIndex = 8;
            this.label1.Dock = DockStyle.Left;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(40, 0x17);
            this.label1.TabIndex = 0;
            this.label1.Text = "索引";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.checkedListBox_Fields.BorderStyle = BorderStyle.None;
            this.checkedListBox_Fields.CheckOnClick = true;
            this.checkedListBox_Fields.Dock = DockStyle.Bottom;
            this.checkedListBox_Fields.IntegralHeight = false;
            this.checkedListBox_Fields.Location = new Point(0, 0x12b);
            this.checkedListBox_Fields.Name = "checkedListBox_Fields";
            this.checkedListBox_Fields.Size = new Size(0x100, 160);
            this.checkedListBox_Fields.TabIndex = 0;
            this.checkedListBox_Fields.ItemCheck += new ItemCheckEventHandler(this.checkedListBox_Fields_ItemCheck);
            this.label2.Dock = DockStyle.Bottom;
            this.label2.Location = new Point(0, 0x116);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x100, 0x15);
            this.label2.TabIndex = 0;
            this.label2.Text = "域图转换";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            this.panel7.Controls.Add(this.comboBox_PacketType);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Dock = DockStyle.Top;
            this.panel7.Location = new Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new Size(0x100, 0x17);
            this.panel7.TabIndex = 11;
            this.comboBox_PacketType.Dock = DockStyle.Fill;
            this.comboBox_PacketType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_PacketType.Location = new Point(0x48, 0);
            this.comboBox_PacketType.Name = "comboBox_PacketType";
            this.comboBox_PacketType.Size = new Size(0xb8, 20);
            this.comboBox_PacketType.TabIndex = 1;
            this.comboBox_PacketType.SelectedIndexChanged += new EventHandler(this.comboBox_PacketType_SelectedIndexChanged);
            this.label3.Dock = DockStyle.Left;
            this.label3.Location = new Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x48, 0x17);
            this.label3.TabIndex = 0;
            this.label3.Text = "通讯接口：";
            this.label3.TextAlign = ContentAlignment.MiddleLeft;
            this.label4.Dock = DockStyle.Fill;
            this.label4.Location = new Point(40, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(80, 0x17);
            this.label4.TabIndex = 1;
            this.label4.Text = "名称";
            this.label4.TextAlign = ContentAlignment.MiddleCenter;
            this.label5.Dock = DockStyle.Right;
            this.label5.Location = new Point(120, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x48, 0x17);
            this.label5.TabIndex = 2;
            this.label5.Text = "类型";
            this.label5.TextAlign = ContentAlignment.MiddleCenter;
            this.label6.Dock = DockStyle.Right;
            this.label6.Location = new Point(0xc0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x20, 0x17);
            this.label6.TabIndex = 3;
            this.label6.Text = "长度";
            this.label6.TextAlign = ContentAlignment.MiddleCenter;
            this.label7.Dock = DockStyle.Right;
            this.label7.Location = new Point(0xe0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new Size(0x20, 0x17);
            this.label7.TabIndex = 4;
            this.label7.Text = "定长";
            this.label7.TextAlign = ContentAlignment.MiddleCenter;
            base.Controls.Add(this.FieldDefines_Panel);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.checkedListBox_Fields);
            base.Controls.Add(this.panel5);
            base.Controls.Add(this.panel7);
            base.Controls.Add(this.panel1);
            base.Name = "PacketDefineControl";
            base.Size = new Size(0x100, 480);
            base.Load += new EventHandler(this.PacketDefineControl_Load);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void LoadDefine(System.Type type)
        {
            this.Clear();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in properties)
            {
                PacketFieldAttribute[] customAttributes = info.GetCustomAttributes(typeof(PacketFieldAttribute), false) as PacketFieldAttribute[];
                if ((customAttributes != null) && (customAttributes.Length >= 1))
                {
                    this.m_Fields[customAttributes[0].Index].Initialize(customAttributes[0]);
                    this.checkedListBox_Fields.Items.Add(this.m_Fields[customAttributes[0].Index].m_Define);
                }
            }
        }

        private void LoadPacketTypes()
        {
            this.comboBox_PacketType.Items.Clear();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(PacketBase.PacketBaseType))
                    {
                        this.comboBox_PacketType.Items.Add(type);
                    }
                }
            }
        }

        private void PacketDefineControl_Load(object sender, EventArgs e)
        {
            this.LoadPacketTypes();
        }

        private void textBox_FieldBitmap_TextChanged(object sender, EventArgs e)
        {
            this.GetFields(this.textBox_FieldBitmap.Text);
        }
    }
}
