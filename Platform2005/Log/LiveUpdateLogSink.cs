namespace Platform.Log
{
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using Platform.Utils;
    using System;

    [QueueTaskSink("LiveUpdateLog")]
    public sealed class LiveUpdateLogSink : FileLogBase, IQueueTaskSink
    {
        static LiveUpdateLogSink()
        {
            LiveUpdateLogSink sink = new LiveUpdateLogSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        private LiveUpdateLogSink()
        {
        }

        public void ProcessTask(params object[] infos)
        {
            if (LogConfig.WriteLiveUpdateLog)
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
                    TraceHelper.WriteLine("Write LiveUpdateLog:" + exception.Message);
                }
            }
        }

        public static void Write(string message)
        {
            string text = PlatformDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "£º" + message;
            QueueTaskManager.EnqueueTask("LiveUpdateLog", new object[] { text });
        }

        protected override string LogFileNameBase
        {
            get
            {
                return "LiveUpdate";
            }
        }

        public static int TaskCount
        {
            get
            {
                return QueueTaskManager.GetTaskCount("LiveUpdateLog");
            }
        }
    }
}
