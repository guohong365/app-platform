namespace Platform.Task.QueueTask
{
    using System;

    public interface IQueueTaskSink
    {
        void ProcessTask(params object[] infos);
    }
}
