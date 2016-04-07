namespace Platform.Session
{
    using System;
    using System.Collections;

    public interface ISessionChannel
    {
        object GetSession(int type, params object[] infos);
        ArrayList LoadSessions(int type);
        bool RemoveSession(int type, params object[] infos);
        bool SaveSession(int type, params object[] infos);
        bool UpdateSession(int type, params object[] infos);
    }
}
