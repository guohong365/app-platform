namespace Platform.CSS.Communication
{
    using Platform.IO;
    using Platform.Task.QueueTask;
    using System;

    [QueueTaskSink("ClientCommunicationServerEventSink")]
    public class ClientCommunicationServerEventSink : IQueueTaskSink
    {
        static ClientCommunicationServerEventSink()
        {
            ClientCommunicationServerEventSink sink = new ClientCommunicationServerEventSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        private ClientCommunicationServerEventSink()
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
