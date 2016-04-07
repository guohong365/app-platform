namespace Platform.Configuration
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    public class ConfigurationSetting : Form
    {
        private Button button_Cancel;
        private Button button_OK;
        private IContainer components;
        private ImageList imageList_SettingsTree;
        private bool m_Loading;
        private static ArrayList m_SettingClass = new ArrayList();
        private Panel panel_SettingContainer;
        private Panel panel1;
        private Panel panel2;
        private TreeView treeView_Settings;

        public ConfigurationSetting()
        {
            this.InitializeComponent();
        }

        private void ConfigurationSetting_Load(object sender, EventArgs e)
        {
            this.m_Loading = true;
            foreach (SettingItem item in m_SettingClass)
            {
                this.treeView_Settings.Nodes.Add(item.Attr.DisplayName).Tag = new SettingRuntimeItem(item);
            }
            this.m_Loading = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DispSetting(SettingRuntimeItem item)
        {
            this.panel_SettingContainer.Controls.Clear();
            ConfigurationControlBase control = item.Control;
            control.Dock = DockStyle.Fill;
            this.panel_SettingContainer.Controls.Add(control);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ResourceManager manager = new ResourceManager(typeof(ConfigurationSetting));
            this.treeView_Settings = new TreeView();
            this.imageList_SettingsTree = new ImageList(this.components);
            this.panel1 = new Panel();
            this.button_Cancel = new Button();
            this.button_OK = new Button();
            this.panel2 = new Panel();
            this.panel_SettingContainer = new Panel();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.treeView_Settings.Dock = DockStyle.Left;
            this.treeView_Settings.FullRowSelect = true;
            this.treeView_Settings.HideSelection = false;
            this.treeView_Settings.ImageList = this.imageList_SettingsTree;
            this.treeView_Settings.Location = new Point(5, 5);
            this.treeView_Settings.Name = "treeView_Settings";
            this.treeView_Settings.Size = new Size(160, 280);
            this.treeView_Settings.TabIndex = 0;
            this.treeView_Settings.Click += new EventHandler(this.treeView_Settings_Click);
            this.treeView_Settings.AfterSelect += new TreeViewEventHandler(this.treeView_Settings_AfterSelect);
            this.imageList_SettingsTree.ImageSize = new Size(0x10, 0x10);
            this.imageList_SettingsTree.ImageStream = (ImageListStreamer) manager.GetObject("imageList_SettingsTree.ImageStream");
            this.imageList_SettingsTree.TransparentColor = Color.Transparent;
            this.panel1.Controls.Add(this.button_Cancel);
            this.panel1.Controls.Add(this.button_OK);
            this.panel1.Dock = DockStyle.Bottom;
            this.panel1.Location = new Point(5, 0x11d);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x248, 0x23);
            this.panel1.TabIndex = 1;
            this.button_Cancel.FlatStyle = FlatStyle.Popup;
            this.button_Cancel.Location = new Point(0x1a8, 6);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "取消";
            this.button_OK.FlatStyle = FlatStyle.Popup;
            this.button_OK.Location = new Point(0x157, 6);
            this.button_OK.Name = "button_OK";
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "确定";
            this.panel2.Dock = DockStyle.Left;
            this.panel2.Location = new Point(0xa5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0x18, 280);
            this.panel2.TabIndex = 2;
            this.panel_SettingContainer.Dock = DockStyle.Fill;
            this.panel_SettingContainer.Location = new Point(0xbd, 5);
            this.panel_SettingContainer.Name = "panel_SettingContainer";
            this.panel_SettingContainer.Size = new Size(400, 280);
            this.panel_SettingContainer.TabIndex = 3;
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(0x252, 0x145);
            base.Controls.Add(this.panel_SettingContainer);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.treeView_Settings);
            base.Controls.Add(this.panel1);
            base.DockPadding.All = 5;
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ConfigurationSetting";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "选项";
            base.Load += new EventHandler(this.ConfigurationSetting_Load);
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        public static bool RegisterConfigurationSettingType(System.Type settingType)
        {
            ConfigurationSettingClassAttribute[] customAttributes = settingType.GetCustomAttributes(typeof(ConfigurationSettingClassAttribute), false) as ConfigurationSettingClassAttribute[];
            if ((customAttributes == null) || (customAttributes.Length < 1))
            {
                return false;
            }
            m_SettingClass.Add(new SettingItem(customAttributes[0], settingType));
            return true;
        }

        private void treeView_Settings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (((!this.m_Loading && (sender is TreeView)) && (e.Node != null)) && (e.Node.Tag != null))
            {
                SettingRuntimeItem tag = e.Node.Tag as SettingRuntimeItem;
                if (tag != null)
                {
                    this.DispSetting(tag);
                }
            }
        }

        private void treeView_Settings_Click(object sender, EventArgs e)
        {
            if (!this.m_Loading)
            {
                TreeView view = sender as TreeView;
                if (view != null)
                {
                    TreeNode nodeAt = view.GetNodeAt(view.PointToClient(Cursor.Position));
                    if (nodeAt != null)
                    {
                        if (view.SelectedNode != nodeAt)
                        {
                            view.SelectedNode = nodeAt;
                        }
                        else if (nodeAt.Tag != null)
                        {
                            SettingRuntimeItem tag = nodeAt.Tag as SettingRuntimeItem;
                            if (tag != null)
                            {
                                this.DispSetting(tag);
                            }
                        }
                    }
                }
            }
        }

        private class SettingItem
        {
            public ConfigurationSettingClassAttribute Attr;
            public FieldInfo[] SettingFields;
            public System.Type SettingType;

            public SettingItem(ConfigurationSettingClassAttribute attr, System.Type settingType)
            {
                this.Attr = attr;
                this.SettingType = settingType;
                FieldInfo[] fields = this.SettingType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (fields != null)
                {
                    ArrayList list = new ArrayList();
                    foreach (FieldInfo info in fields)
                    {
                        if (info.IsStatic)
                        {
                            ConfigurationSettingFieldAttribute[] customAttributes = settingType.GetCustomAttributes(typeof(ConfigurationSettingFieldAttribute), false) as ConfigurationSettingFieldAttribute[];
                            if ((customAttributes == null) || (customAttributes.Length < 1))
                            {
                                return;
                            }
                            list.Add(info);
                        }
                    }
                    this.SettingFields = list.ToArray(typeof(FieldInfo)) as FieldInfo[];
                }
            }
        }

        private class SettingRuntimeItem
        {
            public ConfigurationSettingClassAttribute Attr;
            public System.Type ControlType;
            public ConfigurationControlBase m_Control;
            public FieldInfo[] SettingFields;
            public System.Type SettingType;

            public SettingRuntimeItem(ConfigurationSetting.SettingItem item)
            {
                this.Attr = item.Attr;
                this.ControlType = item.Attr.ControlType;
                this.SettingType = item.SettingType;
                this.SettingFields = item.SettingFields;
                this.m_Control = null;
            }

            public ConfigurationControlBase Control
            {
                get
                {
                    if (this.m_Control == null)
                    {
                        this.m_Control = Activator.CreateInstance(this.ControlType) as ConfigurationControlBase;
                    }
                    return this.m_Control;
                }
            }
        }
    }
}
