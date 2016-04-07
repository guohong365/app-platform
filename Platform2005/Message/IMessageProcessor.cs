namespace Platform.Message
{
    using System;

    public interface IMessageProcessor
    {
        bool IsMessageMatch(Platform.Message.Message message, object state);
        int OnImmediateMessage(Platform.Message.Message message);
        bool OnPostedMessage(Platform.Message.Message message, object state);
    }
}
