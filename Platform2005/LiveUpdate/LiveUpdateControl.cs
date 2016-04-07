namespace Platform.LiveUpdate
{
    using Platform.IO;
    using Platform.Net;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class LiveUpdateControl : UserControl
    {
        private Button button_Update;
        private CheckBox checkBox_UseWebService;
        private Container components;
        private Label label_DownloadInformation;
        private Label label_EachInfo;
        private Label label_Information;
        private Label label2;
        private LiveUpdateHelper m_Helper = new LiveUpdateHelper();
        private LiveUpdateDownloadHandler m_OnUpdateDownload;
        private LiveUpdateEventHandler m_OnUpdateEvent;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private ProgressBar progressBar_EachProcess;
        private ProgressBar progressBar_TotalProcess;

        [Browsable(true), DefaultValue((string)null), Category("自动更新事件")]
        public event LiveUpdateHandler OnLiveUpdate;

        [Browsable(true), Category("自动更新事件"), DefaultValue((string)null)]
        public event LiveUpdatePerformUpdateHandler OnPerformUpdate;

        public LiveUpdateControl()
        {
            this.InitializeComponent();
            this.m_Helper.OnUpdateEvent += new LiveUpdateEventHandler(this.OnUpdateEvent);
            this.m_Helper.OnUpdateDownload += new LiveUpdateDownloadHandler(this.OnUpdateDownload);
            this.m_OnUpdateEvent = new LiveUpdateEventHandler(this.InvokeOnUpdateEvent);
            this.m_OnUpdateDownload = new LiveUpdateDownloadHandler(this.InvokeOnUpdateDownload);
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            this.PerformUpdate();
        }

        private void checkBox_UseWebService_CheckedChanged(object sender, EventArgs e)
        {
            this.m_Helper.UseWebService = this.checkBox_UseWebService.Checked;
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
            components = new Container();
            this.progressBar_TotalProcess = new ProgressBar();
            this.progressBar_EachProcess = new ProgressBar();
            this.label_EachInfo = new Label();
            this.label_Information = new Label();
            this.panel3 = new Panel();
            this.label2 = new Label();
            this.label_DownloadInformation = new Label();
            this.panel4 = new Panel();
            this.panel2 = new Panel();
            this.button_Update = new Button();
            this.checkBox_UseWebService = new CheckBox();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            base.SuspendLayout();
            this.progressBar_TotalProcess.Dock = DockStyle.Fill;
            this.progressBar_TotalProcess.Location = new Point(0x30, 0);
            this.progressBar_TotalProcess.Name = "progressBar_TotalProcess";
            this.progressBar_TotalProcess.Size = new Size(0xd8, 0x10);
            this.progressBar_TotalProcess.Step = 1;
            this.progressBar_TotalProcess.TabIndex = 0;
            this.progressBar_EachProcess.Dock = DockStyle.Fill;
            this.progressBar_EachProcess.Location = new Point(0x98, 0);
            this.progressBar_EachProcess.Name = "progressBar_EachProcess";
            this.progressBar_EachProcess.Size = new Size(160, 0x10);
            this.progressBar_EachProcess.Step = 1;
            this.progressBar_EachProcess.TabIndex = 2;
            this.label_EachInfo.Dock = DockStyle.Left;
            this.label_EachInfo.Location = new Point(0x62, 0);
            this.label_EachInfo.Name = "label_EachInfo";
            this.label_EachInfo.Size = new Size(0x36, 0x10);
            this.label_EachInfo.TabIndex = 3;
            this.label_EachInfo.Text = "下载进度";
            this.label_EachInfo.TextAlign = ContentAlignment.MiddleRight;
            this.label_Information.BorderStyle = BorderStyle.Fixed3D;
            this.label_Information.Dock = DockStyle.Bottom;
            this.label_Information.Location = new Point(0x30, 0);
            this.label_Information.Name = "label_Information";
            this.label_Information.Size = new Size(0x108, 0x10);
            this.label_Information.TabIndex = 6;
            this.panel3.Controls.Add(this.progressBar_TotalProcess);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = DockStyle.Bottom;
            this.panel3.Location = new Point(0x30, 0x10);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(0x108, 0x10);
            this.panel3.TabIndex = 4;
            this.label2.Dock = DockStyle.Left;
            this.label2.Location = new Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x30, 0x10);
            this.label2.TabIndex = 2;
            this.label2.Text = "总进度";
            this.label2.TextAlign = ContentAlignment.MiddleRight;
            this.label_DownloadInformation.BorderStyle = BorderStyle.Fixed3D;
            this.label_DownloadInformation.Dock = DockStyle.Bottom;
            this.label_DownloadInformation.Location = new Point(0x30, 0x20);
            this.label_DownloadInformation.Name = "label_DownloadInformation";
            this.label_DownloadInformation.Size = new Size(0x108, 0x10);
            this.label_DownloadInformation.TabIndex = 7;
            this.panel4.Controls.Add(this.progressBar_EachProcess);
            this.panel4.Controls.Add(this.label_EachInfo);
            this.panel4.Controls.Add(this.checkBox_UseWebService);
            this.panel4.Dock = DockStyle.Bottom;
            this.panel4.Location = new Point(0, 0x30);
            this.panel4.Name = "panel4";
            this.panel4.Size = new Size(0x138, 0x10);
            this.panel4.TabIndex = 5;
            this.panel2.Controls.Add(this.button_Update);
            this.panel2.Dock = DockStyle.Left;
            this.panel2.Location = new Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0x30, 0x30);
            this.panel2.TabIndex = 5;
            this.button_Update.Dock = DockStyle.Fill;
            this.button_Update.FlatStyle = FlatStyle.Popup;
            this.button_Update.Location = new Point(0, 0);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new Size(0x30, 0x30);
            this.button_Update.TabIndex = 0;
            this.button_Update.Text = "检查更新";
            this.button_Update.Click += new EventHandler(this.button_Update_Click);
            this.checkBox_UseWebService.Dock = DockStyle.Left;
            this.checkBox_UseWebService.Location = new Point(0, 0);
            this.checkBox_UseWebService.Name = "checkBox_UseWebService";
            this.checkBox_UseWebService.RightToLeft = RightToLeft.Yes;
            this.checkBox_UseWebService.Size = new Size(0x62, 0x10);
            this.checkBox_UseWebService.TabIndex = 2;
            this.checkBox_UseWebService.Text = "使用服务下载";
            this.checkBox_UseWebService.TextAlign = ContentAlignment.TopLeft;
            this.checkBox_UseWebService.CheckedChanged += new EventHandler(this.checkBox_UseWebService_CheckedChanged);
            base.Controls.Add(this.label_Information);
            base.Controls.Add(this.panel3);
            base.Controls.Add(this.label_DownloadInformation);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.panel4);
            base.Name = "LiveUpdateControl";
            base.Size = new Size(0x138, 0x40);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private bool InvokeOnUpdateDownload(int step, HttpInformation information)
        {
            if ((base.Handle == IntPtr.Zero) || base.IsDisposed)
            {
                return false;
            }
            object[] state = information.State as object[];
            switch (information.MessageType)
            {
                case 0:
                    this.label_DownloadInformation.Text = "正在下载：" + ((string)state[0]) + "……";
                    this.progressBar_EachProcess.Minimum = 0;
                    this.progressBar_EachProcess.Value = 0;
                    if (information.Length < 0)
                    {
                        this.progressBar_EachProcess.Maximum = (int)state[1];
                    }
                    else
                    {
                        this.progressBar_EachProcess.Maximum = information.Length;
                    }
                    break;

                case 1:
                    if (information.ReadTotal > this.progressBar_EachProcess.Maximum)
                    {
                        this.progressBar_EachProcess.Value = this.progressBar_EachProcess.Maximum;
                    }
                    else
                    {
                        this.progressBar_EachProcess.Value = information.ReadTotal;
                    }
                    break;

                case 2:
                    if (information.ReadTotal <= this.progressBar_EachProcess.Maximum)
                    {
                        this.progressBar_EachProcess.Value = information.ReadTotal;
                    }
                    break;

                default:
                    this.label_DownloadInformation.Text = information.Message;
                    break;
            }
            return true;
        }

        private void InvokeOnUpdateEvent(int step, int errorCode, int count, int position, string message)
        {
            if ((base.Handle != IntPtr.Zero) && !base.IsDisposed)
            {
                if (step != 0)
                {
                    if (step != 1)
                    {
                        if (step != 2)
                        {
                            return;
                        }
                        this.button_Update.Enabled = false;
                        this.button_Update.Text = "应用更新";
                        switch (errorCode)
                        {
                            case -1:
                                goto Label_04DE;

                            case 0:
                                if (this.m_Helper.UpdateFilesCount >= 1)
                                {
                                    this.button_Update.Enabled = true;
                                    this.button_Update.Text = "检查更新";
                                    this.label_DownloadInformation.Text = message;
                                }
                                else
                                {
                                    this.button_Update.Enabled = true;
                                    this.button_Update.Text = "检查更新";
                                    this.label_Information.Text = "更新检查完成，没有需要更新的文件！";
                                    this.label_DownloadInformation.Text = "";
                                }
                                if (this.OnLiveUpdate != null)
                                {
                                    this.OnLiveUpdate(step, true);
                                }
                                return;

                            case 1:
                                this.label_Information.Text = message;
                                this.label_DownloadInformation.Text = "";
                                this.progressBar_TotalProcess.Maximum = count;
                                this.progressBar_TotalProcess.Minimum = 0;
                                this.progressBar_TotalProcess.Value = 0;
                                this.progressBar_EachProcess.Maximum = count;
                                this.progressBar_EachProcess.Minimum = 0;
                                this.progressBar_EachProcess.Value = 0;
                                return;

                            case 2:
                                this.label_DownloadInformation.Text = message;
                                return;

                            case 3:
                                this.label_DownloadInformation.Text = message;
                                this.progressBar_TotalProcess.PerformStep();
                                this.progressBar_EachProcess.PerformStep();
                                return;
                        }
                        goto Label_04DE;
                    }
                    this.button_Update.Enabled = false;
                    this.button_Update.Text = "下载更新";
                    switch (errorCode)
                    {
                        case -1:
                            goto Label_0340;

                        case 0:
                            this.button_Update.Enabled = true;
                            this.label_DownloadInformation.Text = message;
                            this.button_Update.Text = "应用更新";
                            if (this.OnLiveUpdate != null)
                            {
                                this.OnLiveUpdate(step, true);
                            }
                            return;

                        case 1:
                            this.label_Information.Text = message;
                            this.label_DownloadInformation.Text = "";
                            this.progressBar_TotalProcess.Maximum = count;
                            this.progressBar_TotalProcess.Minimum = 0;
                            this.progressBar_TotalProcess.Value = 0;
                            this.progressBar_EachProcess.Value = this.progressBar_EachProcess.Minimum;
                            return;

                        case 2:
                            this.label_DownloadInformation.Text = message;
                            this.progressBar_TotalProcess.PerformStep();
                            this.progressBar_EachProcess.Value = this.progressBar_EachProcess.Maximum;
                            return;

                        case 3:
                            this.label_DownloadInformation.Text = message;
                            this.progressBar_TotalProcess.PerformStep();
                            this.progressBar_EachProcess.Value = this.progressBar_EachProcess.Maximum;
                            return;
                    }
                    goto Label_0340;
                }
                this.button_Update.Enabled = false;
                this.button_Update.Text = "检查更新";
                switch (errorCode)
                {
                    case -1:
                        goto Label_01D0;

                    case 0:
                        if (this.m_Helper.UpdateFilesCount >= 1)
                        {
                            this.button_Update.Enabled = true;
                            this.button_Update.Text = "下载更新";
                            this.label_DownloadInformation.Text = message;
                            break;
                        }
                        this.button_Update.Enabled = true;
                        this.button_Update.Text = "检查更新";
                        this.label_Information.Text = "更新检查完成，没有需要更新的文件！";
                        this.label_DownloadInformation.Text = "";
                        break;

                    case 1:
                        this.label_Information.Text = message;
                        this.label_DownloadInformation.Text = "";
                        this.progressBar_TotalProcess.Maximum = 1;
                        this.progressBar_TotalProcess.Minimum = 0;
                        this.progressBar_TotalProcess.Value = 0;
                        this.progressBar_EachProcess.Value = this.progressBar_EachProcess.Minimum;
                        return;

                    case 2:
                        this.label_Information.Text = message;
                        this.label_DownloadInformation.Text = "";
                        this.progressBar_TotalProcess.Maximum = count;
                        this.progressBar_TotalProcess.Minimum = 0;
                        this.progressBar_TotalProcess.Value = 0;
                        this.progressBar_EachProcess.Maximum = count;
                        this.progressBar_EachProcess.Minimum = 0;
                        this.progressBar_EachProcess.Value = 0;
                        return;

                    case 3:
                        this.label_DownloadInformation.Text = message;
                        this.progressBar_TotalProcess.PerformStep();
                        this.progressBar_EachProcess.PerformStep();
                        return;

                    default:
                        goto Label_01D0;
                }
                if (this.OnLiveUpdate != null)
                {
                    this.OnLiveUpdate(step, true);
                }
            }
            return;
        Label_01D0:
            this.button_Update.Enabled = true;
            this.label_DownloadInformation.Text = message;
            if (this.OnLiveUpdate != null)
            {
                this.OnLiveUpdate(step, false);
            }
            return;
        Label_0340:
            this.button_Update.Enabled = true;
            this.label_DownloadInformation.Text = message;
            if (this.OnLiveUpdate != null)
            {
                this.OnLiveUpdate(step, false);
            }
            return;
        Label_04DE:
            this.button_Update.Enabled = true;
            this.label_DownloadInformation.Text = message;
            if (this.OnLiveUpdate != null)
            {
                this.OnLiveUpdate(step, false);
            }
        }

        private bool OnUpdateDownload(int step, HttpInformation information)
        {
            if (!base.IsDisposed)
            {
                base.Invoke(this.m_OnUpdateDownload, new object[] { step, information });
            }
            return true;
        }

        private void OnUpdateEvent(int step, int errorCode, int count, int position, string message)
        {
            if (!base.IsDisposed)
            {
                base.Invoke(this.m_OnUpdateEvent, new object[] { step, errorCode, count, position, message });
            }
        }

        public bool PerformUpdate()
        {
            if (this.OnPerformUpdate != null)
            {
                base.Invoke(this.OnPerformUpdate, new object[] { this.m_Helper.UpdateStep + 1 });
            }
            return this.m_Helper.PerformUpdate();
        }

        public void ResetUpdate()
        {
            this.m_Helper.Reset();
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string serviceUrl, string settingName, string userName, string password)
        {
            this.m_Helper.Setup(downloadFolder, updateFolder, executePath, "", serviceUrl, settingName, null, userName, password);
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName, string userName, string password)
        {
            this.m_Helper.Setup(downloadFolder, updateFolder, executePath, args, serviceUrl, settingName, null, userName, password);
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName, string filetypes, string userName, string password)
        {
            this.m_Helper.Setup(downloadFolder, updateFolder, executePath, args, serviceUrl, settingName, filetypes, userName, password);
        }

        public void StopUpdate()
        {
            this.m_Helper.StopUpdate();
        }

        [Category("自动更新设置"), Browsable(true)]
        public string Args
        {
            get
            {
                return this.m_Helper.Args;
            }
            set
            {
                this.m_Helper.Args = value;
            }
        }

        [Category("自动更新设置"), DefaultValue(true), Browsable(true)]
        public bool AutoDownload
        {
            get
            {
                return this.m_Helper.AutoDownload;
            }
            set
            {
                this.m_Helper.AutoDownload = value;
            }
        }

        [Category("自动更新设置"), DefaultValue(false), Browsable(true)]
        public bool AutoUpdate
        {
            get
            {
                return this.m_Helper.AutoUpdate;
            }
            set
            {
                this.m_Helper.AutoUpdate = value;
            }
        }

        [DefaultValue(true), Category("自动更新设置"), Browsable(true)]
        public bool BackUp
        {
            get
            {
                return this.m_Helper.BackUp;
            }
            set
            {
                this.m_Helper.BackUp = value;
            }
        }

        [Browsable(true), Category("自动更新设置")]
        public string DownloadFolder
        {
            get
            {
                return this.m_Helper.DownloadFolder;
            }
            set
            {
                this.m_Helper.DownloadFolder = PathUtility.GetFullPath(value);
            }
        }

        [Browsable(true), Category("自动更新设置")]
        public string ExecutePath
        {
            get
            {
                return this.m_Helper.ExecutePath;
            }
            set
            {
                this.m_Helper.ExecutePath = PathUtility.GetFullPath(value);
            }
        }

        [Category("自动更新设置"), DefaultValue(true), Browsable(true)]
        public string FileTypes
        {
            get
            {
                return this.m_Helper.FileTypes;
            }
            set
            {
                this.m_Helper.FileTypes = value;
            }
        }

        public bool GetListUseWebService
        {
            get
            {
                return this.m_Helper.GetListUseWebService;
            }
            set
            {
                this.m_Helper.GetListUseWebService = value;
            }
        }

        [Browsable(true), DefaultValue(true), Category("自动更新设置")]
        public bool GZipDownload
        {
            get
            {
                return this.m_Helper.GZipDownload;
            }
            set
            {
                this.m_Helper.GZipDownload = value;
            }
        }

        public bool HasError
        {
            get
            {
                return this.m_Helper.HasError;
            }
        }

        [Category("自动更新设置"), Browsable(true), DefaultValue("DATASETXML")]
        public string HttpDownloadType
        {
            get
            {
                return this.m_Helper.HttpDownloadType;
            }
            set
            {
                this.m_Helper.HttpDownloadType = value;
            }
        }

        public bool IsDownloadFinished
        {
            get
            {
                return this.m_Helper.IsDownloadFinished;
            }
        }

        public bool IsHasUpdateFiles
        {
            get
            {
                return this.m_Helper.IsHasUpdateFiles;
            }
        }

        public bool MustUpdate
        {
            get
            {
                return this.m_Helper.MustUpdate;
            }
        }

        [Browsable(true), Category("自动更新设置"), DefaultValue((string)null)]
        public string Password
        {
            get
            {
                return this.m_Helper.Password;
            }
            set
            {
                this.m_Helper.Password = value;
            }
        }

        public bool Runing
        {
            get
            {
                return this.m_Helper.Runing;
            }
        }

        [Browsable(true), Category("自动更新设置")]
        public string ServiceUrl
        {
            get
            {
                return this.m_Helper.ServiceUrl;
            }
            set
            {
                this.m_Helper.ServiceUrl = value;
            }
        }

        [DefaultValue((string)null), Category("自动更新设置"), Browsable(true)]
        public string SettingName
        {
            get
            {
                return this.m_Helper.SettingName;
            }
            set
            {
                this.m_Helper.SettingName = value;
            }
        }

        public int UpdateFilesCount
        {
            get
            {
                return this.m_Helper.UpdateFilesCount;
            }
        }

        [Category("自动更新设置"), Browsable(true)]
        public string UpdateFolder
        {
            get
            {
                return this.m_Helper.UpdateFolder;
            }
            set
            {
                this.m_Helper.UpdateFolder = PathUtility.GetFullPath(value);
            }
        }

        public int UpdateStep
        {
            get
            {
                return this.m_Helper.UpdateStep;
            }
        }

        [Category("自动更新设置"), Browsable(true), DefaultValue((string)null)]
        public string UserName
        {
            get
            {
                return this.m_Helper.UserName;
            }
            set
            {
                this.m_Helper.UserName = value;
            }
        }

        [DefaultValue(true), Category("自动更新设置"), Browsable(true)]
        public bool UseWebService
        {
            get
            {
                return this.m_Helper.UseWebService;
            }
            set
            {
                this.m_Helper.UseWebService = value;
                this.checkBox_UseWebService.Checked = value;
            }
        }
    }
}
