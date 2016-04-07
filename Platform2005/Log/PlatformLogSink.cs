namespace Platform.Log
{
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using Platform.Utils;
    using System;

    [QueueTaskSink("PlatformLog")]
    public sealed class PlatformLogSink : FileLogBase, IQueueTaskSink
    {
        static PlatformLogSink()
        {
            PlatformLogSink sink = new PlatformLogSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        private PlatformLogSink()
        {
        }

        public void ProcessTask(params object[] infos)
        {
            if (LogConfig.WritePlatformLog)
            {
                try
                {
                    if ((infos != null) && (infos.Length >= 1))
                    {
                        string msg = (string)infos[0];
                        base.WriteLogFileMessage(msg);
                    }
                }
                catch (Exception exception)
                {
                    TraceHelper.WriteLine("Write PlatformLog:" + exception.Message);
                }
            }
        }

        public static void Write(string message)
        {
            TraceHelper.WriteLine(message);
            string text = PlatformDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "£º" + message;
            QueueTaskManager.EnqueueTask("PlatformLog", new object[] { text });
        }

        protected override string LogFileNameBase
        {
            get
            {
                return "Platform";
            }
        }

        public static int TaskCount
        {
            get
            {
                return QueueTaskManager.GetTaskCount("PlatformLog");
            }
        }
    }
}
