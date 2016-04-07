namespace Platform.Task
{
    using System;
    using Platform.Utils;

    public abstract class AutoTaskItemBase
    {
        protected string m_AutoTaskType;
        protected DateTime m_CreateTime = PlatformDateTime.Now;
        protected DateTime m_ExpireTime = DateTime.MaxValue;
        protected AutoTaskPriority m_Priority = AutoTaskPriority.Normal;
        protected AutoTaskState m_State = AutoTaskState.UnHandled;
        protected Guid m_TaskID = Guid.Empty;
        protected DateTime m_UpdateTime = PlatformDateTime.Now;

        protected AutoTaskItemBase()
        {
        }

        public string AutoTaskType
        {
            get
            {
                return this.m_AutoTaskType;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return this.m_CreateTime;
            }
            set
            {
                this.m_CreateTime = value;
            }
        }

        public DateTime ExpireTime
        {
            get
            {
                return this.m_ExpireTime;
            }
            set
            {
                this.m_ExpireTime = value;
            }
        }

        public AutoTaskPriority Priority
        {
            get
            {
                return this.m_Priority;
            }
            set
            {
                this.m_Priority = value;
            }
        }

        public AutoTaskState State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
            }
        }

        public Guid TaskID
        {
            get
            {
                return this.m_TaskID;
            }
            set
            {
                this.m_TaskID = value;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return this.m_UpdateTime;
            }
            set
            {
                this.m_UpdateTime = value;
            }
        }
    }
}
