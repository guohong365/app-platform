namespace Platform.Message
{
    using Platform.ExceptionHandling;
    using System;
    using System.Collections;

    public sealed class MessageControler
    {
        private static Hashtable m_MessageProcessorTable = new Hashtable();
        private static Hashtable m_MessageSaverTable = new Hashtable();

        public static Platform.Message.Message[] GetMessages(string message, object filterParameter, object state)
        {
            IMessageSaver saver = m_MessageSaverTable[message] as IMessageSaver;
            if (saver == null)
            {
                return null;
            }
            return saver.GetMessages(message, filterParameter, state);
        }

        public static void HandlePostedMessages(string message, object filterParameter, object state)
        {
            IMessageSaver saver = m_MessageSaverTable[message] as IMessageSaver;
            if (saver != null)
            {
                Platform.Message.Message[] messageArray = saver.GetMessages(message, filterParameter, state);
                if (messageArray != null)
                {
                    ArrayList list = m_MessageProcessorTable[message] as ArrayList;
                    foreach (Platform.Message.Message message2 in messageArray)
                    {
                        bool flag = false;
                        foreach (IMessageProcessor processor in list)
                        {
                            if (!processor.IsMessageMatch(message2, state))
                            {
                                continue;
                            }
                            try
                            {
                                flag |= processor.OnPostedMessage(message2, state);
                                continue;
                            }
                            catch (Exception exception)
                            {
                                ExceptionHelper.HandleException(exception);
                                continue;
                            }
                        }
                        if (flag)
                        {
                            saver.RemoveMessage(message2);
                        }
                    }
                }
            }
        }

        public static int PostMessage(Platform.Message.Message message)
        {
            IMessageSaver saver = m_MessageSaverTable[message.Msg] as IMessageSaver;
            if (saver != null)
            {
                try
                {
                    saver.SaveMessage(message);
                }
                catch (Exception exception)
                {
                    ExceptionHelper.HandleException(exception);
                }
            }
            return 0;
        }

        public static int PostMessage(string message)
        {
            return PostMessage(new Platform.Message.Message(message, null, null));
        }

        public static int PostMessage(string message, object wparam)
        {
            return PostMessage(new Platform.Message.Message(message, wparam, null));
        }

        public static int PostMessage(string message, object wparam, object lparam)
        {
            return PostMessage(new Platform.Message.Message(message, wparam, lparam));
        }

        public static void RegisterMessageProcessor(string message, IMessageProcessor processor)
        {
            lock (m_MessageProcessorTable.SyncRoot)
            {
                ArrayList list = m_MessageProcessorTable[message] as ArrayList;
                if (list == null)
                {
                    list = new ArrayList();
                    m_MessageProcessorTable[message] = list;
                }
                list.Add(processor);
            }
        }

        public static void RegisterMessageSaver(string message, IMessageSaver saver)
        {
            lock (m_MessageSaverTable.SyncRoot)
            {
                m_MessageSaverTable[message] = saver;
            }
        }

        public static int SendImmediateMessage(Platform.Message.Message message)
        {
            ArrayList list = m_MessageProcessorTable[message.Msg] as ArrayList;
            if (list == null)
            {
                return 0;
            }
            int num = 0;
            foreach (IMessageProcessor processor in list)
            {
                try
                {
                    num = processor.OnImmediateMessage(message);
                    continue;
                }
                catch (Exception exception)
                {
                    ExceptionHelper.HandleException(exception);
                    continue;
                }
            }
            return num;
        }

        public static int SendImmediateMessage(string message)
        {
            return SendImmediateMessage(new Platform.Message.Message(message, null, null));
        }

        public static int SendImmediateMessage(string message, object wparam)
        {
            return SendImmediateMessage(new Platform.Message.Message(message, wparam, null));
        }

        public static int SendImmediateMessage(string message, object wparam, object lparam)
        {
            return SendImmediateMessage(new Platform.Message.Message(message, wparam, lparam));
        }

        public static int SendMessage(Platform.Message.Message message)
        {
            SendImmediateMessage(message);
            PostMessage(message);
            return 0;
        }

        public static int SendMessage(string message)
        {
            return SendMessage(new Platform.Message.Message(message, null, null));
        }

        public static int SendMessage(string message, object wparam)
        {
            return SendMessage(new Platform.Message.Message(message, wparam, null));
        }

        public static int SendMessage(string message, object wparam, object lparam)
        {
            return SendMessage(new Platform.Message.Message(message, wparam, lparam));
        }
    }
}
