namespace Platform.Task
{
    using System;

    public interface IWorkFlowTaskSink : ITaskSink
    {
        bool ChangeTaskState(object task, WorkFlowTsakState state);
    }
}
