namespace Platform.Message
{
    using System;

    [Serializable]
    public class Message
    {
        private object m_Lparam;
        private string m_Msg;
        private object m_Wparam;

        public Message(string message, object wparam, object lparam)
        {
            this.m_Msg = message;
            this.m_Wparam = wparam;
            this.m_Lparam = lparam;
        }

        public object Lparam
        {
            get
            {
                return this.m_Lparam;
            }
        }

        public string Msg
        {
            get
            {
                return this.m_Msg;
            }
        }

        public object Wparam
        {
            get
            {
                return this.m_Wparam;
            }
        }
    }
}
