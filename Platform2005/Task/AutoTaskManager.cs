namespace Platform.Task
{
    using Platform.Threading;
    using System;
    using System.Collections;
    using System.Timers;

    public sealed class AutoTaskManager
    {
        private static Hashtable m_AutoTaskHandlerSink = new Hashtable();
        private static System.Timers.Timer m_Timer = new System.Timers.Timer(2000);

        static AutoTaskManager()
        {
            m_Timer.Elapsed += new ElapsedEventHandler(AutoTaskManager.Timer_Elapsed);
            m_Timer.Stop();
        }

        public static bool AddTask(AutoTaskItemBase item)
        {
            return TaskManager.AddTask("AutoTask", item);
        }

        public static void RegisterAutoTaskHandlerSink(IAutoTaskSink sink)
        {
            if (sink != null)
            {
                AutoTaskSinkAttribute[] customAttributes = sink.GetType().GetCustomAttributes(typeof(AutoTaskSinkAttribute), false) as AutoTaskSinkAttribute[];
                if (customAttributes.Length < 1)
                {
                    throw new Exception("无效任务处理器，无法找到任务处理器属性。");
                }
                foreach (AutoTaskSinkAttribute attribute in customAttributes)
                {
                    ArrayList list = null;
                    if (m_AutoTaskHandlerSink.ContainsKey(attribute.TaskType))
                    {
                        list = m_AutoTaskHandlerSink[attribute.TaskType] as ArrayList;
                    }
                    else
                    {
                        list = new ArrayList();
                        m_AutoTaskHandlerSink[attribute.TaskType] = list;
                    }
                    if (!list.Contains(sink))
                    {
                        list.Add(sink);
                    }
                }
                TaskManager.RegisterTaskSink(sink);
            }
        }

        public static void StartAutoTask()
        {
            m_Timer.Start();
        }

        public static void StopAutoTask()
        {
            m_Timer.Stop();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (AutoTaskItemBase base2 in TaskManager.GetTasks("AutoTask", DateTime.MinValue))
            {
                if ((base2.State == AutoTaskState.UnHandled) && m_AutoTaskHandlerSink.ContainsKey(base2.AutoTaskType))
                {
                    base2.State = AutoTaskState.Handling;
                    ArrayList list2 = m_AutoTaskHandlerSink[base2.AutoTaskType] as ArrayList;
                    foreach (IAutoTaskSink sink in list2)
                    {
                        if (!sink.IsTaskFull)
                        {
                            ThreadPool.Run(new System.Threading.WaitCallback(sink.Handle), base2);
                        }
                    }
                }
            }
        }
    }
}
