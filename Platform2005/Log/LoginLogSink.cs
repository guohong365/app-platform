namespace Platform.Log
{
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using Platform.Utils;
    using System;

    [QueueTaskSink("LoginLog")]
    public sealed class LoginLogSink : FileLogBase, IQueueTaskSink
    {
        static LoginLogSink()
        {
            LoginLogSink sink = new LoginLogSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        public void ProcessTask(params object[] infos)
        {
            if (LogConfig.WriteLoginLog)
            {
                try
                {
                    if ((infos != null) && (infos.Length >= 1))
                    {
                        string text = infos[0] as string;
                        object[] objArray = infos[1] as object[];
                        if ((objArray != null) && (objArray.Length >= 5))
                        {
                            string text2 = objArray[0] as string;
                            string text3 = objArray[1] as string;
                            string text4 = objArray[2] as string;
                            string text5 = objArray[3] as string;
                            string[] textArray = objArray[4] as string[];
                            string text6 = "";
                            if (textArray != null)
                            {
                                foreach (string text7 in textArray)
                                {
                                    if (text7 == null)
                                    {
                                        text6 = text6 + "\r\n(null)";
                                    }
                                    else
                                    {
                                        text6 = text6 + "\r\n" + text7;
                                    }
                                }
                            }
                            string msg = "================\r\n记录时间：" + text + "\r\n操作类型：" + text2 + "\r\n用户信息：" + text3 + "\r\n返回值　：" + text4 + "\r\n处理信息：" + text5 + "\r\n处理信息：" + text6;
                            base.WriteLogFileMessage(msg);
                        }
                    }
                }
                catch (Exception exception)
                {
                    TraceHelper.WriteLine("Write LoginLog:" + exception.Message);
                }
            }
        }

        public static void Write(string loginType, string userInfo, string returnCode, string platformMsg, string[] loginMsg)
        {
            QueueTaskManager.EnqueueTask("LoginLog", new object[] { PlatformDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), new object[] { loginType, userInfo, returnCode, platformMsg, loginMsg } });
        }

        protected override string LogFileNameBase
        {
            get
            {
                return "Login";
            }
        }

        public static int TaskCount
        {
            get
            {
                return QueueTaskManager.GetTaskCount("LoginLog");
            }
        }
    }
}
