namespace Platform.Task
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class AutoTaskSinkAttribute : TaskSinkAttribute
    {
        private string m_TaskType;

        public AutoTaskSinkAttribute(string taskType) : base("AutoTask")
        {
            this.m_TaskType = taskType;
        }

        public string TaskType
        {
            get
            {
                return this.m_TaskType;
            }
            set
            {
                this.m_TaskType = value;
            }
        }
    }
}
