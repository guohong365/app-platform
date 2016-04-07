namespace Platform.Message
{
    using System;

    public interface IMessageSaver
    {
        Platform.Message.Message[] GetMessages(string message, object filter, object state);
        void RemoveMessage(Platform.Message.Message message);
        void SaveMessage(Platform.Message.Message message);
    }
}
