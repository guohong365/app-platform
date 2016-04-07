namespace Platform.Log
{
    using Platform;
    using Platform.Identity;
    using Platform.Task.QueueTask;
    using Platform.Tracing;
    using Platform.Utils;
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Reflection;

    [QueueTaskSink("DBHandlerLog")]
    public sealed class DBHandlerLogSink : FileLogBase, IQueueTaskSink
    {
        private static Type m_IgnoreType = typeof(DBHandlerLogPassAttribute);

        static DBHandlerLogSink()
        {
            DBHandlerLogSink sink = new DBHandlerLogSink();
            QueueTaskManager.RegisterQueueTaskSink(sink);
        }

        private static string BuildUserMessage()
        {
            if (PlatformConfig.DBHandlerWriteLogLevel == Platform.Log.LogLevel.None)
            {
                return null;
            }
            if ((PlatformConfig.DBHandlerWriteLogLevel & Platform.Log.LogLevel.IncludeIgnoreInfo) != Platform.Log.LogLevel.IncludeIgnoreInfo)
            {
                StackTrace trace = new StackTrace(false);
                int frameCount = trace.FrameCount;
                for (int i = 0; i < frameCount; i++)
                {
                    MethodBase method = trace.GetFrame(i).GetMethod();
                    object[] customAttributes = method.GetCustomAttributes(m_IgnoreType, false);
                    object[] objArray2 = method.DeclaringType.GetCustomAttributes(m_IgnoreType, false);
                    if ((customAttributes.Length > 0) || (objArray2.Length > 0))
                    {
                        return null;
                    }
                }
            }
            IUser currentThreadUser = UserManager.GetCurrentThreadUser();
            if (((PlatformConfig.DBHandlerWriteLogLevel & Platform.Log.LogLevel.NoneUser) != Platform.Log.LogLevel.NoneUser) && (currentThreadUser == null))
            {
                return null;
            }
            if (((PlatformConfig.DBHandlerWriteLogLevel & Platform.Log.LogLevel.User) != Platform.Log.LogLevel.User) && (currentThreadUser != null))
            {
                return null;
            }
            return ("操作用户：" + ((currentThreadUser == null) ? "无用户信息" : currentThreadUser.ToString()) + "\r\n操作时间：" + PlatformDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public void ProcessTask(params object[] infos)
        {
            if (LogConfig.WriteDBHelperLog)
            {
                try
                {
                    if ((infos != null) && (infos.Length >= 1))
                    {
                        string msg = (string)infos[0];
                        base.WriteLogFileMessage(msg);
                    }
                }
                catch (Exception exception)
                {
                    TraceHelper.WriteLine("Write DBHandlerLog:" + exception.Message);
                }
            }
        }

        public static void WriteBeginTransactionExceptionLog(IsolationLevel level, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：BeginTransaction") + "\r\n事务级别：" + level) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteBeginTransactionLog(IsolationLevel level, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((text + "\r\n操作方法：BeginTransaction") + "\r\n事务级别：" + level) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteEndTransactionExceptionLog(bool commit, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：EndTransaction") + "\r\n事务操作：" + (commit ? "提交" : "回滚")) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteEndTransactionLog(bool commit, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((text + "\r\n操作方法：EndTransaction") + "\r\n事务操作：" + (commit ? "提交" : "回滚")) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteNonQueryExceptionLog(string strSql, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：ExecuteNonQuery") + "\r\n操作语句：" + strSql) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteNonQueryLog(string strSql, int result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：ExecuteNonQuery") + "\r\n操作语句：" + strSql) + "\r\n操作结果：" + result) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteProcExceptionLog(string procName, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：ExecuteProc") + "\r\n过程名称：" + procName) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteProcLog(string procName, object result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (text + "\r\n操作方法：ExecuteProc") + "\r\n过程名称：" + procName;
                    if (result == null)
                    {
                        text = text + "\r\n操作结果：null";
                    }
                    else
                    {
                        text = text + "\r\n操作结果：" + result.ToString();
                    }
                    text = (text + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteReaderExceptionLog(string strSql, CommandBehavior behavior, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((((text + "\r\n操作方法：ExecuteReader") + "\r\n操作语句：" + strSql) + "\r\n操作级别：" + behavior) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteReaderLog(string strSql, CommandBehavior behavior, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：ExecuteReader") + "\r\n操作语句：" + strSql) + "\r\n操作级别：" + behavior) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteScalarExceptionLog(string strSql, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：ExecuteScalar") + "\r\n操作语句：" + strSql) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteExecuteScalarLog(string strSql, object result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (text + "\r\n操作方法：ExecuteScalar") + "\r\n操作语句：" + strSql;
                    if (result == null)
                    {
                        text = text + "\r\n操作结果：null";
                    }
                    else
                    {
                        text = text + "\r\n操作结果：" + result.ToString();
                    }
                    text = (text + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteFillDataSetSchemaExceptionLog(string[] tableNames, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = text + "\r\n操作方法：FillDataSetSchema" + "\r\n数据表名：";
                    string[] textArray = tableNames;
                    for (int i = 0; i < textArray.Length; i++)
                    {
                        if (textArray[i] == null)
                        {
                            text = text + "(null),";
                        }
                    }
                    text.TrimEnd(new char[] { ',' });
                    text = ((text + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteFillExceptionLog(string strSql, Type fillType, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((((text + "\r\n操作方法：Fill") + "\r\n操作语句：" + strSql) + "\r\n填充类型：" + fillType) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteFillExceptionLog(string strSql, Type fillType, string tableName, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((((text + "\r\n操作方法：Fill") + "\r\n操作语句：" + strSql) + "\r\n填充类型：" + fillType) + "\r\n填充表名：" + tableName) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteFillLog(string strSql, Type fillType, int result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((((text + "\r\n操作方法：Fill") + "\r\n操作语句：" + strSql) + "\r\n填充类型：" + fillType) + "\r\n填充结果：" + result) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteFillLog(string strSql, Type fillType, string tableName, int result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((((text + "\r\n操作方法：Fill") + "\r\n操作语句：" + strSql) + "\r\n填充类型：" + fillType) + "\r\n填充表名：" + tableName) + "\r\n填充结果：" + result) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteGetDataSetExceptionLog(string tableName, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = ((((text + "\r\n操作方法：GetDataSet") + "\r\n数据表名：" + tableName) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteUpdateExceptionLog(Type fillType, string tableName, Exception exp, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((((text + "\r\n操作方法：Update") + "\r\n更新类型：" + fillType) + "\r\n更新表名：" + tableName) + "\r\n异常信息：" + exp.Message) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        public static void WriteUpdateLog(Type fillType, string tableName, int result, TimeSpan ts)
        {
            try
            {
                string text = BuildUserMessage();
                if (text != null)
                {
                    text = (((((text + "\r\n操作方法：Update") + "\r\n更新类型：" + fillType) + "\r\n更新表名：" + tableName) + "\r\n更新结果：" + result) + "\r\n执行时间：" + ts.TotalSeconds) + "\r\n";
                    QueueTaskManager.EnqueueTask("DBHandlerLog", new object[] { text });
                }
            }
            catch
            {
            }
        }

        protected override string LogFileNameBase
        {
            get
            {
                return "DBHandler";
            }
        }

        public static int TaskCount
        {
            get
            {
                return QueueTaskManager.GetTaskCount("DBHandlerLog");
            }
        }
    }
}
