namespace Platform
{
    using Platform.Configuration;
    using Platform.IO;
    using Platform.Log;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public sealed class PlatformConfig
    {
        [ConfigItem("/PlatformSettings", "DBHandlerWriteLog", null)]
        public static bool DBHandlerWriteLog = false;
        [ConfigItem("/PlatformSettings", "DBHandlerWriteLogLevel", null)]
        public static Platform.Log.LogLevel DBHandlerWriteLogLevel = Platform.Log.LogLevel.None;
        public static readonly Guid InstanceID = Guid.NewGuid();
        public static bool LogDBException = true;
        private static string m_appMutexName = "GET";
        private static ManualResetEvent m_AppRuning = new ManualResetEvent(false);
        private static Mutex m_appStartMutex;
        private static string m_BasePath;
        private static readonly string m_DoMainName;
        private static string m_ExecuteFileName;
        private static string m_ExecuteFullFileName;
        [ConfigItem("/PlatformSettings", "TextEncodingName", null)]
        public static Encoding TextEncoding = Encoding.Unicode;

        static PlatformConfig()
        {
            try
            {
                m_BasePath = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            }
            catch
            {
                m_BasePath = Path.GetFullPath(".");
            }
            try
            {
                m_ExecuteFullFileName = Application.ExecutablePath;
            }
            catch
            {
                m_ExecuteFullFileName = "";
            }
            try
            {
                m_ExecuteFileName = Path.GetFileName(Application.ExecutablePath);
            }
            catch
            {
            }
            try
            {
                m_DoMainName = PathUtility.GetValidName(AppDomain.CurrentDomain.FriendlyName);
            }
            catch
            {
                m_DoMainName = AppDomain.CurrentDomain.FriendlyName;
            }
            Application.ApplicationExit += new EventHandler(PlatformConfig.Application_ApplicationExit);
        }

        private static void AfterInitialize()
        {
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            AppRuning = false;
        }

        private static void BeforeInitialize()
        {
        }

        public static bool ExitApp()
        {
            try
            {
                if (m_appStartMutex != null)
                {
                    m_appStartMutex.ReleaseMutex();
                    m_appStartMutex.Close();
                    m_appStartMutex = null;
                }
                AppRuning = false;
                Application.Exit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool PreExitApp()
        {
            try
            {
                if (m_appStartMutex != null)
                {
                    m_appStartMutex.ReleaseMutex();
                    m_appStartMutex.Close();
                    m_appStartMutex = null;
                }
                AppRuning = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RestartApp()
        {
            try
            {
                if (m_appStartMutex != null)
                {
                    m_appStartMutex.ReleaseMutex();
                    m_appStartMutex.Close();
                    m_appStartMutex = null;
                }
                Process.Start(m_ExecuteFullFileName);
                AppRuning = false;
                Application.Exit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetAppMutex(string mutexName)
        {
            m_appMutexName = mutexName;
            bool createdNew = false;
            if (m_appStartMutex != null)
            {
                m_appStartMutex.ReleaseMutex();
                m_appStartMutex.Close();
                m_appStartMutex = null;
            }
            m_appStartMutex = new Mutex(true, m_appMutexName, out createdNew);
            if (!createdNew)
            {
                return false;
            }
            return true;
        }

        public static bool AppRuning
        {
            get
            {
                return !m_AppRuning.WaitOne(0, false);
            }
            set
            {
                if (value)
                {
                    m_AppRuning.Reset();
                }
                else
                {
                    m_AppRuning.Set();
                }
            }
        }

        public static string BasePath
        {
            get
            {
                return m_BasePath;
            }
            set
            {
                m_BasePath = value;
            }
        }

        public static string DoMainName
        {
            get
            {
                return m_DoMainName;
            }
        }

        public static string ExecuteFileName
        {
            get
            {
                return m_ExecuteFileName;
            }
        }

        public static string ExecuteFullFileName
        {
            get
            {
                return m_ExecuteFullFileName;
            }
        }
    }
}
