namespace Platform.CSS.Communication
{
    using Platform.IO;
    using Platform.Task.QueueTask;
    using System;

    [QueueTaskSink("ServerCommunicationClientEventSink")]
    public class ServerCommunicationClientEventSink : IQueueTaskSink
    {
        static ServerCommunicationClientEventSink()
        {
            ServerCommunicationClientEventSink sink = new ServerCommunicationClientEventSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        private ServerCommunicationClientEventSink()
        {
        }

        public static void OnServerEventReceived(MemoryStream ms)
        {
        }

        public void ProcessTask(params object[] infos)
        {
        }
    }
}
