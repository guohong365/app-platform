namespace Platform.Task
{
    using System;

    public sealed class WorkFlowTaskManager
    {
        private static IWorkFlowTaskSink m_Sink;

        public static bool SetWorkFlowTaskSink(IWorkFlowTaskSink sink)
        {
            if (sink == null)
            {
                return false;
            }
            if (m_Sink != null)
            {
                TaskManager.RegisterTaskSink(sink);
            }
            m_Sink = sink;
            return true;
        }
    }
}
