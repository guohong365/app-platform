namespace Platform.Task
{
    using System;

    public interface IAutoTaskSink : ITaskSink
    {
        void Handle(object item);

        bool IsTaskFull { get; }

        int RemainTask { get; }

        int TotalTask { get; }
    }
}
