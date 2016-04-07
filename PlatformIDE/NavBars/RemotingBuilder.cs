namespace PlatformIDE.NavBars
{
    using Platform.CSS.Remoting;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    public class RemotingBuilder : UserControl
    {
        private Button button_BrowserPath;
        private Button button_GetRuningAssembly;
        private CheckBox checkBox_HandleException;
        private ColumnHeader columnHeader1;
        private Container components = null;
        private Label label1;
        private ListView listView_TypeList;
        private Hashtable m_TypeValueMap = new Hashtable();
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private TextBox textBox_Path;

        public RemotingBuilder()
        {
            this.InitializeComponent();
            this.m_TypeValueMap[typeof(bool)] = "false";
            this.m_TypeValueMap[typeof(byte)] = "0";
            this.m_TypeValueMap[typeof(sbyte)] = "0";
            this.m_TypeValueMap[typeof(char)] = "'0'";
            this.m_TypeValueMap[typeof(ushort)] = "0";
            this.m_TypeValueMap[typeof(short)] = "0";
            this.m_TypeValueMap[typeof(uint)] = "0";
            this.m_TypeValueMap[typeof(int)] = "0";
            this.m_TypeValueMap[typeof(ulong)] = "0";
            this.m_TypeValueMap[typeof(long)] = "0";
            this.m_TypeValueMap[typeof(DateTime)] = "System.DateTime.MinValue";
            this.m_TypeValueMap[typeof(Guid)] = "System.Guid.Empty";
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

        private void button_GetRuningAssembly_Click(object sender, EventArgs e)
        {
            this.listView_TypeList.Items.Clear();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    RemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(RemotingClassAttribute), false) as RemotingClassAttribute[];
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
                RemotingClassAttribute[] customAttributes = type.GetCustomAttributes(typeof(RemotingClassAttribute), false) as RemotingClassAttribute[];
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
                string text14 = (text4 + "#region 远程调用代码自动生成，版本(V1.06)，请不要手工修改  调用类接口：" + text2 + "." + name + "\r\n") + "\r\n";
                text14 = text14 + text4 + "[ Platform.CSS.Remoting.RemotingClass( \"" + name + "\" , \"" + text2 + "\" ) ]\r\n";
                string text5 = (text14 + text4 + "public sealed class " + type.Name + "\r\n") + text4 + "{\r\n";
                string text6 = text4 + "\t";
                foreach (MethodInfo info in methods)
                {
                    UnRemotingAttribute[] attributeArray2 = info.GetCustomAttributes(typeof(UnRemotingAttribute), false) as UnRemotingAttribute[];
                    if (attributeArray2.Length <= 0)
                    {
                        object obj2;
                        ParameterInfo[] parameters = info.GetParameters();
                        string text7 = "new byte[]{ ";
                        string text8 = "object[] parameters = new object[]{ ";
                        string text9 = "";
                        string text10 = text3 + "::" + info.Name + "(";
                        string fullName = null;
                        RemotingMethodAttribute attribute = null;
                        RemotingMethodAttribute[] attributeArray3 = info.GetCustomAttributes(typeof(RemotingMethodAttribute), false) as RemotingMethodAttribute[];
                        if ((attributeArray3 != null) && (attributeArray3.Length > 0))
                        {
                            attribute = attributeArray3[0];
                        }
                        if (attribute != null)
                        {
                            fullName = attribute.FullName;
                            if (attribute.FullName == null)
                            {
                                text5 = text5 + text6 + "[ Platform.CSS.Remoting.RemotingMethod() ]\r\n";
                            }
                            else
                            {
                                text14 = text5;
                                text5 = text14 + text6 + "[ Platform.CSS.Remoting.RemotingMethod( \"" + fullName + "\" ) ]\r\n";
                            }
                        }
                        text14 = text5;
                        text5 = text14 + text6 + "public " + (info.IsStatic ? "static " : "") + TypeUtility.GetSimpleDataTypeString(info.ReturnType) + " " + info.Name + "( ";
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ParameterInfo info2 = parameters[i];
                            text10 = text10 + info2.ParameterType.Name;
                            string simpleDataTypeString = TypeUtility.GetSimpleDataTypeString(info2.ParameterType);
                            if (info2.ParameterType.IsByRef)
                            {
                                text7 = text7 + "1";
                                obj2 = text9;
                                text9 = string.Concat(new object[] { obj2, text6, this.checkBox_HandleException.Checked ? "\t\t" : "\t", info2.Name, " = ", (simpleDataTypeString == "object") ? "" : ("(" + simpleDataTypeString + ")"), "parameters[ ", i, " ];\r\n" });
                            }
                            else
                            {
                                text7 = text7 + "0";
                            }
                            if (info2.IsOut)
                            {
                                text8 = text8 + "null";
                            }
                            else
                            {
                                text8 = text8 + info2.Name;
                            }
                            text14 = text5;
                            text5 = text14 + (info2.ParameterType.IsByRef ? (info2.IsOut ? "out " : "ref ") : "") + simpleDataTypeString + " " + info2.Name;
                            if (i < (parameters.Length - 1))
                            {
                                text7 = text7 + " , ";
                                text8 = text8 + " , ";
                                text5 = text5 + " , ";
                                text10 = text10 + ",";
                            }
                        }
                        text5 = text5 + " )\r\n";
                        text7 = text7 + " }";
                        text8 = text8 + " };";
                        text10 = text10 + ")";
                        text5 = text5 + text6 + "{\r\n";
                        if (this.checkBox_HandleException.Checked)
                        {
                            text5 = (text5 + text6 + "\ttry\r\n") + text6 + "\t{\r\n";
                        }
                        text14 = text5;
                        text5 = text14 + text6 + (this.checkBox_HandleException.Checked ? "\t\t" : "\t") + text8 + "\r\n";
                        if (info.ReturnType != typeof(void))
                        {
                            text5 = text5 + text6 + (this.checkBox_HandleException.Checked ? "\t\t" : "\t") + "object rc = ";
                        }
                        else
                        {
                            text5 = text5 + text6 + (this.checkBox_HandleException.Checked ? "\t\t" : "\t");
                        }
                        if ((attribute != null) && (fullName != null))
                        {
                            text14 = text5;
                            text5 = text14 + "Platform.CSS.Remoting.RemotingClient.RemoteExecute( \"" + fullName + "\" , " + text7 + " , parameters );\r\n";
                        }
                        else
                        {
                            text14 = text5;
                            text5 = text14 + "Platform.CSS.Remoting.RemotingClient.RemoteExecute( \"" + text10 + "\" , " + text7 + " , parameters );\r\n";
                        }
                        text5 = text5 + text9;
                        if (info.ReturnType != typeof(void))
                        {
                            text14 = text5;
                            text5 = text14 + text6 + (this.checkBox_HandleException.Checked ? "\t\t" : "\t") + "return " + ((info.ReturnType == typeof(object)) ? "" : ("(" + TypeUtility.GetSimpleDataTypeString(info.ReturnType) + ")")) + "rc;\r\n";
                        }
                        if (this.checkBox_HandleException.Checked)
                        {
                            text5 = ((((((text5 + text6 + "\t}\r\n") + text6 + "\tcatch( System.Exception exp )\r\n") + text6 + "\t{\r\n") + text6 + "\t\tif( !Platform.CSS.CSSConfig.ClientCommunicationRemotingHandleException )\r\n") + text6 + "\t\t\tthrow exp;\r\n") + text6 + "\t\tif( Platform.CSS.CSSConfig.ClientCommunicationRemotingShowException )\r\n") + text6 + "\t\t\tPlatform.ExceptionHandling.ExceptionForm.ShowException( exp , false , false , true , false , \"远程调用异常\" );\r\n";
                            if (info.ReturnType != typeof(void))
                            {
                                if (info.ReturnType.IsValueType)
                                {
                                    if (this.m_TypeValueMap.ContainsKey(info.ReturnType))
                                    {
                                        obj2 = text5;
                                        text5 = string.Concat(new object[] { obj2, text6, "\t\treturn ", this.m_TypeValueMap[info.ReturnType], ";\r\n" });
                                    }
                                    else
                                    {
                                        text14 = text5;
                                        text5 = text14 + text6 + "\t\treturn (" + TypeUtility.GetSimpleDataTypeString(info.ReturnType) + ")(object)null;\r\n";
                                    }
                                }
                                else
                                {
                                    text14 = text5;
                                    text5 = text14 + text6 + "\t\treturn " + ((info.ReturnType == typeof(object)) ? "" : ("(" + TypeUtility.GetSimpleDataTypeString(info.ReturnType) + ")")) + "null;\r\n";
                                }
                            }
                            text5 = (text5 + text6 + "\t}\r\n") + text6 + "}\r\n";
                        }
                        else
                        {
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
                    object[] customAttributes = type.GetCustomAttributes(typeof(RemotingClassAttribute), false);
                    RemotingClassAttribute[] attributeArray = type.GetCustomAttributes(typeof(RemotingClassAttribute), false) as RemotingClassAttribute[];
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
            this.panel3 = new Panel();
            this.checkBox_HandleException = new CheckBox();
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
            this.listView_TypeList.Location = new Point(0, 0x43);
            this.listView_TypeList.MultiSelect = false;
            this.listView_TypeList.Name = "listView_TypeList";
            this.listView_TypeList.Size = new Size(0xd8, 0x1bd);
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
            this.panel1.Controls.Add(this.checkBox_HandleException);
            this.panel1.Controls.Add(this.button_GetRuningAssembly);
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
            this.panel3.Dock = DockStyle.Top;
            this.panel3.Location = new Point(0, 0x2c);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(0xd8, 0x17);
            this.panel3.TabIndex = 4;
            this.checkBox_HandleException.Dock = DockStyle.Right;
            this.checkBox_HandleException.Location = new Point(0x88, 0);
            this.checkBox_HandleException.Name = "checkBox_HandleException";
            this.checkBox_HandleException.Size = new Size(80, 0x17);
            this.checkBox_HandleException.TabIndex = 3;
            this.checkBox_HandleException.Text = "处理异常";
            base.Controls.Add(this.listView_TypeList);
            base.Controls.Add(this.panel3);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.panel2);
            base.Name = "RemotingBuilder";
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

        internal static string VerifyCodeBase(string codebase)
        {
            if (codebase == null)
            {
                return null;
            }
            int length = codebase.Length;
            if (length == 0)
            {
                return null;
            }
            int index = codebase.IndexOf(':');
            if ((((index != -1) && ((index + 2) < length)) && ((codebase[index + 1] == '/') || (codebase[index + 1] == '\\'))) && ((codebase[index + 2] == '/') || (codebase[index + 2] == '\\')))
            {
                return codebase;
            }
            if (((length > 2) && (codebase[0] == '\\')) && (codebase[1] == '\\'))
            {
                return ("file://" + codebase);
            }
            return ("file:///" + Path.GetFullPath(codebase));
        }
    }
}
