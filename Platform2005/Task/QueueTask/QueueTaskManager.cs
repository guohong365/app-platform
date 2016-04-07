namespace Platform.Task.QueueTask
{
    using System;
    using System.Collections;
    using System.Threading;

    public sealed class QueueTaskManager
    {
        private static Hashtable m_QueueTaskSinkHelpers = new Hashtable();

        public static void EnqueueTask(string category, params object[] infos)
        {
            QueueTaskSinkHelper helper = m_QueueTaskSinkHelpers[category] as QueueTaskSinkHelper;
            if (helper != null)
            {
                helper.EnqueueTask(infos);
            }
        }

        public static void Flush()
        {
            foreach (QueueTaskSinkHelper helper in m_QueueTaskSinkHelpers.Values)
            {
                helper.Flush();
            }
        }

        public static int GetTaskCount(string category)
        {
            QueueTaskSinkHelper helper = m_QueueTaskSinkHelpers[category] as QueueTaskSinkHelper;
            if (helper == null)
            {
                return 0;
            }
            return helper.TaskCount;
        }

        public static bool RegisterQueueTaskSink(IQueueTaskSink sink)
        {
            if (sink == null)
            {
                return false;
            }
            QueueTaskSinkAttribute[] customAttributes = sink.GetType().GetCustomAttributes(typeof(QueueTaskSinkAttribute), false) as QueueTaskSinkAttribute[];
            if (customAttributes.Length < 1)
            {
                return false;
            }
            return RegisterQueueTaskSink(sink, customAttributes[0].Category, customAttributes[0].TaskPriority);
        }

        public static bool RegisterQueueTaskSink(Type sinkType)
        {
            if (sinkType == null)
            {
                return false;
            }
            if (sinkType.GetInterface("Platform.Task.QueueTask.IQueueTaskSink", false) == null)
            {
                return false;
            }
            IQueueTaskSink sink = Activator.CreateInstance(sinkType) as IQueueTaskSink;
            return RegisterQueueTaskSink(sink);
        }

        public static bool RegisterQueueTaskSink(IQueueTaskSink sink, string category, ThreadPriority taskPriority)
        {
            if (sink == null)
            {
                return false;
            }
            QueueTaskSinkHelper helper = m_QueueTaskSinkHelpers[category] as QueueTaskSinkHelper;
            if (helper == null)
            {
                helper = new QueueTaskSinkHelper();
                m_QueueTaskSinkHelpers[category] = helper;
            }
            helper.AddSink(sink, taskPriority);
            return true;
        }

        public static bool RegisterQueueTaskSink(Type sinkType, string category, ThreadPriority taskPriority)
        {
            if (sinkType == null)
            {
                return false;
            }
            if (sinkType.GetInterface("Platform.Task.QueueTask.IQueueTaskSink", false) == null)
            {
                return false;
            }
            IQueueTaskSink sink = Activator.CreateInstance(sinkType) as IQueueTaskSink;
            return RegisterQueueTaskSink(sink, category, taskPriority);
        }

        public static bool RemoveLogSink(string category)
        {
            m_QueueTaskSinkHelpers.Remove(category);
            return true;
        }

        public static bool RemoveQueueTaskSink(Type type)
        {
            QueueTaskSinkAttribute[] customAttributes = type.GetCustomAttributes(typeof(QueueTaskSinkAttribute), false) as QueueTaskSinkAttribute[];
            if (customAttributes.Length < 1)
            {
                return false;
            }
            RemoveQueueTaskSink(type, customAttributes[0].Category);
            return true;
        }

        public static bool RemoveQueueTaskSink(Type type, string category)
        {
            QueueTaskSinkHelper helper = m_QueueTaskSinkHelpers[category] as QueueTaskSinkHelper;
            if (helper != null)
            {
                helper.RemoveSink(type);
            }
            return true;
        }
    }
}
