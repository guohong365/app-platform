namespace Platform.LiveUpdate
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Web.Services;
    using System.Web.Services.Description;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;

    [WebServiceBinding(Name = "LiveupdateServiceSoap", Namespace = "http://www.platform.com.cn/liveupdate/"), GeneratedCode("System.Web.Services", "2.0.50727.312"), DebuggerStepThrough, DesignerCategory("code")]
    public class LiveupdateService : SoapHttpClientProtocol
    {
        private SendOrPostCallback AddNoteOperationCompleted;
        private SendOrPostCallback CheckFileOperationCompleted;
        private SendOrPostCallback CleanUpdateFilesOperationCompleted;
        private SendOrPostCallback CreateSettingNameOperationCompleted;
        private SendOrPostCallback DeleteSettingNameOperationCompleted;
        private SendOrPostCallback GetFileListNewOperationCompleted;
        private SendOrPostCallback GetFileListOperationCompleted;
        private SendOrPostCallback GetFileListTypedOperationCompleted;
        private SendOrPostCallback GetFileOperationCompleted;
        private SendOrPostCallback GetFullFileListOperationCompleted;
        private SendOrPostCallback GetNoteListOperationCompleted;
        private SendOrPostCallback GetNoteOperationCompleted;
        private SendOrPostCallback GetSettingNamesOperationCompleted;
        private SendOrPostCallback LoginOperationCompleted;
        private SendOrPostCallback RemoveFileByIdOperationCompleted;
        private SendOrPostCallback UpdateFileContentOperationCompleted;
        private SendOrPostCallback UpdateFileOperationCompleted;
        private SendOrPostCallback UpdateSettingNameOperationCompleted;
        private SendOrPostCallback UploadFileOperationCompleted;
        private bool useDefaultCredentialsSetExplicitly;

        public event AddNoteCompletedEventHandler AddNoteCompleted;

        public event CheckFileCompletedEventHandler CheckFileCompleted;

        public event CleanUpdateFilesCompletedEventHandler CleanUpdateFilesCompleted;

        public event CreateSettingNameCompletedEventHandler CreateSettingNameCompleted;

        public event DeleteSettingNameCompletedEventHandler DeleteSettingNameCompleted;

        public event GetFileCompletedEventHandler GetFileCompleted;

        public event GetFileListCompletedEventHandler GetFileListCompleted;

        public event GetFileListNewCompletedEventHandler GetFileListNewCompleted;

        public event GetFileListTypedCompletedEventHandler GetFileListTypedCompleted;

        public event GetFullFileListCompletedEventHandler GetFullFileListCompleted;

        public event GetNoteCompletedEventHandler GetNoteCompleted;

        public event GetNoteListCompletedEventHandler GetNoteListCompleted;

        public event GetSettingNamesCompletedEventHandler GetSettingNamesCompleted;

        public event LoginCompletedEventHandler LoginCompleted;

        public event RemoveFileByIdCompletedEventHandler RemoveFileByIdCompleted;

        public event UpdateFileCompletedEventHandler UpdateFileCompleted;

        public event UpdateFileContentCompletedEventHandler UpdateFileContentCompleted;

        public event UpdateSettingNameCompletedEventHandler UpdateSettingNameCompleted;

        public event UploadFileCompletedEventHandler UploadFileCompleted;

        public LiveupdateService()
        {
            this.Url = "http://localhost/LiveUpdate/LiveupdateService.asmx";
            if (this.IsLocalFileSystemWebService(this.Url))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/AddNote", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int AddNote(string id, string title, string note)
        {
            return (int)base.Invoke("AddNote", new object[] { id, title, note })[0];
        }

        public void AddNoteAsync(string id, string title, string note)
        {
            this.AddNoteAsync(id, title, note, null);
        }

        public void AddNoteAsync(string id, string title, string note, object userState)
        {
            if (this.AddNoteOperationCompleted == null)
            {
                this.AddNoteOperationCompleted = new SendOrPostCallback(this.OnAddNoteOperationCompleted);
            }
            base.InvokeAsync("AddNote", new object[] { id, title, note }, this.AddNoteOperationCompleted, userState);
        }

        public IAsyncResult BeginAddNote(string id, string title, string note, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("AddNote", new object[] { id, title, note }, callback, asyncState);
        }

        public IAsyncResult BeginCheckFile(string id, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("CheckFile", new object[] { id }, callback, asyncState);
        }

        public IAsyncResult BeginCleanUpdateFiles(string settingName, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("CleanUpdateFiles", new object[] { settingName }, callback, asyncState);
        }

        public IAsyncResult BeginCreateSettingName(string settingName, string updateFolder, string updateZipFolder, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("CreateSettingName", new object[] { settingName, updateFolder, updateZipFolder }, callback, asyncState);
        }

        public IAsyncResult BeginDeleteSettingName(string settingName, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("DeleteSettingName", new object[] { settingName }, callback, asyncState);
        }

        public IAsyncResult BeginGetFile(string id, bool zipflag, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetFile", new object[] { id, zipflag }, callback, asyncState);
        }

        public IAsyncResult BeginGetFileList(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetFileList", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginGetFileListNew(string settingName, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetFileListNew", new object[] { settingName }, callback, asyncState);
        }

        public IAsyncResult BeginGetFileListTyped(string settingName, string filetype, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetFileListTyped", new object[] { settingName, filetype }, callback, asyncState);
        }

        public IAsyncResult BeginGetFullFileList(string settingName, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetFullFileList", new object[] { settingName }, callback, asyncState);
        }

        public IAsyncResult BeginGetNote(string id, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetNote", new object[] { id }, callback, asyncState);
        }

        public IAsyncResult BeginGetNoteList(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetNoteList", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginGetSettingNames(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetSettingNames", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginLogin(string userName, string password, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("Login", new object[] { userName, password }, callback, asyncState);
        }

        public IAsyncResult BeginRemoveFileById(string id, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("RemoveFileById", new object[] { id }, callback, asyncState);
        }

        public IAsyncResult BeginUpdateFile(string id, bool restart, bool must, int fileType, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("UpdateFile", new object[] { id, restart, must, fileType }, callback, asyncState);
        }

        public IAsyncResult BeginUpdateFileContent(string id, byte[] fileData, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("UpdateFileContent", new object[] { id, fileData }, callback, asyncState);
        }

        public IAsyncResult BeginUpdateSettingName(string settingName, string updateFolder, string updateZipFolder, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("UpdateSettingName", new object[] { settingName, updateFolder, updateZipFolder }, callback, asyncState);
        }

        public IAsyncResult BeginUploadFile(string settingName, string path, string fileName, bool restart, bool must, int fileType, byte[] fileData, AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("UploadFile", new object[] { settingName, path, fileName, restart, must, fileType, fileData }, callback, asyncState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/CheckFile", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string CheckFile(string id)
        {
            return (string)base.Invoke("CheckFile", new object[] { id })[0];
        }

        public void CheckFileAsync(string id)
        {
            this.CheckFileAsync(id, null);
        }

        public void CheckFileAsync(string id, object userState)
        {
            if (this.CheckFileOperationCompleted == null)
            {
                this.CheckFileOperationCompleted = new SendOrPostCallback(this.OnCheckFileOperationCompleted);
            }
            base.InvokeAsync("CheckFile", new object[] { id }, this.CheckFileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/CleanUpdateFiles", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CleanUpdateFiles(string settingName)
        {
            return (int)base.Invoke("CleanUpdateFiles", new object[] { settingName })[0];
        }

        public void CleanUpdateFilesAsync(string settingName)
        {
            this.CleanUpdateFilesAsync(settingName, null);
        }

        public void CleanUpdateFilesAsync(string settingName, object userState)
        {
            if (this.CleanUpdateFilesOperationCompleted == null)
            {
                this.CleanUpdateFilesOperationCompleted = new SendOrPostCallback(this.OnCleanUpdateFilesOperationCompleted);
            }
            base.InvokeAsync("CleanUpdateFiles", new object[] { settingName }, this.CleanUpdateFilesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/CreateSettingName", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CreateSettingName(string settingName, string updateFolder, string updateZipFolder)
        {
            return (int)base.Invoke("CreateSettingName", new object[] { settingName, updateFolder, updateZipFolder })[0];
        }

        public void CreateSettingNameAsync(string settingName, string updateFolder, string updateZipFolder)
        {
            this.CreateSettingNameAsync(settingName, updateFolder, updateZipFolder, null);
        }

        public void CreateSettingNameAsync(string settingName, string updateFolder, string updateZipFolder, object userState)
        {
            if (this.CreateSettingNameOperationCompleted == null)
            {
                this.CreateSettingNameOperationCompleted = new SendOrPostCallback(this.OnCreateSettingNameOperationCompleted);
            }
            base.InvokeAsync("CreateSettingName", new object[] { settingName, updateFolder, updateZipFolder }, this.CreateSettingNameOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/DeleteSettingName", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int DeleteSettingName(string settingName)
        {
            return (int)base.Invoke("DeleteSettingName", new object[] { settingName })[0];
        }

        public void DeleteSettingNameAsync(string settingName)
        {
            this.DeleteSettingNameAsync(settingName, null);
        }

        public void DeleteSettingNameAsync(string settingName, object userState)
        {
            if (this.DeleteSettingNameOperationCompleted == null)
            {
                this.DeleteSettingNameOperationCompleted = new SendOrPostCallback(this.OnDeleteSettingNameOperationCompleted);
            }
            base.InvokeAsync("DeleteSettingName", new object[] { settingName }, this.DeleteSettingNameOperationCompleted, userState);
        }

        public int EndAddNote(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public string EndCheckFile(IAsyncResult asyncResult)
        {
            return (string)base.EndInvoke(asyncResult)[0];
        }

        public int EndCleanUpdateFiles(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndCreateSettingName(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndDeleteSettingName(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public byte[] EndGetFile(IAsyncResult asyncResult)
        {
            return (byte[])base.EndInvoke(asyncResult)[0];
        }

        public int EndGetFileList(IAsyncResult asyncResult, out DataSet dsList)
        {
            object[] objArray = base.EndInvoke(asyncResult);
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public int EndGetFileListNew(IAsyncResult asyncResult, out DataSet dsList)
        {
            object[] objArray = base.EndInvoke(asyncResult);
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public int EndGetFileListTyped(IAsyncResult asyncResult, out DataSet dsList)
        {
            object[] objArray = base.EndInvoke(asyncResult);
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public int EndGetFullFileList(IAsyncResult asyncResult, out DataSet dsList)
        {
            object[] objArray = base.EndInvoke(asyncResult);
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public bool EndGetNote(IAsyncResult asyncResult, out string title, out string note)
        {
            object[] objArray = base.EndInvoke(asyncResult);
            title = (string)objArray[1];
            note = (string)objArray[2];
            return (bool)objArray[0];
        }

        public DataSet EndGetNoteList(IAsyncResult asyncResult)
        {
            return (DataSet)base.EndInvoke(asyncResult)[0];
        }

        public string[] EndGetSettingNames(IAsyncResult asyncResult)
        {
            return (string[])base.EndInvoke(asyncResult)[0];
        }

        public bool EndLogin(IAsyncResult asyncResult)
        {
            return (bool)base.EndInvoke(asyncResult)[0];
        }

        public int EndRemoveFileById(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndUpdateFile(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndUpdateFileContent(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndUpdateSettingName(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        public int EndUploadFile(IAsyncResult asyncResult)
        {
            return (int)base.EndInvoke(asyncResult)[0];
        }

        [return: XmlElement(DataType = "base64Binary")]
        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetFile", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public byte[] GetFile(string id, bool zipflag)
        {
            return (byte[])base.Invoke("GetFile", new object[] { id, zipflag })[0];
        }

        public void GetFileAsync(string id, bool zipflag)
        {
            this.GetFileAsync(id, zipflag, null);
        }

        public void GetFileAsync(string id, bool zipflag, object userState)
        {
            if (this.GetFileOperationCompleted == null)
            {
                this.GetFileOperationCompleted = new SendOrPostCallback(this.OnGetFileOperationCompleted);
            }
            base.InvokeAsync("GetFile", new object[] { id, zipflag }, this.GetFileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetFileList", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetFileList(out DataSet dsList)
        {
            object[] objArray = base.Invoke("GetFileList", new object[0]);
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public void GetFileListAsync()
        {
            this.GetFileListAsync(null);
        }

        public void GetFileListAsync(object userState)
        {
            if (this.GetFileListOperationCompleted == null)
            {
                this.GetFileListOperationCompleted = new SendOrPostCallback(this.OnGetFileListOperationCompleted);
            }
            base.InvokeAsync("GetFileList", new object[0], this.GetFileListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetFileListNew", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetFileListNew(string settingName, out DataSet dsList)
        {
            object[] objArray = base.Invoke("GetFileListNew", new object[] { settingName });
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public void GetFileListNewAsync(string settingName)
        {
            this.GetFileListNewAsync(settingName, null);
        }

        public void GetFileListNewAsync(string settingName, object userState)
        {
            if (this.GetFileListNewOperationCompleted == null)
            {
                this.GetFileListNewOperationCompleted = new SendOrPostCallback(this.OnGetFileListNewOperationCompleted);
            }
            base.InvokeAsync("GetFileListNew", new object[] { settingName }, this.GetFileListNewOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetFileListTyped", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetFileListTyped(string settingName, string filetype, out DataSet dsList)
        {
            object[] objArray = base.Invoke("GetFileListTyped", new object[] { settingName, filetype });
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public void GetFileListTypedAsync(string settingName, string filetype)
        {
            this.GetFileListTypedAsync(settingName, filetype, null);
        }

        public void GetFileListTypedAsync(string settingName, string filetype, object userState)
        {
            if (this.GetFileListTypedOperationCompleted == null)
            {
                this.GetFileListTypedOperationCompleted = new SendOrPostCallback(this.OnGetFileListTypedOperationCompleted);
            }
            base.InvokeAsync("GetFileListTyped", new object[] { settingName, filetype }, this.GetFileListTypedOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetFullFileList", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetFullFileList(string settingName, out DataSet dsList)
        {
            object[] objArray = base.Invoke("GetFullFileList", new object[] { settingName });
            dsList = (DataSet)objArray[1];
            return (int)objArray[0];
        }

        public void GetFullFileListAsync(string settingName)
        {
            this.GetFullFileListAsync(settingName, null);
        }

        public void GetFullFileListAsync(string settingName, object userState)
        {
            if (this.GetFullFileListOperationCompleted == null)
            {
                this.GetFullFileListOperationCompleted = new SendOrPostCallback(this.OnGetFullFileListOperationCompleted);
            }
            base.InvokeAsync("GetFullFileList", new object[] { settingName }, this.GetFullFileListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetNote", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool GetNote(string id, out string title, out string note)
        {
            object[] objArray = base.Invoke("GetNote", new object[] { id });
            title = (string)objArray[1];
            note = (string)objArray[2];
            return (bool)objArray[0];
        }

        public void GetNoteAsync(string id)
        {
            this.GetNoteAsync(id, null);
        }

        public void GetNoteAsync(string id, object userState)
        {
            if (this.GetNoteOperationCompleted == null)
            {
                this.GetNoteOperationCompleted = new SendOrPostCallback(this.OnGetNoteOperationCompleted);
            }
            base.InvokeAsync("GetNote", new object[] { id }, this.GetNoteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetNoteList", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public DataSet GetNoteList()
        {
            return (DataSet)base.Invoke("GetNoteList", new object[0])[0];
        }

        public void GetNoteListAsync()
        {
            this.GetNoteListAsync(null);
        }

        public void GetNoteListAsync(object userState)
        {
            if (this.GetNoteListOperationCompleted == null)
            {
                this.GetNoteListOperationCompleted = new SendOrPostCallback(this.OnGetNoteListOperationCompleted);
            }
            base.InvokeAsync("GetNoteList", new object[0], this.GetNoteListOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/GetSettingNames", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetSettingNames()
        {
            return (string[])base.Invoke("GetSettingNames", new object[0])[0];
        }

        public void GetSettingNamesAsync()
        {
            this.GetSettingNamesAsync(null);
        }

        public void GetSettingNamesAsync(object userState)
        {
            if (this.GetSettingNamesOperationCompleted == null)
            {
                this.GetSettingNamesOperationCompleted = new SendOrPostCallback(this.OnGetSettingNamesOperationCompleted);
            }
            base.InvokeAsync("GetSettingNames", new object[0], this.GetSettingNamesOperationCompleted, userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if ((url == null) || (url == string.Empty))
            {
                return false;
            }
            Uri uri = new Uri(url);
            return ((uri.Port >= 0x400) && (string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0));
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/Login", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool Login(string userName, string password)
        {
            return (bool)base.Invoke("Login", new object[] { userName, password })[0];
        }

        public void LoginAsync(string userName, string password)
        {
            this.LoginAsync(userName, password, null);
        }

        public void LoginAsync(string userName, string password, object userState)
        {
            if (this.LoginOperationCompleted == null)
            {
                this.LoginOperationCompleted = new SendOrPostCallback(this.OnLoginOperationCompleted);
            }
            base.InvokeAsync("Login", new object[] { userName, password }, this.LoginOperationCompleted, userState);
        }

        private void OnAddNoteOperationCompleted(object arg)
        {
            if (this.AddNoteCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.AddNoteCompleted(this, new AddNoteCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnCheckFileOperationCompleted(object arg)
        {
            if (this.CheckFileCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.CheckFileCompleted(this, new CheckFileCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnCleanUpdateFilesOperationCompleted(object arg)
        {
            if (this.CleanUpdateFilesCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.CleanUpdateFilesCompleted(this, new CleanUpdateFilesCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnCreateSettingNameOperationCompleted(object arg)
        {
            if (this.CreateSettingNameCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.CreateSettingNameCompleted(this, new CreateSettingNameCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnDeleteSettingNameOperationCompleted(object arg)
        {
            if (this.DeleteSettingNameCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.DeleteSettingNameCompleted(this, new DeleteSettingNameCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetFileListNewOperationCompleted(object arg)
        {
            if (this.GetFileListNewCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetFileListNewCompleted(this, new GetFileListNewCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetFileListOperationCompleted(object arg)
        {
            if (this.GetFileListCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetFileListCompleted(this, new GetFileListCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetFileListTypedOperationCompleted(object arg)
        {
            if (this.GetFileListTypedCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetFileListTypedCompleted(this, new GetFileListTypedCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetFileOperationCompleted(object arg)
        {
            if (this.GetFileCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetFileCompleted(this, new GetFileCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetFullFileListOperationCompleted(object arg)
        {
            if (this.GetFullFileListCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetFullFileListCompleted(this, new GetFullFileListCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetNoteListOperationCompleted(object arg)
        {
            if (this.GetNoteListCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetNoteListCompleted(this, new GetNoteListCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetNoteOperationCompleted(object arg)
        {
            if (this.GetNoteCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetNoteCompleted(this, new GetNoteCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnGetSettingNamesOperationCompleted(object arg)
        {
            if (this.GetSettingNamesCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.GetSettingNamesCompleted(this, new GetSettingNamesCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnLoginOperationCompleted(object arg)
        {
            if (this.LoginCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.LoginCompleted(this, new LoginCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnRemoveFileByIdOperationCompleted(object arg)
        {
            if (this.RemoveFileByIdCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.RemoveFileByIdCompleted(this, new RemoveFileByIdCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnUpdateFileContentOperationCompleted(object arg)
        {
            if (this.UpdateFileContentCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.UpdateFileContentCompleted(this, new UpdateFileContentCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnUpdateFileOperationCompleted(object arg)
        {
            if (this.UpdateFileCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.UpdateFileCompleted(this, new UpdateFileCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnUpdateSettingNameOperationCompleted(object arg)
        {
            if (this.UpdateSettingNameCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.UpdateSettingNameCompleted(this, new UpdateSettingNameCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        private void OnUploadFileOperationCompleted(object arg)
        {
            if (this.UploadFileCompleted != null)
            {
                InvokeCompletedEventArgs args = (InvokeCompletedEventArgs)arg;
                this.UploadFileCompleted(this, new UploadFileCompletedEventArgs(args.Results, args.Error, args.Cancelled, args.UserState));
            }
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/RemoveFileById", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int RemoveFileById(string id)
        {
            return (int)base.Invoke("RemoveFileById", new object[] { id })[0];
        }

        public void RemoveFileByIdAsync(string id)
        {
            this.RemoveFileByIdAsync(id, null);
        }

        public void RemoveFileByIdAsync(string id, object userState)
        {
            if (this.RemoveFileByIdOperationCompleted == null)
            {
                this.RemoveFileByIdOperationCompleted = new SendOrPostCallback(this.OnRemoveFileByIdOperationCompleted);
            }
            base.InvokeAsync("RemoveFileById", new object[] { id }, this.RemoveFileByIdOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/UpdateFile", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int UpdateFile(string id, bool restart, bool must, int fileType)
        {
            return (int)base.Invoke("UpdateFile", new object[] { id, restart, must, fileType })[0];
        }

        public void UpdateFileAsync(string id, bool restart, bool must, int fileType)
        {
            this.UpdateFileAsync(id, restart, must, fileType, null);
        }

        public void UpdateFileAsync(string id, bool restart, bool must, int fileType, object userState)
        {
            if (this.UpdateFileOperationCompleted == null)
            {
                this.UpdateFileOperationCompleted = new SendOrPostCallback(this.OnUpdateFileOperationCompleted);
            }
            base.InvokeAsync("UpdateFile", new object[] { id, restart, must, fileType }, this.UpdateFileOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/UpdateFileContent", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int UpdateFileContent(string id, [XmlElement(DataType = "base64Binary")] byte[] fileData)
        {
            return (int)base.Invoke("UpdateFileContent", new object[] { id, fileData })[0];
        }

        public void UpdateFileContentAsync(string id, byte[] fileData)
        {
            this.UpdateFileContentAsync(id, fileData, null);
        }

        public void UpdateFileContentAsync(string id, byte[] fileData, object userState)
        {
            if (this.UpdateFileContentOperationCompleted == null)
            {
                this.UpdateFileContentOperationCompleted = new SendOrPostCallback(this.OnUpdateFileContentOperationCompleted);
            }
            base.InvokeAsync("UpdateFileContent", new object[] { id, fileData }, this.UpdateFileContentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/UpdateSettingName", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int UpdateSettingName(string settingName, string updateFolder, string updateZipFolder)
        {
            return (int)base.Invoke("UpdateSettingName", new object[] { settingName, updateFolder, updateZipFolder })[0];
        }

        public void UpdateSettingNameAsync(string settingName, string updateFolder, string updateZipFolder)
        {
            this.UpdateSettingNameAsync(settingName, updateFolder, updateZipFolder, null);
        }

        public void UpdateSettingNameAsync(string settingName, string updateFolder, string updateZipFolder, object userState)
        {
            if (this.UpdateSettingNameOperationCompleted == null)
            {
                this.UpdateSettingNameOperationCompleted = new SendOrPostCallback(this.OnUpdateSettingNameOperationCompleted);
            }
            base.InvokeAsync("UpdateSettingName", new object[] { settingName, updateFolder, updateZipFolder }, this.UpdateSettingNameOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.platform.com.cn/liveupdate/UploadFile", RequestNamespace = "http://www.platform.com.cn/liveupdate/", ResponseNamespace = "http://www.platform.com.cn/liveupdate/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int UploadFile(string settingName, string path, string fileName, bool restart, bool must, int fileType, [XmlElement(DataType = "base64Binary")] byte[] fileData)
        {
            return (int)base.Invoke("UploadFile", new object[] { settingName, path, fileName, restart, must, fileType, fileData })[0];
        }

        public void UploadFileAsync(string settingName, string path, string fileName, bool restart, bool must, int fileType, byte[] fileData)
        {
            this.UploadFileAsync(settingName, path, fileName, restart, must, fileType, fileData, null);
        }

        public void UploadFileAsync(string settingName, string path, string fileName, bool restart, bool must, int fileType, byte[] fileData, object userState)
        {
            if (this.UploadFileOperationCompleted == null)
            {
                this.UploadFileOperationCompleted = new SendOrPostCallback(this.OnUploadFileOperationCompleted);
            }
            base.InvokeAsync("UploadFile", new object[] { settingName, path, fileName, restart, must, fileType, fileData }, this.UploadFileOperationCompleted, userState);
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly) && !this.IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
    }
}
