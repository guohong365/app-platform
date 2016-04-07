namespace Platform.Task
{
    using System;
    using System.Collections;

    public sealed class TaskManager
    {
        private static Hashtable m_TaskSinks = new Hashtable();

        public static bool AddTask(string category, object item)
        {
            ArrayList list = m_TaskSinks[category] as ArrayList;
            if (list == null)
            {
                return false;
            }
            foreach (ITaskSink sink in list)
            {
                sink.AddTask(item);
            }
            return true;
        }

        public static ArrayList GetTasks(string category, object filter)
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = m_TaskSinks[category] as ArrayList;
            if (list2 != null)
            {
                foreach (ITaskSink sink in list2)
                {
                    ArrayList tasks = sink.GetTasks(filter);
                    list.AddRange(tasks);
                }
            }
            return list;
        }

        public static void RegisterTaskSink(ITaskSink sink)
        {
            if (sink != null)
            {
                TaskSinkAttribute[] customAttributes = sink.GetType().GetCustomAttributes(typeof(TaskSinkAttribute), false) as TaskSinkAttribute[];
                if (customAttributes.Length < 1)
                {
                   throw new Exception("无效任务处理器，无法找到任务处理器属性。");
                }
                foreach (TaskSinkAttribute attribute in customAttributes)
                {
                    ArrayList list = m_TaskSinks[attribute.Category] as ArrayList;
                    if (list == null)
                    {
                        list = new ArrayList();
                        m_TaskSinks[attribute.Category] = list;
                    }
                    if (!list.Contains(sink))
                    {
                        list.Add(sink);
                    }
                }
            }
        }

        public static bool RemoveTask(string category, object item)
        {
            ArrayList list = m_TaskSinks[category] as ArrayList;
            if (list == null)
            {
                return false;
            }
            foreach (ITaskSink sink in list)
            {
                sink.RemoveTask(item);
            }
            return true;
        }

        public static void RemoveTaskSink(ITaskSink sink)
        {
            if (sink != null)
            {
                TaskSinkAttribute[] customAttributes = sink.GetType().GetCustomAttributes(typeof(TaskSinkAttribute), false) as TaskSinkAttribute[];
                if (customAttributes.Length < 1)
                {
                    throw new Exception("无效任务处理器，无法找到任务处理器属性。");
                }
                foreach (TaskSinkAttribute attribute in customAttributes)
                {
                    ArrayList list = m_TaskSinks[attribute.Category] as ArrayList;
                    if (list != null)
                    {
                        list.Remove(sink);
                    }
                }
            }
        }

        public static bool UpdateTask(string category, object item)
        {
            ArrayList list = m_TaskSinks[category] as ArrayList;
            if (list == null)
            {
                return false;
            }
            foreach (ITaskSink sink in list)
            {
                sink.UpdateTask(item);
            }
            return true;
        }
    }
}
