namespace Platform.Threading
{
    using Platform;
    using Platform.Caching;
    using Platform.ExceptionHandling;
    using Platform.Identity;
    using Platform.Utils;
    using System;
    using System.Collections;
    using System.Threading;

    public sealed class ThreadPool : IBufferItem
    {
        private DateTime m_ActiveTime = PlatformDateTime.Now;
        private static int m_Count = 0;
        private ManualResetEvent m_ExitEvent = new ManualResetEvent(false);
        private bool m_Finished = true;
        private ThreadPriority m_Priority = ThreadPriority.Normal;
        private bool m_Runing = true;
        private ManualResetEvent m_RuningEvent = new ManualResetEvent(false);
        private static BufferStorage m_Storage = new BufferStorage(typeof(Platform.Threading.ThreadPool));
        private Thread m_Thread;
        private static ArrayList m_ThreadExitEvents = new ArrayList();
        private static Hashtable m_Threads = new Hashtable();
        private WaitCallback m_UserFinallyHandler;
        private WaitCallback m_UserHandler;
        private Guid m_UserID = Guid.Empty;
        private object m_UserState;
        private ManualResetEvent m_Wait;

        static ThreadPool()
        {
            m_Storage.BeforeFinallyRelease += new BufferStorageBeforeFinalReleaseEventHandler(Platform.Threading.ThreadPool.OnBeforeFinallyRelease);
            m_Storage.AfterFinallyRelease += new BufferStorageAfterFinalReleaseEventHandler(Platform.Threading.ThreadPool.OnAfterFinallyRelease);
            m_Storage.ItemCreated += new BufferStorageCreateItemEventHandler(Platform.Threading.ThreadPool.OnItemCreated);
        }

        public ThreadPool()
        {
            this.Start();
        }

