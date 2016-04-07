namespace PlatformIDE.NavBars
{
    using Platform.CSS.Remoting;
    using Platform.Utils;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    public class BroadcastRemotingBuilder : UserControl
    {
        private Button button_BrowserPath;
        private Button button_CreateGuid;
        private Button button_GetRuningAssembly;
        private ColumnHeader columnHeader1;
        private Container components = null;
        private Label label1;
        private ListView listView_TypeList;
        private Panel panel1;
        private Panel panel2;
        private TextBox textBox_Path;

        public BroadcastRemotingBuilder()
        {
            this.InitializeComponent();
        }

        private void button_BrowserPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_Path.Text = dialog.FileName;
                this.GetRemoteCallServerType(this.textBox_Path.Text);
            }
        }

        private void button_CreateGuid_Click(object sender, EventArgs e)
        {
            string text = Guid.NewGuid().ToString("B").ToUpper();
            Console.WriteLine(text);
            Clipboard.SetDataObject(text);
        }

        private void button_GetRuningAssembly_Click(object sender, EventArgs e)
        {
            this.listView_TypeList.Items.Clear();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    BroadcastRemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(BroadcastRemotingClassAttribute), false) as BroadcastRemotingClassAttribute[];
                    if (customAttributes.Length >= 1)
                    {
                        this.listView_TypeList.Items.Add(type.FullName).Tag = type;
                    }
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

        private string GetClientDefine(System.Type type)
        {
            try
            {
                BroadcastRemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(BroadcastRemotingClassAttribute), false) as BroadcastRemotingClassAttribute[];
                if (customAttributes.Length < 1)
                {
                    return "";
                }
                string name = customAttributes[0].Name;
                switch (name)
                {
                    case null:
                    case "":
                        name = type.Name;
                        break;
                }
                string text2 = customAttributes[0].Namespace;
                if ((text2 == null) || (text2 == ""))
                {
                    text2 = type.Namespace;
                }
                string text3 = text2 + "+" + name;
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                string text4 = "\t";
                string text12 = (text4 + "#region 远程广播调用代码自动生成，版本(V1.05)，请不要手工修改  广播调用类接口：" + text2 + "." + name + "\r\n") + "\r\n";
                text12 = text12 + text4 + "[ Platform.CSS.Remoting.BroadcastRemotingClass( \"" + name + "\" , \"" + text2 + "\" ) ]\r\n";
                string text5 = (text12 + text4 + "public sealed class " + type.Name + "\r\n") + text4 + "{\r\n";
                string text6 = text4 + "\t";
                foreach (MethodInfo info in methods)
                {
                    if (info.ReturnType == typeof(void))
                    {
                        int num;
                        ParameterInfo[] parameters = info.GetParameters();
                        bool flag = false;
                        for (num = 0; num < parameters.Length; num++)
                        {
                            if (parameters[num].IsOut)
                            {
                                flag = true;
                                break;
                            }
                            if (parameters[num].ParameterType.IsByRef)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            BroadcastRemotingMethodAttribute attribute = null;
                            BroadcastRemotingMethodAttribute[] attributeArray2 = info.GetCustomAttributes(typeof(BroadcastRemotingMethodAttribute), false) as BroadcastRemotingMethodAttribute[];
                            if ((attributeArray2 != null) && (attributeArray2.Length > 0))
                            {
                                attribute = attributeArray2[0];
                            }
                            string text7 = "new object[]{ ";
                            string text8 = text3 + "::" + info.Name + "(";
                            string fullName = null;
                            if (attribute != null)
                            {
                                fullName = attribute.FullName;
                                if (attribute.FullName != null)
                                {
                                    text12 = text5;
                                    text5 = text12 + text6 + "[ Platform.CSS.Remoting.BroadcastRemotingMethod( \"" + attribute.FullName + "\" ) ]\r\n";
                                }
                                else
                                {
                                    text5 = text5 + text6 + "[ Platform.CSS.Remoting.BroadcastRemotingMethod( ) ]\r\n";
                                }
                            }
                            text12 = text5;
                            text5 = text12 + text6 + "public " + (info.IsStatic ? "static " : "") + TypeUtility.GetSimpleDataTypeString(info.ReturnType) + " " + info.Name + "( ";
                            for (num = 0; num < parameters.Length; num++)
                            {
                                ParameterInfo info2 = parameters[num];
                                text8 = text8 + info2.ParameterType.Name;
                                string simpleDataTypeString = TypeUtility.GetSimpleDataTypeString(info2.ParameterType);
                                text7 = text7 + info2.Name;
                                text5 = text5 + simpleDataTypeString + " " + info2.Name;
                                if (num < (parameters.Length - 1))
                                {
                                    text7 = text7 + " , ";
                                    text5 = text5 + " , ";
                                    text8 = text8 + ",";
                                }
                            }
                            text5 = text5 + " )\r\n";
                            text7 = text7 + " }";
                            text8 = text8 + ")";
                            text5 = text5 + text6 + "{\r\n";
                            if ((attribute != null) && (fullName != null))
                            {
                                text12 = text5;
                                text5 = text12 + text6 + "\tPlatform.CSS.Remoting.BroadcastRemotingClient.RemoteExecute( \"" + fullName + "\" , " + text7 + " );\r\n";
                            }
                            else
                            {
                                text12 = text5;
                                text5 = text12 + text6 + "\tPlatform.CSS.Remoting.BroadcastRemotingClient.RemoteExecute( \"" + text8 + "\" , " + text7 + " );\r\n";
                            }
                            text5 = text5 + text6 + "}\r\n";
                        }
                    }
                }
                return ((((text5 + text4 + "}\r\n") + "\r\n") + text4 + "#endregion\r\n") + "\r\n");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return "";
            }
        }

        private void GetRemoteCallServerType(string asmFileName)
        {
            if (File.Exists(asmFileName))
            {
                this.listView_TypeList.Items.Clear();
                Assembly assembly = Assembly.LoadFile(asmFileName);
                foreach (System.Type type in assembly.GetTypes())
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(BroadcastRemotingClassAttribute), false);
                    BroadcastRemotingClassAttribute[] attributeArray = type.GetCustomAttributes(typeof(BroadcastRemotingClassAttribute), false) as BroadcastRemotingClassAttribute[];
                    if (attributeArray.Length >= 1)
                    {
                        this.listView_TypeList.Items.Add(type.FullName).Tag = type;
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.listView_TypeList = new ListView();
            this.columnHeader1 = new ColumnHeader();
            this.panel2 = new Panel();
            this.textBox_Path = new TextBox();
            this.button_BrowserPath = new Button();
            this.label1 = new Label();
            this.panel1 = new Panel();
            this.button_GetRuningAssembly = new Button();
            this.button_CreateGuid = new Button();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.listView_TypeList.BorderStyle = BorderStyle.FixedSingle;
            this.listView_TypeList.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
            this.listView_TypeList.Dock = DockStyle.Fill;
            this.listView_TypeList.FullRowSelect = true;
            this.listView_TypeList.GridLines = true;
            this.listView_TypeList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listView_TypeList.HideSelection = false;
            this.listView_TypeList.Location = new Point(0, 0x2c);
            this.listView_TypeList.MultiSelect = false;
            this.listView_TypeList.Name = "listView_TypeList";
            this.listView_TypeList.Size = new Size(0xd8, 0x1d4);
            this.listView_TypeList.TabIndex = 2;
            this.listView_TypeList.View = View.Details;
            this.listView_TypeList.SelectedIndexChanged += new EventHandler(this.listView_TypeList_SelectedIndexChanged);
            this.columnHeader1.Text = "远程调用类";
            this.columnHeader1.Width = 0xba;
            this.panel2.Controls.Add(this.textBox_Path);
            this.panel2.Controls.Add(this.button_BrowserPath);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = DockStyle.Top;
            this.panel2.Location = new Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0xd8, 0x15);
            this.panel2.TabIndex = 1;
            this.textBox_Path.Dock = DockStyle.Fill;
            this.textBox_Path.Location = new Point(0x30, 0);
            this.textBox_Path.Name = "textBox_Path";
            this.textBox_Path.Size = new Size(0x88, 0x15);
            this.textBox_Path.TabIndex = 0;
            this.textBox_Path.Text = "";
            this.button_BrowserPath.Dock = DockStyle.Right;
            this.button_BrowserPath.FlatStyle = FlatStyle.Popup;
            this.button_BrowserPath.Location = new Point(0xb8, 0);
            this.button_BrowserPath.Name = "button_BrowserPath";
            this.button_BrowserPath.Size = new Size(0x20, 0x15);
            this.button_BrowserPath.TabIndex = 2;
            this.button_BrowserPath.Text = "…";
            this.button_BrowserPath.Click += new EventHandler(this.button_BrowserPath_Click);
            this.label1.Dock = DockStyle.Left;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x30, 0x15);
            this.label1.TabIndex = 1;
            this.label1.Text = "路径：";
            this.label1.TextAlign = ContentAlignment.MiddleRight;
            this.panel1.Controls.Add(this.button_GetRuningAssembly);
            this.panel1.Controls.Add(this.button_CreateGuid);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0x15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0xd8, 0x17);
            this.panel1.TabIndex = 3;
            this.button_GetRuningAssembly.Dock = DockStyle.Left;
            this.button_GetRuningAssembly.FlatStyle = FlatStyle.Popup;
            this.button_GetRuningAssembly.Location = new Point(0, 0);
            this.button_GetRuningAssembly.Name = "button_GetRuningAssembly";
            this.button_GetRuningAssembly.Size = new Size(0x58, 0x17);
            this.button_GetRuningAssembly.TabIndex = 2;
            this.button_GetRuningAssembly.Text = "获取加载接口";
            this.button_GetRuningAssembly.Click += new EventHandler(this.button_GetRuningAssembly_Click);
            this.button_CreateGuid.Dock = DockStyle.Right;
            this.button_CreateGuid.FlatStyle = FlatStyle.Popup;
            this.button_CreateGuid.Location = new Point(0x80, 0);
            this.button_CreateGuid.Name = "button_CreateGuid";
            this.button_CreateGuid.Size = new Size(0x58, 0x17);
            this.button_CreateGuid.TabIndex = 3;
            this.button_CreateGuid.Text = "创建接口ID";
            this.button_CreateGuid.Click += new EventHandler(this.button_CreateGuid_Click);
            base.Controls.Add(this.listView_TypeList);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.panel2);
            base.Name = "BroadcastRemotingBuilder";
            base.Size = new Size(0xd8, 0x200);
            base.SizeChanged += new EventHandler(this.RemotingBuilder_SizeChanged);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void listView_TypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_TypeList.SelectedItems.Count >= 1)
            {
                ListViewItem item = this.listView_TypeList.SelectedItems[0];
                System.Type tag = item.Tag as System.Type;
                string clientDefine = this.GetClientDefine(tag);
                Console.WriteLine("");
                Console.WriteLine(clientDefine);
                Clipboard.SetDataObject(clientDefine);
            }
        }

        private void RemotingBuilder_SizeChanged(object sender, EventArgs e)
        {
            this.columnHeader1.Width = this.listView_TypeList.Width - 0x19;
        }
    }
}
