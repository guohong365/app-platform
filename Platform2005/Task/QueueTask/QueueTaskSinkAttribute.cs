namespace Platform.Task.QueueTask
{
    using System;
    using System.Threading;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class QueueTaskSinkAttribute : Attribute
    {
        private string m_Category;
        private ThreadPriority m_TaskPriority;

        public QueueTaskSinkAttribute(string category)
        {
            this.m_Category = category;
            this.m_TaskPriority = ThreadPriority.Normal;
        }

        public QueueTaskSinkAttribute(string category, ThreadPriority priority)
        {
            this.m_Category = category;
            this.m_TaskPriority = priority;
        }

        public string Category
        {
            get
            {
                return this.m_Category;
            }
        }

        public ThreadPriority TaskPriority
        {
            get
            {
                return this.m_TaskPriority;
            }
        }
    }
}
