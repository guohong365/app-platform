namespace Platform.Identity
{
    using Platform.Session;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class UserManager
    {
        private static Type m_DefaultType = null;
        private static readonly LocalDataStoreSlot m_UserIDSlot = Thread.GetNamedDataSlot("PlatformUserID");
        private static Hashtable m_Users = new Hashtable();
        private static string m_UserSessionChannelName = null;
        private static PlatformUserBanChangedHandler OnBanUser = null;
        private static PlatformLoadUserHandler OnLoadUser = null;
        private static PlatformModifyUserHandler OnModifyUser = null;
        private static PlatformRemoveUserHandler OnRemoveUser = null;
        private static PlatformUserActiveTimeChangedHandler OnUserActiveTimeChanged = null;

        public static void AddUser(IUser user)
        {
            lock (m_Users.SyncRoot)
            {
                m_Users[user.PlatformUserID] = user;
            }
        }

        public static void CleanCurrentThreadUser()
        {
            Thread.SetData(m_UserIDSlot, null);
        }

        public static IUser CreateUserInstance()
        {
            if (m_DefaultType == null)
            {
                return null;
            }
            return (Activator.CreateInstance(m_DefaultType) as IUser);
        }

        public static void CurrentUserActiveTimeChanged()
        {
            try
            {
                IUser currentThreadUser = GetCurrentThreadUser();
                if (currentThreadUser != null)
                {
                    currentThreadUser.UpdateActiveTime();
                    OnUserActiveTimeChanged(currentThreadUser);
                }
            }
            catch
            {
            }
        }

        public static bool GetBan(Guid userid, out string banMessage)
        {
            banMessage = null;
            IUser user = GetUser(userid);
            if (user == null)
            {
                return false;
            }
            return user.GetBan(out banMessage);
        }

        public static IUser GetCurrentThreadUser()
        {
            Guid currentThreadUserID = GetCurrentThreadUserID();
            if (currentThreadUserID == Guid.Empty)
            {
                return null;
            }
            return GetUser(currentThreadUserID);
        }

        public static Guid GetCurrentThreadUserID()
        {
            object data = Thread.GetData(m_UserIDSlot);
            if (data == null)
            {
                return Guid.Empty;
            }
            if (data.GetType() != typeof(Guid))
            {
                return Guid.Empty;
            }
            return (Guid) data;
        }

        public static IUser GetUser(Guid userid)
        {
            IUser user;
            lock (m_Users.SyncRoot)
            {
                user = m_Users[userid] as IUser;
            }
            if (user == null)
            {
                user = LoadSessionUser(userid);
            }
            return user;
        }

        public static IUser GetUserNotFromSession(Guid userid)
        {
            lock (m_Users.SyncRoot)
            {
                return (m_Users[userid] as IUser);
            }
        }

        public static IUser LoadSessionUser(Guid userID)
        {
            IUser user = SessionManager.GetSession(m_UserSessionChannelName, 0, new object[] { userID }) as IUser;
            if (user == null)
            {
                return null;
            }
            AddUser(user);
            return user;
        }

        public static IUser LoadSessionUser(int type, params object[] infos)
        {
            IUser user = SessionManager.GetSession(m_UserSessionChannelName, type, infos) as IUser;
            if (user == null)
            {
                return null;
            }
            AddUser(user);
            return user;
        }

        public static IUser LoadUser(byte[] userIdentities, out byte[] userData, out int error, out string platformMsg, out string[] msg)
        {
            error = 0;
            msg = new string[0];
            platformMsg = "";
            if (OnLoadUser == null)
            {
                userData = new byte[0];
                error = -1;
               platformMsg = "无法找到获取用户处理方法！";
                return null;
            }
            IUser user = OnLoadUser(userIdentities, out userData, out error, out msg);
            if (user == null)
            {
               platformMsg = "无法获得用户！";
                return null;
            }
            return user;
        }

        public static bool ModifyUser(IUser user, byte[] modifyData, out byte[] modifyRcData)
        {
            modifyRcData = null;
            if (OnModifyUser == null)
            {
                return false;
            }
            return OnModifyUser(user, modifyData, out modifyRcData);
        }

        public static void RegisterUserHandlers(PlatformRemoveUserHandler removeHandler, PlatformLoadUserHandler loadHandler, PlatformModifyUserHandler modifyHandler, PlatformUserActiveTimeChangedHandler userActiveTimeChanged, PlatformUserBanChangedHandler userBanChanged)
        {
            OnRemoveUser = removeHandler;
            OnLoadUser = loadHandler;
            OnModifyUser = modifyHandler;
            OnUserActiveTimeChanged = userActiveTimeChanged;
            OnBanUser = userBanChanged;
        }

        public static void RegisterUserSessionChannelName(string channelName)
        {
            m_UserSessionChannelName = channelName;
        }

        public static void RegisterUserType(Type type)
        {
            if (type.GetInterface("Platform.Identity.IUser") != null)
            {
                m_DefaultType = type;
            }
        }

        public static IUser RemoveCurrentThreadUser()
        {
            IUser currentThreadUser = GetCurrentThreadUser();
            if (currentThreadUser == null)
            {
                return null;
            }
            RemoveUser(currentThreadUser);
            return currentThreadUser;
        }

        public static bool RemoveSessionUser(IUser user)
        {
            SessionManager.RemoveSession(m_UserSessionChannelName, 0, new object[] { user.PlatformUserID });
            RemoveUser(user);
            return true;
        }

        public static void RemoveUser(IUser user)
        {
            lock (m_Users.SyncRoot)
            {
                m_Users.Remove(user.PlatformUserID);
            }
        }

        public static void RemoveUser(Guid userid)
        {
            lock (m_Users.SyncRoot)
            {
                m_Users.Remove(userid);
            }
        }

        public static bool RemoveUser(IUser user, byte[] userIdentities, out byte[] userData, out int error, out string platformMsg, out string[] msg)
        {
            error = 0;
            msg = new string[0];
            platformMsg = "";
            if (OnRemoveUser == null)
            {
                platformMsg = "无法找到获取用户处理方法！";
                error = -1;
                userData = new byte[0];
                return true;
            }
            return OnRemoveUser(user, userIdentities, out userData, out error, out msg);
        }

        public static void SetBan(Guid userid, bool ban, string banMessage)
        {
            IUser user = GetUser(userid);
            if (user != null)
            {
                user.SetBan(ban, banMessage);
                if (OnBanUser != null)
                {
                    OnBanUser(user, ban, banMessage);
                }
            }
        }

        public static void SetCurrentThreadUser(IUser user)
        {
            Thread.SetData(m_UserIDSlot, user.PlatformUserID);
        }

        public static void SetCurrentThreadUserID(Guid userID)
        {
            Thread.SetData(m_UserIDSlot, userID);
        }

        public static void UserActiveTimeChanged(IUser user)
        {
            if (user != null)
            {
                user.UpdateActiveTime();
                if (OnUserActiveTimeChanged != null)
                {
                    try
                    {
                        OnUserActiveTimeChanged(user);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void UserBanChanged(IUser user, bool ban, string message)
        {
            if (user != null)
            {
                user.SetBan(ban, message);
                if (OnBanUser != null)
                {
                    try
                    {
                        OnBanUser(user, ban, message);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static Hashtable Users
        {
            get
            {
                return m_Users;
            }
        }
    }
}
