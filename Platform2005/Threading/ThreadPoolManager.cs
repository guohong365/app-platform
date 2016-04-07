namespace Platform.Threading
{
    using System;
    using System.Threading;

    public sealed class ThreadPoolManager
    {
        private static ICorThreadpool m_Helper = ((ICorThreadpool) new ThreadPoolHelper());

        public static void SetMaxThreads(uint MaxWorkerThreads, uint MaxIOCompletionThreads)
        {
            m_Helper.SetMaxThreads(MaxWorkerThreads, MaxIOCompletionThreads);
        }

        public static void SetMinThreads(uint MinWorkerThreads, uint MinIOCompletionThreads)
        {
            System.Threading.ThreadPool.SetMinThreads((int) MinWorkerThreads, (int) MinIOCompletionThreads);
        }

        public static void SetThreads(uint MaxWorkerThreads, uint MaxIOCompletionThreads, uint MinWorkerThreads, uint MinIOCompletionThreads)
        {
            m_Helper.SetMaxThreads(MaxWorkerThreads, MaxIOCompletionThreads);
            MinWorkerThreads = (MinWorkerThreads > MaxWorkerThreads) ? MaxWorkerThreads : MinWorkerThreads;
            MinIOCompletionThreads = (MinIOCompletionThreads > MaxIOCompletionThreads) ? MaxIOCompletionThreads : MinIOCompletionThreads;
            System.Threading.ThreadPool.SetMinThreads((int) MinWorkerThreads, (int) MinIOCompletionThreads);
        }
    }
}
