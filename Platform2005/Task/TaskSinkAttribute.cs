namespace Platform.Task
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class TaskSinkAttribute : Attribute
    {
        private string m_Category;

        public TaskSinkAttribute(string category)
        {
            this.m_Category = category;
        }

        public string Category
        {
            get
            {
                return this.m_Category;
            }
            set
            {
                this.m_Category = value;
            }
        }
    }
}