        private void Close()
        {
            this.m_Runing = false;
            if ((this.m_Thread != null) && ((this.m_Thread.ThreadState | ThreadState.Stopped) != ThreadState.Stopped))
            {
                try
                {
                    this.m_RuningEvent.Set();
                }
                catch
                {
                }
                Thread.Sleep(1);
                if ((this.m_Thread.ThreadState | ThreadState.Stopped) != ThreadState.Stopped)
                {
                    try
                    {
                        this.m_Thread.Abort();
                    }
                    catch
                    {
                    }
                    if ((this.m_Thread.ThreadState | ThreadState.Stopped) != ThreadState.Stopped)
                    {
                        if ((this.m_Thread.ThreadState | ThreadState.Unstarted) == ThreadState.Unstarted)
                        {
                            try
                            {
                                this.m_Thread.Start();
                            }
                            catch
                            {
                            }
                        }
                        if ((this.m_Thread.ThreadState | ThreadState.Stopped) != ThreadState.Stopped)
                        {
                            Thread.Sleep(1);
                            try
                            {
                                this.m_RuningEvent.Set();
                            }
                            catch
                            {
                            }
                            if ((this.m_Thread.ThreadState | ThreadState.Unstarted) == ThreadState.Unstarted)
                            {
                                try
                                {
                                    this.m_Thread.Join();
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            RemoveCurrentPoolMap(this.m_Thread);
            this.m_UserState = null;
            this.m_UserHandler = null;
            this.m_Wait = null;
            this.m_UserID = Guid.Empty;
            this.m_Thread = null;
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler, object stateObject)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler, object stateObject)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            return Run(UserManager.GetCurrentThreadUserID(), ThreadPriority.Normal, handler, finallyHandler, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool CurrentThreadUserRun(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            return Run(UserManager.GetCurrentThreadUserID(), priority, handler, finallyHandler, stateObject, wait);
        }

        public void FinalRelease()
        {
            this.Close();
        }

        public void FreeBuffer()
        {
            this.m_Finished = true;
            this.m_UserState = null;
            this.m_Wait = null;
            this.m_UserHandler = null;
            this.m_UserID = Guid.Empty;
            m_Storage.FreeBuffer(this);
        }

        public static Platform.Threading.ThreadPool GetCurrentPool()
        {
            return (m_Threads[Thread.CurrentThread] as Platform.Threading.ThreadPool);
        }

        private void InternalRun()
        {
            if (this.WaitForRun())
            {
                while (this.m_Runing && PlatformConfig.AppRuning)
                {
                    if (this.m_UserHandler != null)
                    {
                        try
                        {
                            Thread.CurrentThread.Priority = this.m_Priority;
                            UserManager.SetCurrentThreadUserID(this.m_UserID);
                            if (this.m_Wait != null)
                            {
                                this.m_Wait.Reset();
                            }
                            UpdateActiveTime();
                            this.m_UserHandler(this.m_UserState);
                        }
                        catch (Exception exception)
                        {
                            ExceptionHelper.HandleException(exception);
                        }
                        try
                        {
                            if (this.m_UserFinallyHandler != null)
                            {
                                this.m_UserFinallyHandler(this.m_UserState);
                            }
                            Thread.CurrentThread.Priority = ThreadPriority.Normal;
                        }
                        catch (Exception exception2)
                        {
                            ExceptionHelper.HandleException(exception2);
                        }
                        finally
                        {
                            if (this.m_Wait != null)
                            {
                                this.m_Wait.Set();
                            }
                        }
                    }
                    this.FreeBuffer();
                    if (!this.WaitForRun())
                    {
                        break;
                    }
                }
            }
            else
            {
                return;
            }
            this.m_ExitEvent.Set();
        }

        private void InternalRun(Guid userID, ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            this.m_Finished = false;
            this.m_UserState = stateObject;
            this.m_Wait = wait;
            this.m_Priority = priority;
            this.m_UserHandler = handler;
            this.m_UserFinallyHandler = finallyHandler;
            this.m_UserID = userID;
            this.m_RuningEvent.Set();
        }

        private static void OnAfterFinallyRelease()
        {
        }

        private static void OnBeforeFinallyRelease(ArrayList allItems)
        {
            if (m_ThreadExitEvents.Count >= 1)
            {
                m_ThreadExitEvents.ToArray(typeof(ManualResetEvent));
                foreach (Platform.Threading.ThreadPool pool in allItems)
                {
                    pool.m_Runing = false;
                    pool.m_RuningEvent.Set();
                }
            }
        }

        private static void OnItemCreated(IBufferItem item)
        {
            m_ThreadExitEvents.Add(((Platform.Threading.ThreadPool) item).m_ExitEvent);
        }

        public static void PreStoreBuffer(int count)
        {
            m_Storage.PreStoreItem(count);
        }

        private static void RemoveCurrentPoolMap(Thread thread)
        {
            if (thread != null)
            {
                lock (m_Threads.SyncRoot)
                {
                    m_Threads.Remove(thread);
                }
            }
        }

        public void Reset()
        {
            this.Close();
            this.Start();
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler)
        {
            return Run(userID, ThreadPriority.Normal, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler)
        {
            return Run(Guid.Empty, priority, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler, object stateObject)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler)
        {
            return Run(userID, priority, handler, null, null, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler, object stateObject)
        {
            return Run(userID, ThreadPriority.Normal, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(userID, ThreadPriority.Normal, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler, object stateObject)
        {
            return Run(Guid.Empty, priority, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(Guid.Empty, priority, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler, object stateObject)
        {
            return Run(userID, priority, handler, null, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler)
        {
            return Run(userID, priority, handler, finallyHandler, null, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(userID, ThreadPriority.Normal, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(userID, ThreadPriority.Normal, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(Guid.Empty, priority, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(Guid.Empty, priority, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            return Run(Guid.Empty, ThreadPriority.Normal, handler, finallyHandler, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler, object stateObject, ManualResetEvent wait)
        {
            return Run(userID, priority, handler, null, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject)
        {
            return Run(userID, priority, handler, finallyHandler, stateObject, null);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            return Run(userID, ThreadPriority.Normal, handler, finallyHandler, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            return Run(Guid.Empty, priority, handler, finallyHandler, stateObject, wait);
        }

        public static Platform.Threading.ThreadPool Run(Guid userID, ThreadPriority priority, WaitCallback handler, WaitCallback finallyHandler, object stateObject, ManualResetEvent wait)
        {
            if (handler == null)
            {
                return null;
            }
            if (m_Storage.FinalReleased)
            {
                return null;
            }
            Platform.Threading.ThreadPool buffer = m_Storage.GetBuffer() as Platform.Threading.ThreadPool;
            buffer.InternalRun(userID, priority, handler, finallyHandler, stateObject, wait);
            return buffer;
        }

        public static void SetSystemThreadPool()
        {
            uint maxWorkerThreads = (ThreadingConfig.ThreadPoolLimitation > 0) ? ((uint) ThreadingConfig.ThreadPoolLimitation) : 0x400;
            ThreadPoolManager.SetThreads(maxWorkerThreads, maxWorkerThreads * 100, maxWorkerThreads, maxWorkerThreads * 100);
        }

        private void Start()
        {
            this.m_Runing = true;
            this.m_RuningEvent.Reset();
            lock (m_Threads.SyncRoot)
            {
                m_Count++;
                this.m_Thread = new Thread(new ThreadStart(this.InternalRun));
                this.m_Thread.Name = "ThreadPool " + m_Count;
                m_Threads[this.m_Thread] = this;
            }
            this.m_ActiveTime = PlatformDateTime.Now;
            this.m_Thread.Start();
        }

        public static void UpdateActiveTime()
        {
            Platform.Threading.ThreadPool pool = m_Threads[Thread.CurrentThread] as Platform.Threading.ThreadPool;
            if (pool != null)
            {
                pool.m_ActiveTime = PlatformDateTime.Now;
            }
        }

        private bool WaitForRun()
        {
            while (this.m_Runing)
            {
                if (this.m_RuningEvent.WaitOne(5, false))
                {
                    this.m_RuningEvent.Reset();
                    return true;
                }
            }
            return false;
        }

        public DateTime ActiveTime
        {
            get
            {
                return this.m_ActiveTime;
            }
        }

        public bool Finished
        {
            get
            {
                return this.m_Finished;
            }
        }

        public string ThreadName
        {
            get
            {
                if (this.m_Thread != null)
                {
                    return this.m_Thread.Name;
                }
                return "";
            }
        }

        public static Hashtable Threads
        {
            get
            {
                return m_Threads;
            }
        }
    }
}
