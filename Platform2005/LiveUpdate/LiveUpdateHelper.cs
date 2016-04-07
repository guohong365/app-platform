namespace Platform.LiveUpdate
{
    using Platform.Compress;
    using Platform.Font;
    using Platform.IO;
    using Platform.Log;
    using Platform.Net;
    using Platform.Resources;
    using Platform.Security;
    using Platform.Serialization;
    using Platform.Threading;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml;

    public sealed class LiveUpdateHelper
    {
        private string m_Args = "";
        private string[] m_Asms;
        private bool m_AutoDownload = true;
        private bool m_AutoUpdate;
        private bool m_BackUp = true;
        private string m_DestFolder = "";
        private int m_DownloadFileCount;
        private string m_ExecutePath = "";
        private string m_FileListUrl;
        private ArrayList m_Files = new ArrayList();
        private string m_FileTypes;
        private string m_GetFileUrl;
        private bool m_GetListUseWebService = true;
        private string m_GetNoteUrl;
        private bool m_GZipDownload;
        private bool m_HasError;
        private HashUtility m_Hash = new HashUtility(HashType.SHA1);
        private string m_HttpDownloadType = "DATASETXML";
        private Hashtable m_LocalFileInfos = new Hashtable();
        private string m_LocalFolder = "";
        private bool m_MustUpdate;
        private bool m_NeedRestart;
        private string m_NoteListUrl;
        private string m_Password;
        private Platform.Threading.ThreadPool m_Pool;
        private int m_Runing;
        private string m_ServiceUrl;
        private string m_SettingName;
        private object m_SyncRoot = new object();
        private HttpGetFileEventHandler m_UpdateInformationHandler;
        private int m_UpdateStep = -1;
        private string m_UserName;
        private bool m_UseWebService;
        private string m_WebServiceUrl;

        public event LiveUpdateDownloadHandler OnUpdateDownload;

        public event LiveUpdateEventHandler OnUpdateEvent;

        public LiveUpdateHelper()
        {
            this.m_UpdateInformationHandler = new HttpGetFileEventHandler(this.OnUpdateInformation);
        }

        private bool CheckFileLock(string path, FILE file)
        {
            string text = Path.GetFullPath((path + @"\" + file.PATH + @"\" + file.NAME).Replace("/", @"\")).ToUpper();
            if (this.m_Asms == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                this.m_Asms = new string[assemblies.Length];
                for (int i = 0; i < assemblies.Length; i++)
                {
                    if (assemblies[i].Location == null)
                    {
                        this.m_Asms[i] = "";
                    }
                    else
                    {
                        try
                        {
                            this.m_Asms[i] = Path.GetFullPath(assemblies[i].Location).ToUpper();
                        }
                        catch
                        {
                            this.m_Asms[i] = assemblies[i].Location.ToUpper();
                        }
                    }
                }
            }
            foreach (string text2 in this.m_Asms)
            {
                if (text == text2)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckUpdate(string path, FILE file)
        {
            string text = Path.GetFullPath((path + @"\" + file.PATH + @"\" + file.NAME).Replace("/", @"\")).ToUpper();
            if (!File.Exists(text))
            {
                return true;
            }
            FileInfo info = new FileInfo(text);
            if (this.m_LocalFileInfos.ContainsKey(text.ToUpper()))
            {
                LocalFileInfo info2 = this.m_LocalFileInfos[text.ToUpper()] as LocalFileInfo;
                if (((info2.FileTime == info.LastWriteTime) && (info2.CRC == file.CRC)) && (info.Length == file.Length))
                {
                    return false;
                }
            }
            byte[] fileHash = this.m_Hash.GetFileHash(text);
            if (fileHash == null)
            {
                return true;
            }
            if (ByteUtility.BytesToHexString(fileHash) != file.CRC)
            {
                return true;
            }
            this.m_LocalFileInfos[text.ToUpper()] = new LocalFileInfo(file.CRC, file.NAME, file.PATH, info.LastWriteTime);
            return false;
        }

        private void FinallyRun(object state)
        {
            Interlocked.Exchange(ref this.m_Runing, 0);
            if (Monitor.TryEnter(this.m_SyncRoot))
            {
                try
                {
                    if (this.m_HasError)
                    {
                        this.ResetUnStop();
                    }
                    else if (this.m_UpdateStep == 0)
                    {
                        if (this.m_Files.Count < 1)
                        {
                            this.ResetUnStop();
                        }
                        else if (this.m_AutoDownload)
                        {
                            this.PerformUpdate();
                        }
                    }
                    else if (this.m_UpdateStep == 1)
                    {
                        if (this.m_AutoUpdate || (this.m_Files.Count < 1))
                        {
                            this.PerformUpdate();
                        }
                    }
                    else if (this.m_UpdateStep == 2)
                    {
                        this.ResetUnStop();
                    }
                }
                finally
                {
                    Monitor.Exit(this.m_SyncRoot);
                }
            }
        }

        private ArrayList GetUpdateListFiles(Platform.IO.MemoryStream ms)
        {
            if (this.m_HttpDownloadType == "DATASET")
            {
                ArrayList list = new ArrayList();
                ms.Seek((long)0, SeekOrigin.Begin);
                DataSet set = SerialFormatHelper.BinaryDeserial(ms) as DataSet;
                if (set == null)
                {
                    return null;
                }
                if (set.Tables.Count >= 1)
                {
                    foreach (DataRow row in set.Tables[0].Rows)
                    {
                        list.Add(new FILE(row));
                    }
                }
                return list;
            }
            if (this.m_HttpDownloadType == "DATASETXML")
            {
                ArrayList list2 = new ArrayList();
                DataSet set2 = new DataSet();
                set2.ReadXml(ms);
                if (set2.Tables.Count >= 1)
                {
                    foreach (DataRow row2 in set2.Tables[0].Rows)
                    {
                        list2.Add(new FILE(row2));
                    }
                }
                return list2;
            }
            if ((this.m_HttpDownloadType != null) && (this.m_HttpDownloadType != "XML"))
            {
                return null;
            }
            ArrayList list3 = new ArrayList();
            XmlDocument document = new XmlDocument();
            ms.Seek((long)0, SeekOrigin.Begin);
            document.Load(ms);
            XmlNode documentElement = document.DocumentElement;
            if (documentElement.Name != "FILELIST")
            {
                return null;
            }
            foreach (XmlNode node2 in documentElement.ChildNodes)
            {
                if (node2.Name == "FILE")
                {
                    list3.Add(new FILE(node2));
                }
            }
            return list3;
        }

        private ArrayList GetUpdateListFiles(DataSet ds)
        {
            if (ds == null)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            if (ds.Tables.Count >= 1)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new FILE(row));
                }
            }
            return list;
        }

        private void InitializeLocalFileInfos()
        {
            this.m_LocalFileInfos.Clear();
            if (File.Exists(this.m_DestFolder + @"\filesinfo.dat"))
            {
                try
                {
                    using (FileStream stream = new FileStream(this.m_DestFolder + @"\filesinfo.dat", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        int num = reader.ReadInt32();
                        for (int i = 0; i < num; i++)
                        {
                            string text = reader.ReadString();
                            string text2 = reader.ReadString();
                            long ticks = reader.ReadInt64();
                            string text3 = reader.ReadString();
                            LocalFileInfo info = new LocalFileInfo();
                            info.CRC = text3.ToUpper();
                            info.FileName = text2.ToUpper();
                            info.FileTime = new DateTime(ticks);
                            info.Path = text.ToUpper();
                            string text4 = Path.GetFullPath((this.m_DestFolder + @"\" + info.Path + @"\" + info.FileName).Replace("/", @"\")).ToUpper();
                            this.m_LocalFileInfos[text4] = info;
                        }
                    }
                }
                catch
                {
                    this.m_LocalFileInfos.Clear();
                }
            }
        }

        private void InvokeUpdateEvent(int step, int errorCode, int count, int position, string message)
        {
            if (this.OnUpdateEvent != null)
            {
                this.OnUpdateEvent(step, errorCode, count, position, message);
            }
        }

        private void LiveUpdate()
        {
            try
            {
                this.m_HasError = false;
                int count = this.m_Files.Count;
                int position = 0;
                ArrayList backUpList = new ArrayList();
                ArrayList updateList = new ArrayList();
                string path = this.m_DestFolder + @"\Backup\B" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (this.m_BackUp)
                {
                    this.InvokeUpdateEvent(2, 1, count, 0, "开始备份文件……");
                    LiveUpdateLogSink.Write("开始备份文件……");
                    PathUtility.MakeDirectory(path);
                    LiveUpdateLogSink.Write("创建目录：" + path);
                    foreach (FILE file in this.m_Files)
                    {
                        this.InvokeUpdateEvent(2, 2, count, position, "正在备份：" + file.NAME + " ……");
                        PathUtility.MakeDirectory(path + @"\" + file.PATH);
                        if (!File.Exists(Path.GetFullPath(this.m_DestFolder + @"\" + file.FilePath)))
                        {
                            position++;
                            this.InvokeUpdateEvent(2, 3, count, position, "备份：" + file.NAME + " 不需要备份！");
                            continue;
                        }
                        string fullPath = Path.GetFullPath(this.m_DestFolder + @"\" + file.FilePath);
                        string destFile = Path.GetFullPath(path + @"\" + file.FilePath);
                        if (!FileUtility.Move(fullPath, destFile, true))
                        {
                            this.m_HasError = true;
                            LiveUpdateLogSink.Write("备份：" + fullPath + " 失败！");
                            this.InvokeUpdateEvent(2, -1, 0, 0, "备份：" + file.NAME + " 失败！");
                            this.LiveUpdateRollBack(this.m_DestFolder, path, this.m_LocalFolder, backUpList, updateList);
                            this.InvokeUpdateEvent(2, -1, 0, 0, "应用更新失败！");
                            return;
                        }
                        LiveUpdateLogSink.Write("备份：" + fullPath + " 成功！");
                        position++;
                        backUpList.Add(file);
                        this.InvokeUpdateEvent(2, 3, count, position, "备份：" + file.NAME + " 完成！");
                    }
                }
                LiveUpdateLogSink.Write("备份完成，开始更新");
                this.InvokeUpdateEvent(2, 1, count, 0, "开始更新文件……");
                position = 0;
                foreach (FILE file2 in this.m_Files)
                {
                    this.InvokeUpdateEvent(2, 2, count, position, "正在更新：" + file2.NAME + " ……");
                    PathUtility.MakeDirectory(this.m_DestFolder + @"\" + file2.PATH);
                    string sourceFile = Path.GetFullPath(this.m_LocalFolder + @"\" + file2.FilePath);
                    string text5 = Path.GetFullPath(this.m_DestFolder + @"\" + file2.FilePath);
                    if (!FileUtility.Copy(sourceFile, text5, true))
                    {
                        this.m_HasError = true;
                        LiveUpdateLogSink.Write("更新：" + sourceFile + " TO " + text5 + " 失败！");
                        this.InvokeUpdateEvent(2, -1, 0, 0, "更新：" + file2.NAME + " 失败！");
                        this.LiveUpdateRollBack(this.m_DestFolder, path, this.m_LocalFolder, backUpList, updateList);
                        this.InvokeUpdateEvent(2, -1, 0, 0, "应用更新失败！");
                        return;
                    }
                    if (file2.FILETYPE == "1")
                    {
                        EUDCUtility.InstallEUDC("宋体", text5);
                        EUDCUtility.InstallEUDC("Simsun", text5);
                        EUDCUtility.InstallEUDC("SystemDefaultEUDCFont", text5);
                    }
                    else if (file2.FILETYPE == "2")
                    {
                        EUDCUtility.InstallEUDC("黑体", text5);
                    }
                    LiveUpdateLogSink.Write("更新：" + sourceFile + " TO " + text5 + " 类型：" + file2.FILETYPE + " 成功！");
                    updateList.Add(file2);
                    try
                    {
                        FileInfo info = new FileInfo(text5);
                        this.m_LocalFileInfos[text5.ToUpper()] = new LocalFileInfo(file2.CRC, file2.NAME, file2.PATH, info.LastWriteTime);
                    }
                    catch
                    {
                    }
                    position++;
                    this.InvokeUpdateEvent(2, 3, count, position, "更新：" + file2.NAME + " 完成！");
                }
                FileUtility.DeleteDirectory(path);
                FileUtility.DeleteDirectory(this.m_LocalFolder);
                Interlocked.Exchange(ref this.m_UpdateStep, 2);
                LiveUpdateLogSink.Write("更新完成！\r\n");
                if (count > 0)
                {
                    this.SaveLocalFileInfo();
                }
                this.InvokeUpdateEvent(2, 0, count, 0, "应用更新成功！");
            }
            catch (Exception exception)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(2, -1, 0, 0, "应用更新失败！请尝试再次更新！" + exception.Message);
                LiveUpdateLogSink.Write("更新失败！" + exception.Message + "\r\n");
            }
        }

        private void LiveUpdateRollBack(string appPath, string backupPath, string updatePath, ArrayList backUpList, ArrayList updateList)
        {
            try
            {
                int count = updateList.Count;
                int position = 0;
                this.InvokeUpdateEvent(2, 1, count, 0, "开始回滚更新的文件……");
                LiveUpdateLogSink.Write("开始回滚更新文件");
                foreach (FILE file in updateList)
                {
                    this.InvokeUpdateEvent(2, 2, count, position, "正在回滚：" + file.NAME + " ……");
                    string fullPath = Path.GetFullPath(appPath + @"\" + file.FilePath);
                    FileUtility.Delete(fullPath);
                    position++;
                    this.InvokeUpdateEvent(2, 3, count, position, "回滚：" + file.NAME + " 完成！");
                    LiveUpdateLogSink.Write("回滚更新文件：" + fullPath + " 完成");
                }
                LiveUpdateLogSink.Write("回滚更新文件完成");
                count = backUpList.Count;
                position = 0;
                this.InvokeUpdateEvent(2, 1, count, 0, "开始回滚备份的文件……");
                LiveUpdateLogSink.Write("开始回滚备份文件");
                foreach (FILE file2 in backUpList)
                {
                    this.InvokeUpdateEvent(2, 2, count, position, "正在回滚：" + file2.NAME + " ……");
                    PathUtility.MakeDirectory(appPath + @"\" + file2.PATH);
                    string sourceFile = Path.GetFullPath(backupPath + @"\" + file2.FilePath);
                    string destFile = Path.GetFullPath(appPath + @"\" + file2.FilePath);
                    FileUtility.Move(sourceFile, destFile, true);
                    position++;
                    this.InvokeUpdateEvent(2, 3, count, position, "回滚：" + file2.NAME + " 完成！");
                    LiveUpdateLogSink.Write("回滚备份文件：" + sourceFile + " TO " + destFile + " 完成");
                }
                LiveUpdateLogSink.Write("回滚备份文件完成");
            }
            catch
            {
            }
        }

        private bool OnUpdateInformation(HttpInformation information)
        {
            if (this.OnUpdateDownload != null)
            {
                return this.OnUpdateDownload(this.m_UpdateStep + 1, information);
            }
            return true;
        }

        public bool PerformUpdate()
        {
            if (Interlocked.CompareExchange(ref this.m_Runing, 1, 0) != 0)
            {
                return false;
            }
            if (this.m_UpdateStep < 0)
            {
                this.m_DownloadFileCount = 0;
                this.m_Pool = Platform.Threading.ThreadPool.Run(new WaitCallback(this.ThreadGetUpdateFileList), new WaitCallback(this.FinallyRun));
            }
            else if (this.m_UpdateStep == 0)
            {
                this.m_Pool = Platform.Threading.ThreadPool.Run(new WaitCallback(this.ThreadDownloadUpdateFiles), new WaitCallback(this.FinallyRun));
            }
            else
            {
                this.m_Pool = Platform.Threading.ThreadPool.Run(new WaitCallback(this.ThreadPerformUpdate), new WaitCallback(this.FinallyRun));
            }
            return true;
        }

        private void ProcessUpdate()
        {
            try
            {
                this.m_HasError = false;
                int count = this.m_Files.Count;
                this.InvokeUpdateEvent(2, 1, count, 0, "正在处理更新……");
                string fullPath = Path.GetFullPath(this.m_DestFolder + @"\liveupdate.dat");
                PathUtility.MakeDirectory(this.m_DestFolder);
                FileUtility.Delete(fullPath);
                using (FileStream stream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    int id = Process.GetCurrentProcess().Id;
                    writer.Write(id);
                    writer.Write(this.m_ExecutePath);
                    writer.Write(this.m_Args);
                    writer.Write(this.m_LocalFolder);
                    writer.Write(this.m_DestFolder);
                    writer.Write(this.m_Files.Count);
                    foreach (FILE file in this.m_Files)
                    {
                        writer.Write(string.Concat(new object[] { file.FilePath, ",", file.FilePath, ",", file.FILETYPE, ",", file.PATH, ",", file.NAME, ',', file.CRC }));
                    }
                    stream.Flush();
                }
                string localFileName = Path.GetFullPath(this.m_DestFolder + @"\LiveUpdate.exe");
                if (!Platform.Resources.ResourceUtility.SaveResourceToFile(Assembly.GetExecutingAssembly(), "Platform.LiveUpdate.LiveUpdate.exe", localFileName, true))
                {
                    this.m_HasError = true;
                    this.InvokeUpdateEvent(2, -1, 0, 0, "启动更新程序错误！");
                }
                else
                {
                    Process.Start(localFileName, "\"" + fullPath + "\"");
                    Interlocked.Exchange(ref this.m_UpdateStep, 2);
                    this.InvokeUpdateEvent(2, 0, count, 0, "请等待应用更新……");
                }
            }
            catch (Exception exception)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(2, -1, 0, 0, "应用更新失败！请尝试再次更新！" + exception.Message);
            }
        }

        public void Reset()
        {
            lock (this.m_SyncRoot)
            {
                this.StopUpdate();
                Interlocked.Exchange(ref this.m_Runing, 0);
                this.m_UpdateStep = -1;
                this.m_NeedRestart = false;
                this.m_MustUpdate = false;
                this.m_HasError = false;
                this.m_Files.Clear();
                this.InitializeLocalFileInfos();
            }
        }

        private void ResetUnStop()
        {
            lock (this.m_SyncRoot)
            {
                Interlocked.Exchange(ref this.m_Runing, 0);
                this.m_UpdateStep = -1;
                this.m_NeedRestart = false;
                this.m_MustUpdate = false;
                this.m_HasError = false;
                this.m_Files.Clear();
            }
        }

        private void SaveLocalFileInfo()
        {
            try
            {
                using (FileStream stream = new FileStream(this.m_DestFolder + @"\filesinfo.dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    stream.Seek((long)0, SeekOrigin.Begin);
                    stream.SetLength((long)0);
                    BinaryWriter writer = new BinaryWriter(stream);
                    int count = this.m_LocalFileInfos.Count;
                    writer.Write(count);
                    foreach (LocalFileInfo info in this.m_LocalFileInfos.Values)
                    {
                        writer.Write(info.Path);
                        writer.Write(info.FileName);
                        writer.Write(info.FileTime.Ticks);
                        writer.Write(info.CRC);
                    }
                    stream.Flush();
                }
            }
            catch
            {
            }
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName)
        {
            this.Setup(downloadFolder, updateFolder, executePath, args, serviceUrl, settingName, null, null, null);
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName, string filetypes)
        {
            this.Setup(downloadFolder, updateFolder, executePath, args, serviceUrl, settingName, filetypes, null, null);
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName, string userName, string password)
        {
            this.Setup(downloadFolder, updateFolder, executePath, args, serviceUrl, settingName, null, userName, password);
        }

        public void Setup(string downloadFolder, string updateFolder, string executePath, string args, string serviceUrl, string settingName, string filetypes, string userName, string password)
        {
            this.m_LocalFolder = PathUtility.GetFullPath(downloadFolder);
            this.m_DestFolder = PathUtility.GetFullPath(updateFolder);
            this.m_ExecutePath = executePath;
            this.m_Args = args;
            this.ServiceUrl = serviceUrl;
            this.m_SettingName = settingName;
            this.m_UserName = userName;
            this.m_Password = password;
            this.m_FileTypes = filetypes;
            this.Reset();
        }

        public void StopUpdate()
        {
            if ((this.m_Pool != null) && !this.m_Pool.Finished)
            {
                this.m_Pool.Reset();
            }
            this.m_Pool = null;
            Interlocked.Exchange(ref this.m_Runing, 0);
        }

        public void ThreadDownloadUpdateFiles(object state)
        {
            try
            {
                this.m_HasError = false;
                int count = this.m_Files.Count;
                int position = 0;
                this.InvokeUpdateEvent(1, 1, count, 0, "正在下载更新文件……");
                PathUtility.MakeDirectory(this.m_LocalFolder);
                foreach (FILE file in this.m_Files)
                {
                    position++;
                    PathUtility.MakeDirectory(this.m_LocalFolder + @"\" + file.PATH);
                    if (!this.CheckUpdate(this.m_LocalFolder, file))
                    {
                        Interlocked.Increment(ref this.m_DownloadFileCount);
                        this.InvokeUpdateEvent(1, 2, count, position, "下载文件：" + file.NAME + " 已经下载！");
                        continue;
                    }
                    if (this.m_UseWebService)
                    {
                        if (!this.WebServiceGetFile(this.m_LocalFolder + @"\" + file.PATH + @"\" + file.NAME, file.ID, this.m_GZipDownload, this.m_UpdateInformationHandler, this.m_GZipDownload ? file.ZipLength : file.Length, new object[] { file.NAME, this.m_GZipDownload ? file.ZipLength : file.Length }))
                        {
                            this.m_HasError = true;
                            this.InvokeUpdateEvent(1, -1, 0, 0, "下载文件 " + file.NAME + " 错误！");
                            return;
                        }
                    }
                    else if (!HttpUtility.GetFile(this.m_LocalFolder + @"\" + file.PATH + @"\" + file.NAME, this.m_GetFileUrl + "?ID=" + file.ID + (this.m_GZipDownload ? "&ZIP=true" : ""), this.m_UserName, this.m_Password, this.m_GZipDownload, this.m_UpdateInformationHandler, new object[] { file.NAME, this.m_GZipDownload ? file.ZipLength : file.Length }))
                    {
                        this.m_HasError = true;
                        this.InvokeUpdateEvent(1, -1, 0, 0, "下载文件 " + file.NAME + " 错误！");
                        return;
                    }
                    this.InvokeUpdateEvent(1, 3, count, position, "下载文件：" + file.NAME + " 已经下载！");
                    this.m_DownloadFileCount++;
                }
                Interlocked.Exchange(ref this.m_UpdateStep, 1);
                this.InvokeUpdateEvent(1, 0, count, 0, "下载更新文件完成！");
            }
            catch (Exception exception)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(1, -1, 0, 0, "下载文件错误！" + exception.Message);
            }
        }

        private void ThreadGetUpdateFileList(object state)
        {
            this.m_HasError = false;
            ArrayList updateListFiles = null;
            this.InvokeUpdateEvent(0, 1, 1, 0, "正在下载列表……");
            if (this.m_GetListUseWebService)
            {
                DataSet ds = null;
                try
                {
                    ds = this.WebServiceGetFileList();
                }
                catch (Exception exception)
                {
                    this.m_HasError = true;
                    this.InvokeUpdateEvent(0, -1, 0, 0, "下载更新列表错误！" + exception.Message);
                    return;
                }
                try
                {
                    this.InvokeUpdateEvent(0, 1, 0, 0, "分析下载列表……");
                    updateListFiles = this.GetUpdateListFiles(ds);
                    goto Label_01DA;
                }
                catch (Exception exception2)
                {
                    this.m_HasError = true;
                    this.InvokeUpdateEvent(0, -1, 0, 0, "分析更新列表错误！" + exception2.Message);
                    return;
                }
            }
            Platform.IO.MemoryStream ms = null;
            try
            {
                string text = "";
                if ((this.m_FileTypes != null) && (this.m_FileTypes.Trim() != ""))
                {
                    text = text + "&FILETYPE=" + this.m_FileTypes.Trim();
                }
                ms = HttpUtility.GetFile(this.m_FileListUrl + "?SETTINGNAME=" + this.m_SettingName + (text + ((this.m_HttpDownloadType == null) ? "" : ("&DATATYPE=" + this.m_HttpDownloadType))), this.m_UserName, this.m_Password, this.m_UpdateInformationHandler, new object[] { "更新列表文件", 0 });
                if (ms == null)
                {
                    this.m_HasError = true;
                    this.InvokeUpdateEvent(0, -1, 0, 0, "下载列表错误！");
                    return;
                }
            }
            catch (Exception exception3)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(0, -1, 0, 0, "分析更新列表错误！" + exception3.Message);
                return;
            }
            try
            {
                this.InvokeUpdateEvent(0, 1, 0, 0, "分析下载列表……");
                updateListFiles = this.GetUpdateListFiles(ms);
            }
            catch (Exception exception4)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(0, -1, 0, 0, "分析更新列表错误！" + exception4.Message);
                return;
            }
        Label_01DA:
            try
            {
                if (updateListFiles == null)
                {
                    this.m_HasError = true;
                    this.InvokeUpdateEvent(0, -1, 0, 0, "更新列表格式错误！");
                    return;
                }
                int count = updateListFiles.Count;
                int position = 0;
                this.m_NeedRestart = false;
                this.m_MustUpdate = false;
                this.m_Files.Clear();
                this.InvokeUpdateEvent(0, 2, count, 0, "正在分析应用文件……");
                foreach (FILE file in updateListFiles)
                {
                    position++;
                    this.InvokeUpdateEvent(0, 3, count, position, "正在分析：" + file.NAME + "……");
                    if (this.CheckUpdate(this.m_DestFolder, file))
                    {
                        if (file.FILETYPE == null)
                        {
                            file.FILETYPE = "0";
                        }
                        this.m_Files.Add(file);
                        if (file.RESTART == "1")
                        {
                            this.m_NeedRestart = true;
                        }
                        if (file.MUST == "1")
                        {
                            this.m_MustUpdate = true;
                        }
                        if (this.CheckFileLock(this.m_DestFolder, file))
                        {
                            this.m_NeedRestart = true;
                        }
                    }
                }
                this.m_HasError = false;
                Interlocked.Exchange(ref this.m_UpdateStep, 0);
                this.InvokeUpdateEvent(0, 0, count, 0, "分析更新列表完成！");
                this.SaveLocalFileInfo();
            }
            catch (Exception exception5)
            {
                this.m_HasError = true;
                this.InvokeUpdateEvent(0, -1, 0, 0, "分析更新列表错误！" + exception5.Message);
            }
        }

        private void ThreadPerformUpdate(object state)
        {
            if (this.m_Files.Count < 1)
            {
                this.m_HasError = false;
                this.InvokeUpdateEvent(2, 1, 0, 0, "开始进行更新……");
                Interlocked.Exchange(ref this.m_UpdateStep, 2);
                this.InvokeUpdateEvent(2, 0, 0, 0, "应用不需要进行更新！");
            }
            else if (this.m_NeedRestart)
            {
                this.ProcessUpdate();
            }
            else
            {
                this.LiveUpdate();
            }
        }

        private bool WebServiceGetFile(string fileName, string id, bool zipflag, HttpGetFileEventHandler handler, int length, object state)
        {
            HttpInformation information = new HttpInformation();
            information.State = state;
            try
            {
                byte[] buffer2;
                if (handler != null)
                {
                    information.MessageType = 0;
                    information.FileName = Path.GetFileName(fileName);
                    information.Length = length;
                    handler(information);
                }
                LiveupdateService service = new LiveupdateService();
                service.Url = this.m_WebServiceUrl;
                byte[] file = service.GetFile(id, zipflag);
                if (handler != null)
                {
                    information.MessageType = 2;
                    information.ReadTotal = file.Length;
                    handler(information);
                }
                if (file == null)
                {
                    if (handler != null)
                    {
                        information.MessageType = -1;
                        information.Message = "解压数据错误！";
                        handler(information);
                    }
                    return false;
                }
                if (zipflag)
                {
                    byte[] input = file;
                    buffer2 = GZipUtility.GUnzip(input, 0, input.Length);
                    if (buffer2 == null)
                    {
                        if (handler != null)
                        {
                            information.MessageType = -1;
                            information.Message = "解压数据错误！";
                            handler(information);
                        }
                        return false;
                    }
                }
                else
                {
                    buffer2 = file;
                }
                using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    stream.SetLength((long)0);
                    stream.Write(buffer2, 0, buffer2.Length);
                    stream.Flush();
                }
                return true;
            }
            catch (Exception exception)
            {
                if (handler != null)
                {
                    information.MessageType = -1;
                    information.Message = exception.Message;
                    handler(information);
                }
                return false;
            }
        }

        private DataSet WebServiceGetFileList()
        {
            DataSet set;
            LiveupdateService service = new LiveupdateService();
            service.Url = this.m_WebServiceUrl;
            int fileListNew = 0;
            if ((this.m_FileTypes == null) || (this.m_FileTypes.Trim() == ""))
            {
                fileListNew = service.GetFileListNew(this.m_SettingName, out set);
            }
            else
            {
                fileListNew = service.GetFileListTyped(this.m_SettingName, this.m_FileTypes.Trim(), out set);
            }
            if (fileListNew == 0)
            {
                return set;
            }
            return null;
        }

        public string Args
        {
            get
            {
                return this.m_Args;
            }
            set
            {
                this.m_Args = value;
            }
        }

        public bool AutoDownload
        {
            get
            {
                return this.m_AutoDownload;
            }
            set
            {
                this.m_AutoDownload = value;
            }
        }

        public bool AutoUpdate
        {
            get
            {
                return this.m_AutoUpdate;
            }
            set
            {
                this.m_AutoUpdate = value;
            }
        }

        public bool BackUp
        {
            get
            {
                return this.m_BackUp;
            }
            set
            {
                this.m_BackUp = value;
            }
        }

        public string DownloadFolder
        {
            get
            {
                return this.m_LocalFolder;
            }
            set
            {
                this.m_LocalFolder = PathUtility.GetFullPath(value);
            }
        }

        public string ExecutePath
        {
            get
            {
                return this.m_ExecutePath;
            }
            set
            {
                this.m_ExecutePath = value;
            }
        }

        public string FileTypes
        {
            get
            {
                return this.m_FileTypes;
            }
            set
            {
                this.m_FileTypes = value;
            }
        }

        public bool GetListUseWebService
        {
            get
            {
                return this.m_GetListUseWebService;
            }
            set
            {
                this.m_GetListUseWebService = value;
            }
        }

        public bool GZipDownload
        {
            get
            {
                return this.m_GZipDownload;
            }
            set
            {
                this.m_GZipDownload = value;
            }
        }

        public bool HasError
        {
            get
            {
                return this.m_HasError;
            }
        }

        public string HttpDownloadType
        {
            get
            {
                return this.m_HttpDownloadType;
            }
            set
            {
                if (value == null)
                {
                    this.m_HttpDownloadType = null;
                }
                else
                {
                    this.m_HttpDownloadType = value.ToUpper();
                }
            }
        }

        public bool IsDownloadFinished
        {
            get
            {
                if (this.m_UpdateStep > 0)
                {
                    return (this.m_Files.Count == this.m_DownloadFileCount);
                }
                return false;
            }
        }

        public bool IsHasUpdateFiles
        {
            get
            {
                if (this.m_UpdateStep > -1)
                {
                    return (this.m_Files.Count > 0);
                }
                return false;
            }
        }

        public bool MustUpdate
        {
            get
            {
                return this.m_MustUpdate;
            }
        }

        public bool NeedRestart
        {
            get
            {
                return this.m_NeedRestart;
            }
            set
            {
                this.m_NeedRestart = value;
            }
        }

        public string Password
        {
            get
            {
                return this.m_Password;
            }
            set
            {
                this.m_Password = value;
            }
        }

        public bool Runing
        {
            get
            {
                return (this.m_Runing == 1);
            }
        }

        public string ServiceUrl
        {
            get
            {
                return this.m_ServiceUrl;
            }
            set
            {
                if (value == null)
                {
                    this.m_ServiceUrl = null;
                    this.m_GetFileUrl = null;
                    this.m_FileListUrl = null;
                    this.m_WebServiceUrl = null;
                }
                else
                {
                    this.m_ServiceUrl = value.Trim(new char[] { '/' });
                    this.m_GetFileUrl = this.m_ServiceUrl + "/GetFile.aspx";
                    this.m_FileListUrl = this.m_ServiceUrl + "/FileList.aspx";
                    this.m_WebServiceUrl = this.m_ServiceUrl + "/LiveupdateService.asmx";
                    this.m_GetNoteUrl = this.m_ServiceUrl + "/GetNote.aspx";
                    this.m_NoteListUrl = this.m_ServiceUrl + "/GetNoteList.aspx";
                }
            }
        }

        public string SettingName
        {
            get
            {
                return this.m_SettingName;
            }
            set
            {
                this.m_SettingName = value;
            }
        }

        public int UpdateFilesCount
        {
            get
            {
                if (this.m_UpdateStep < 0)
                {
                    return 0;
                }
                return this.m_Files.Count;
            }
        }

        public string UpdateFolder
        {
            get
            {
                return this.m_DestFolder;
            }
            set
            {
                this.m_DestFolder = PathUtility.GetFullPath(value);
            }
        }

        public int UpdateStep
        {
            get
            {
                return this.m_UpdateStep;
            }
        }

        public string UserName
        {
            get
            {
                return this.m_UserName;
            }
            set
            {
                this.m_UserName = value;
            }
        }

        public bool UseWebService
        {
            get
            {
                return this.m_UseWebService;
            }
            set
            {
                this.m_UseWebService = value;
            }
        }

        private class FILE
        {
            [FillField]
            public string CRC;
            public string FilePath;
            [FillField]
            public string FILETYPE;
            [FillField]
            public string ID;
            public int Length;
            [FillField]
            public string LENGTH;
            [FillField]
            public string MUST;
            [FillField]
            public string NAME;
            [FillField]
            public string PATH;
            [FillField]
            public string RESTART;
            [FillField]
            public string UPDATETIME;
            public int ZipLength;
            [FillField]
            public string ZIPLENGTH;

            public FILE(DataRow row)
            {
                this.NAME = "";
                this.PATH = "";
                this.ID = "";
                this.CRC = "";
                this.FILETYPE = "";
                this.RESTART = "";
                this.MUST = "";
                this.LENGTH = "";
                this.ZIPLENGTH = "";
                this.UPDATETIME = "";
                FillUtility.FillFields(this, row);
                try
                {
                    this.Length = int.Parse(this.LENGTH);
                }
                catch
                {
                    this.Length = 0;
                }
                try
                {
                    this.ZipLength = int.Parse(this.ZIPLENGTH);
                }
                catch
                {
                    this.ZipLength = 0;
                }
                this.FilePath = this.PATH.Replace("/", @"\") + @"\" + this.NAME.Replace("/", @"\");
            }

            public FILE(XmlNode node)
            {
                this.NAME = "";
                this.PATH = "";
                this.ID = "";
                this.CRC = "";
                this.FILETYPE = "";
                this.RESTART = "";
                this.MUST = "";
                this.LENGTH = "";
                this.ZIPLENGTH = "";
                this.UPDATETIME = "";
                FillUtility.FillFields(this, node);
                try
                {
                    this.Length = int.Parse(this.LENGTH);
                }
                catch
                {
                    this.Length = 0;
                }
                try
                {
                    this.ZipLength = int.Parse(this.ZIPLENGTH);
                }
                catch
                {
                    this.ZipLength = 0;
                }
                this.FilePath = this.PATH.Replace("/", @"\") + @"\" + this.NAME.Replace("/", @"\");
            }
        }

        private class LocalFileInfo
        {
            public string CRC;
            public string FileName;
            public DateTime FileTime;
            public string Path;

            public LocalFileInfo()
            {
            }

            public LocalFileInfo(string crc, string fileName, string path, DateTime fileTime)
            {
                this.CRC = crc;
                this.FileName = fileName;
                this.Path = path;
                this.FileTime = fileTime;
            }
        }
    }
}
