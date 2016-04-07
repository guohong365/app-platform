namespace Platform.Log
{
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using Platform.Utils;
    using System;

    [QueueTaskSink("OperationLog")]
    public sealed class OperationLogSink : FileLogBase, IQueueTaskSink
    {
        static OperationLogSink()
        {
            OperationLogSink sink = new OperationLogSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        public void ProcessTask(params object[] infos)
        {
            try
            {
                if ((infos != null) && (infos.Length >= 1))
                {
                    string logMessage = "================\r\n记录时间：" + ((string)infos[0]) + "\r\n用户信息：" + ((string)infos[1]) + "\r\n业务操作：" + ((string)infos[2]) + "\r\n操作信息：" + ((string)infos[3]);
                    TraceHelper.WriteLine(logMessage);
                    if (LogConfig.WriteOperationLog)
                    {
                        base.WriteLogFileMessage(logMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                TraceHelper.WriteLine("Write OperationLog:" + exception.Message);
            }
        }

        public static void Write(string userInfo, string operation, string message)
        {
            QueueTaskManager.EnqueueTask("OperationLog", new object[] { PlatformDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), userInfo, operation, message });
        }

        protected override string LogFileNameBase
        {
            get
            {
                return "Operation";
            }
        }

        public static int TaskCount
        {
            get
            {
                return QueueTaskManager.GetTaskCount("OperationLog");
            }
        }
    }
}
