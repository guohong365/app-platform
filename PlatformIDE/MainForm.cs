namespace PlatformIDE
{
    using Crownwood.DotNetMagic.Common;
    using Crownwood.DotNetMagic.Docking;
    using NavBars;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Resources;
    using System.Windows.Forms;

    public class MainForm : Form
    {
        private Container components = null;
        private DockingManager m_DockManager;
        private RichTextBox richTextBox_Output;
        private StatusBar statusBar_Main;
        private ToolBar toolBar1;
        private ToolBarButton toolBarButton_ClearOutput;

        public MainForm()
        {
            this.InitializeComponent();
            this.m_DockManager = new DockingManager(this, VisualStyle.IDE2005);
            this.m_DockManager.ContentHiding += new DockingManager.ContentHidingHandler(this.OnDockingManager_ContentHiding);
            this.m_DockManager.ContentHidden += new DockingManager.ContentHandler(this.OnDockManager_ContentHidden);
            this.m_DockManager.ResizeBarVector = 2;
            this.m_DockManager.OuterControl = this.statusBar_Main;
            this.m_DockManager.InnerControl = this.richTextBox_Output;
            ControlTraceListener listener = new ControlTraceListener(this.richTextBox_Output);
            Trace.Listeners.Add(listener);
            Debug.Listeners.Add(listener);
            Console.SetOut(new ControlTextWriter(this.richTextBox_Output));
            this.AddDockCompontent(new RemotingBuilder(), Crownwood.DotNetMagic.Docking.State.DockLeft, "远程调用生成", 250, 250, 0, false, false, false);
            this.AddDockCompontent(new BroadcastRemotingBuilder(), Crownwood.DotNetMagic.Docking.State.DockLeft, "远程广播调用生成", 250, 250, 0, false, false, false);
            this.AddDockCompontent(new DataSetCreator(), Crownwood.DotNetMagic.Docking.State.DockLeft, "数据集生成", 250, 250, 0, false, false, false);
            this.AddDockCompontent(new PacketDefineControl(), Crownwood.DotNetMagic.Docking.State.DockLeft, "通讯包定义", 250, 250, 0, false, false, false);
            this.AddDockCompontent(new ToolsControl(), Crownwood.DotNetMagic.Docking.State.DockLeft, "工具集", 250, 250, 0, false, false, false);
        }

        private Content AddDockCompontent(Control ctrl, Crownwood.DotNetMagic.Docking.State dockStyle, string name, int width, int height, int imageIndex, bool autoHide, bool closeButton, bool hideButton)
        {
            ctrl.Parent = null;
            ctrl.Dock = DockStyle.Fill;
            Content c = null;
            foreach (Content content2 in this.m_DockManager.Contents)
            {
                if (content2.ParentWindowContent == null)
                {
                    continue;
                }
                if ((content2.ParentWindowContent.State == dockStyle) && (content2.IsAutoHidden == autoHide))
                {
                    c = this.m_DockManager.Contents.Add(ctrl, name);
                    c.CloseOnHide = false;
                    c.HideButton = hideButton;
                    c.CloseButton = closeButton;
                    c.DisplaySize = new Size(width, height);
                    c.AutoHideSize = new Size(width, height);
                    c.FloatingSize = new Size(width, height);
                    if (content2.IsAutoHidden)
                    {
                        this.m_DockManager.ToggleContentAutoHide(content2);
                        this.m_DockManager.AddContentToWindowContent(c, content2.ParentWindowContent);
                        this.m_DockManager.ToggleContentAutoHide(content2);
                    }
                    else
                    {
                        this.m_DockManager.AddContentToWindowContent(c, content2.ParentWindowContent);
                    }
                    break;
                }
            }
            if (c == null)
            {
                c = this.m_DockManager.Contents.Add(ctrl, name);
                c.CloseOnHide = false;
                c.HideButton = hideButton;
                c.CloseButton = closeButton;
                c.DisplaySize = new Size(width, height);
                c.AutoHideSize = new Size(width, height);
                c.FloatingSize = new Size(width, height);
                WindowContent content3 = this.m_DockManager.AddContentWithState(c, dockStyle);
                if (autoHide)
                {
                    this.m_DockManager.ToggleContentAutoHide(c);
                }
            }
            return c;
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
            ResourceManager manager = new ResourceManager(typeof(MainForm));
            this.richTextBox_Output = new RichTextBox();
            this.toolBar1 = new ToolBar();
            this.toolBarButton_ClearOutput = new ToolBarButton();
            this.statusBar_Main = new StatusBar();
            base.SuspendLayout();
            this.richTextBox_Output.Dock = DockStyle.Fill;
            this.richTextBox_Output.Location = new Point(0, 0x29);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.Size = new Size(480, 0x117);
            this.richTextBox_Output.TabIndex = 0;
            this.richTextBox_Output.Text = "";
            this.richTextBox_Output.WordWrap = false;
            this.richTextBox_Output.TextChanged += new EventHandler(this.richTextBox_Output_TextChanged);
            this.toolBar1.Appearance = ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new ToolBarButton[] { this.toolBarButton_ClearOutput });
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new Size(480, 0x29);
            this.toolBar1.TabIndex = 1;
            this.toolBar1.Visible = false;
            this.toolBar1.ButtonClick += new ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            this.toolBarButton_ClearOutput.Text = "清除输出";
            this.toolBarButton_ClearOutput.ToolTipText = "清除输出";
            this.statusBar_Main.Location = new Point(0, 320);
            this.statusBar_Main.Name = "statusBar_Main";
            this.statusBar_Main.ShowPanels = true;
            this.statusBar_Main.Size = new Size(480, 0x15);
            this.statusBar_Main.SizingGrip = false;
            this.statusBar_Main.TabIndex = 2;
            this.AutoScaleBaseSize = new Size(6, 14);
            base.ClientSize = new Size(480, 0x155);
            base.Controls.Add(this.richTextBox_Output);
            base.Controls.Add(this.statusBar_Main);
            base.Controls.Add(this.toolBar1);
          
            base.Name = "MainForm";
            this.Text = "系统平台工具";
            base.WindowState = FormWindowState.Maximized;
            base.Load += new EventHandler(this.MainForm_Load);
            base.ResumeLayout(false);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void OnDockingManager_ContentHiding(Content c, CancelEventArgs e)
        {
            if ((c != null) && (c.Control != null))
            {
            }
        }

        private void OnDockManager_ContentHidden(Content c, EventArgs e)
        {
            if ((c != null) && (c.Control != null))
            {
            }
        }

        private void richTextBox_Output_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox_Output.Focus();
            int textLength = this.richTextBox_Output.TextLength;
            this.richTextBox_Output.Select(textLength, 0);
            this.richTextBox_Output.ScrollToCaret();
        }

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == this.toolBarButton_ClearOutput)
            {
                this.richTextBox_Output.Text = "";
            }
        }
    }
}
