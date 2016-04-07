namespace Platform.Task.QueueTask
{
    using Platform;
    using Platform.Threading;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Threading;

    internal sealed class QueueTaskSinkHelper
    {
        private ManualResetEvent m_EnqueueEvent = new ManualResetEvent(false);
        private ArrayList m_Sinks = new ArrayList();
        private Queue m_Tasks = new Queue();
        private Platform.Threading.ThreadPool m_Thread;
        private ThreadPriority m_ThreadPriority = ThreadPriority.Normal;

        public void AddSink(IQueueTaskSink sink, ThreadPriority priority)
        {
            if (!this.m_Sinks.Contains(sink))
            {
                this.m_Sinks.Add(sink);
                this.m_ThreadPriority = priority;
                this.StartThread();
            }
        }

        public void EnqueueTask(params object[] infos)
        {
            lock (this.m_Tasks.SyncRoot)
            {
                this.m_Tasks.Enqueue(infos);
            }
            this.m_EnqueueEvent.Set();
        }

        public void Flush()
        {
            if (this.m_Sinks.Count >= 1)
            {
                this.m_EnqueueEvent.Set();
                while (this.m_Tasks.Count > 0)
                {
                    Thread.Sleep(5);
                }
            }
        }

        private void QueueTaskHandler(object state)
        {
            object[] infos = null;
            while (PlatformConfig.AppRuning)
            {
                if (this.m_Sinks.Count < 1)
                {
                    break;
                }
                if (this.m_EnqueueEvent.WaitOne(1, false))
                {
                    lock (this.m_Tasks.SyncRoot)
                    {
                        if (this.m_Tasks.Count > 0)
                        {
                            infos = this.m_Tasks.Dequeue() as object[];
                        }
                        if (this.m_Tasks.Count < 1)
                        {
                            this.m_EnqueueEvent.Reset();
                        }
                    }
                    if (infos != null)
                    {
                        foreach (IQueueTaskSink sink in this.m_Sinks)
                        {
                            try
                            {
                                sink.ProcessTask(infos);
                                continue;
                            }
                            catch (Exception exception)
                            {
                                TraceHelper.WriteLine("Ö´ÐÐÈÎÎñ´íÎó£º" + exception.Message);                            
                                continue;
                            }
                        }
                        infos = null;
                    }
                }
            }
            this.m_Thread = null;
        }

        public void RemoveSink(IQueueTaskSink sink)
        {
            if (this.m_Sinks.Contains(sink))
            {
                this.m_Sinks.Remove(sink);
            }
        }

        public void RemoveSink(Type sinkType)
        {
            ArrayList list = new ArrayList();
            foreach (IQueueTaskSink sink in this.m_Sinks)
            {
                if (sink.GetType() == sinkType)
                {
                    list.Add(sink);
                }
            }
            foreach (IQueueTaskSink sink2 in list)
            {
                this.m_Sinks.Remove(sink2);
            }
        }

        private void StartThread()
        {
            if (this.m_Thread == null)
            {
                this.m_Thread = Platform.Threading.ThreadPool.Run(this.m_ThreadPriority, new WaitCallback(this.QueueTaskHandler));
            }
        }

        private void StopThread()
        {
            if (this.m_Thread != null)
            {
                this.m_Thread.Reset();
            }
        }

        public int TaskCount
        {
            get
            {
                return this.m_Tasks.Count;
            }
        }
    }
}
