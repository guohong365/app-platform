namespace Platform.Task
{
    using System;
    using System.Collections;

    public interface ITaskSink
    {
        bool AddTask(object item);
        ArrayList GetTasks(object filter);
        bool RemoveTask(object item);
        bool UpdateTask(object item);
    }
}
