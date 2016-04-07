namespace Platform.CSS.Communication
{
    using Platform;
    using Platform.CSS;
    using Platform.Identity;
    using Platform.IO;
    using Platform.Threading;
    using Platform.Tracing;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class ServerCacheResultManager
    {
        private static Hashtable m_Sinks = new Hashtable();

        public static void Flush()
        {
            foreach (SinkHelper helper in m_Sinks.Values)
            {
                helper.Flush();
            }
        }

        public static void GetServerResult(string settingName, IUser user, Guid packetID, int threadID, out MemoryStream result)
        {
            SinkHelper helper = m_Sinks[settingName] as SinkHelper;
            if (helper == null)
            {
                result = null;
            }
            else
            {
                helper.GetServerResult(user, packetID, threadID, out result);
            }
        }

        public static void RegisterServerCacheResultSink(string settingName, IServerCacheResultSink sink)
        {
            m_Sinks[settingName] = new SinkHelper(sink);
        }

        public static void SaveServerResult(string settingName, IUser user, Guid packetID, int threadID, MemoryStream result)
        {
            SinkHelper helper = m_Sinks[settingName] as SinkHelper;
            if (helper != null)
            {
                helper.SaveServerResult(user, packetID, threadID, result);
            }
        }

        private class SinkHelper
        {
            private object[] m_CurrentData;
            private Queue m_Queue = new Queue();
            private object m_ReadSyncRoot = new object();
            private IServerCacheResultSink m_Sink;
            private Platform.Threading.ThreadPool m_Thread;

            public SinkHelper(IServerCacheResultSink sink)
            {
                this.m_Sink = sink;
                if (this.m_Sink != null)
                {
                    this.m_Thread = Platform.Threading.ThreadPool.Run(CSSConfig.CommunicationServerCommonCacheThreadPriority, new WaitCallback(this.SaveThread));
                }
            }

            public void Flush()
            {
                if (this.m_Sink != null)
                {
                    while (this.m_Queue.Count > 0)
                    {
                        Thread.Sleep(5);
                    }
                }
            }

            public void GetServerResult(IUser user, Guid packetID, int threadID, out MemoryStream result)
            {
                Queue queue = null;
                lock (this.m_Queue.SyncRoot)
                {
                    queue = this.m_Queue.Clone() as Queue;
                }
                if (queue.Count > 0)
                {
                    foreach (object[] objArray in queue)
                    {
                        Guid guid = (Guid) objArray[1];
                        if (guid == packetID)
                        {
                            int num = (int) objArray[2];
                            if (num == threadID)
                            {
                                IUser user2 = (IUser) objArray[0];
                                if (user2.PlatformUserID == user.PlatformUserID)
                                {
                                    result = (MemoryStream) objArray[3];
                                    return;
                                }
                            }
                        }
                    }
                }
                lock (this.m_ReadSyncRoot)
                {
                    if (((this.m_CurrentData != null) && (((Guid) this.m_CurrentData[1]) == packetID)) && ((((int) this.m_CurrentData[2]) == threadID) && (((IUser) this.m_CurrentData[0]).PlatformUserID == user.PlatformUserID)))
                    {
                        result = (MemoryStream) this.m_CurrentData[3];
                    }
                    else if (this.m_Sink == null)
                    {
                        result = null;
                    }
                    else
                    {
                        try
                        {
                            this.m_Sink.GetResult(user, packetID, threadID, out result);
                        }
                        catch
                        {
                            result = null;
                        }
                    }
                }
            }

            public void SaveServerResult(IUser user, Guid packetID, int threadID, MemoryStream result)
            {
                lock (this.m_Queue.SyncRoot)
                {
                    this.m_Queue.Enqueue(new object[] { user, packetID, threadID, result });
                }
            }

            private void SaveThread(object state)
            {
                if (this.m_Sink != null)
                {
                    while (PlatformConfig.AppRuning)
                    {
                        lock (this.m_Queue.SyncRoot)
                        {
                            if (this.m_Queue.Count > 0)
                            {
                                this.m_CurrentData = this.m_Queue.Dequeue() as object[];
                            }
                        }
                        if (this.m_CurrentData == null)
                        {
                            Thread.Sleep(1);
                        }
                        else
                        {
                            lock (this.m_ReadSyncRoot)
                            {
                                try
                                {
                                    this.m_Sink.SaveResult((IUser) this.m_CurrentData[0], (Guid) this.m_CurrentData[1], (int) this.m_CurrentData[2], (MemoryStream) this.m_CurrentData[3]);
                                }
                                catch (Exception exception)
                                {
                                    lock (this.m_Queue.SyncRoot)
                                    {
                                        this.m_Queue.Enqueue(this.m_CurrentData);
                                    }
                                    TraceHelper.WriteLine("保存请求结果错误：" + exception.Message);
                                }
                                this.m_CurrentData = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
