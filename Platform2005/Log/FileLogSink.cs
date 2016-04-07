namespace Platform.Log
{
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using System;
    using System.Threading;

    public sealed class FileLogSink : FileLogBase, IQueueTaskSink
    {
        private string m_LogFileName;
        private string m_LogType;

        public FileLogSink(string fileName, string logType)
        {
            this.m_LogType = logType;
            this.m_LogFileName = fileName;
            QueueTaskManager.RegisterQueueTaskSink(this, logType, ThreadPriority.AboveNormal);
        }

        public void ProcessTask(params object[] infos)
        {
            try
            {
                if ((infos != null) && (infos.Length >= 0))
                {
                    string msg = (string) infos[0];
                    base.WriteLogFileMessage(msg);
                }
            }
            catch (Exception exception)
            {
                TraceHelper.WriteLine("Write FileLog:" + exception.Message);
            }
        }

        public void Write(string logMsg)
        {
            QueueTaskManager.EnqueueTask(this.m_LogType, new object[] { logMsg });
        }

        protected override string LogFileNameBase
        {
            get
            {
                return this.m_LogFileName;
            }
        }
    }
}
