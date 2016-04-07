namespace Platform.Threading
{
    using System;
    using System.Runtime.InteropServices;

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("84680D3A-B2C1-46e8-ACC2-DBC0A359159A")]
    internal interface ICorThreadpool
    {
        void RegisterWaitForSingleObject();
        void UnregisterWait();
        void QueueUserWorkItem();
        void CreateTimer();
        void ChangeTimer();
        void DeleteTimer();
        void BindIoCompletionCallback();
        void CallOrQueueUserWorkItem();
        void SetMaxThreads(uint MaxWorkerThreads, uint MaxIOCompletionThreads);
        void GetMaxThreads(out uint MaxWorkerThreads, out uint MaxIOCompletionThreads);
        void GetAvailableThreads(out uint AvailableWorkerThreads, out uint AvailableIOCompletionThreads);
    }
}
