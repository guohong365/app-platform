using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using Platform.Configuration;
using Platform.ExceptionHandling;
using Platform.Log;
using Platform.Utils;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
namespace Platform.DBHelper
{
    public sealed class DBHandler
    {
        private static readonly Type _dataSetType = typeof(DataSet);
        private static readonly Type _dataTableType = typeof(DataTable);

        [ConfigCollectionItem("/DBHanderSqlReplaceTable")]
        private static readonly Hashtable _dbHanderSqlReplaceTable = new Hashtable();
        [ConfigItem("/PlatformSettings", "EnableDBHandlerReplaceSql", null)]
        private static bool EnableDBHandlerReplaceSql = true;
        private IDbDataAdapter m_Adapter;
        private Hashtable m_Adapters = new Hashtable();
        private IDbCommand m_Command;
        private IDbConnection m_Connection;
        private Type m_CurrentType;
        private static Hashtable m_DataSetTypeTable = new Hashtable();
        private static Hashtable m_DBHandlerTypeDataSets = new Hashtable();
        private object m_DBHandlerTypeKey;
        private static string m_DefaultConnectionString;
        private static Type m_DefaultType;

        
        private static readonly LocalDataStoreSlot m_ExceptionSlot = Thread.GetNamedDataSlot("PlatformDBHandlerException");
        private bool m_IsConnectionType;
        private object m_Object;
        private Hashtable m_ProcParaeters = new Hashtable();
        private IDbTransaction m_Transaction;
        private bool m_TransactionOpenConnection;
        private static readonly Hashtable m_UnusedDBHandlers = new Hashtable();

        private DBHandler()
        {
        }

        public bool BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public bool BeginTransaction(IsolationLevel level)
        {
            DateTime now = DateTime.Now;
            try
            {
                if (m_Connection.State != ConnectionState.Open)
                {
                    m_Connection.Open();
                    m_TransactionOpenConnection = true;
                }
                else
                {
                    m_TransactionOpenConnection = false;
                }
                m_Transaction = m_Connection.BeginTransaction(level);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteBeginTransactionLog(level, DateTime.Now - now);
                }
                return true;
            }
            catch (Exception exception)
            {
                m_Transaction = null;
                if (m_TransactionOpenConnection)
                {
                    m_Connection.Close();
                }
                m_TransactionOpenConnection = false;
                HandleException(exception, level.ToString());
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteBeginTransactionExceptionLog(level, exception, DateTime.Now - now);
                }
                return false;
            }
        }

        public static void CleanException()
        {
            Thread.SetData(m_ExceptionSlot, null);
        }

        public void CloseConnection()
        {
            m_Connection.Close();
        }

        public IDbDataParameter CreateParameter()
        {
            return DBBuilder.CreateParameter(m_Connection);
        }

        public IDbDataParameter CreateParameter(string name)
        {
            IDbDataParameter parameter = DBBuilder.CreateParameter(m_Connection);
            parameter.ParameterName = name;
            return parameter;
        }

        public IDbDataParameter CreateParameter(string name, object value)
        {
            IDbDataParameter parameter = DBBuilder.CreateParameter(m_Connection);
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        public bool EndTransaction(bool commit)
        {
            bool flag;
            if (m_Transaction == null)
            {
                return false;
            }
            DateTime now = DateTime.Now;
            try
            {
                if (commit)
                {
                    m_Transaction.Commit();
                }
                else
                {
                    m_Transaction.Rollback();
                }
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteEndTransactionLog(commit, DateTime.Now - now);
                }
                flag = true;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteEndTransactionExceptionLog(commit, exception, DateTime.Now - now);
                }
                HandleException(exception, commit ? "Commit" : "Rollback");
                flag = false;
            }
            finally
            {
                m_Transaction = null;
                if (m_TransactionOpenConnection)
                {
                    m_Connection.Close();
                }
                m_TransactionOpenConnection = false;
            }
            return flag;
        }

        public int ExecuteNonQuery(string strSql)
        {
            return ExecuteNonQuery(m_Transaction, strSql);
        }
        public int ExecuteNonQuery(string strSql, IDbDataParameter[] parameters)
        {
            return ExecuteNonQuery(m_Transaction, strSql, parameters);
        }

        public int ExecuteNonQuery(IDbTransaction transaction, string strSql)
        {
            return ExecuteNonQuery(transaction, strSql, null);
            //int num2;
            //bool flag = false;
            //DateTime now = DateTime.Now;
            //IDbCommand command = this.GetCommand(transaction);
            //try
            //{
            //    if (command.Connection.State != ConnectionState.Open)
            //    {
            //        command.Connection.Open();
            //        flag = true;
            //    }
            //    command.CommandType = CommandType.Text;
            //    command.Parameters.Clear();
            //    strSql = GetReplaceSql(strSql);
            //    command.CommandText = strSql;
            //    int result = command.ExecuteNonQuery();
            //    if (PlatformConfig.DBHandlerWriteLog)
            //    {
            //        DBHandlerLogSink.WriteExecuteNonQueryLog(strSql, result, (TimeSpan)(DateTime.Now - now));
            //    }
            //    num2 = result;
            //}
            //catch (Exception exception)
            //{
            //    if (PlatformConfig.DBHandlerWriteLog)
            //    {
            //        DBHandlerLogSink.WriteExecuteNonQueryExceptionLog(strSql, exception, (TimeSpan)(DateTime.Now - now));
            //    }
            //    this.HandleException(exception, strSql);
            //    num2 = -1;
            //}
            //finally
            //{
            //    if (flag)
            //    {
            //        command.Connection.Close();
            //    }
            //    command.Transaction = null;
            //}
            //return num2;
        }
        public int ExecuteNonQuery(IDbTransaction transaction, string strSql, IDbDataParameter[] parameters)
        {
            int num2;
            bool flag = false;
            DateTime now = DateTime.Now;
            IDbCommand command = getCommand(transaction);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                    flag = true;
                }
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                
                //王伟2007-12-05注
                //strSql = GetReplaceSql(strSql);
                command.CommandText = strSql;
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                int result = command.ExecuteNonQuery();
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteNonQueryLog(strSql, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteNonQueryExceptionLog(strSql, exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                num2 = -1;
            }
            finally
            {
                if (flag)
                {
                    command.Connection.Close();
                }
                command.Transaction = null;
            }
            return num2;
        }


        public static int ExecuteNonQueryOnce(string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.ExecuteNonQuery(strSql);
            buffer.FreeBuffer();
            return num;
        }
        public static int ExecuteNonQueryOnce(string strSql, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.ExecuteNonQuery(strSql, parameters);
            buffer.FreeBuffer();
            return num;
        }

        public static int[] ExecuteNonQueryOnce(string[] strSql)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer();
            try
            {
                if (!buffer.BeginTransaction())
                {
                    return null;
                }
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(strSql[i]);
                    if (numArray[i] < 0)
                    {
                        buffer.EndTransaction(false);
                        return null;
                    }
                }
                buffer.EndTransaction(true);
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int ExecuteNonQueryOnce(IDbTransaction transaction, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.ExecuteNonQuery(transaction, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int ExecuteNonQueryOnce(IDbTransaction transaction, string strSql, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.ExecuteNonQuery(transaction, strSql, parameters);
            buffer.FreeBuffer();
            return num;
        }

        public static int[] ExecuteNonQueryOnce(IDbTransaction transaction, string[] strSql)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer();
            try
            {
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(transaction, strSql[i]);
                    if (numArray[i] < 0)
                    {
                        return null;
                    }
                }
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int ExecuteNonQueryOnce(string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.ExecuteNonQuery(strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int[] ExecuteNonQueryOnce(string[] strSql, Type type)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer(type);
            try
            {
                if (!buffer.BeginTransaction())
                {
                    return null;
                }
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(strSql[i]);
                    if (numArray[i] < 0)
                    {
                        buffer.EndTransaction(false);
                        return null;
                    }
                }
                buffer.EndTransaction(true);
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int[] ExecuteNonQueryOnce(IDbTransaction transaction, string[] strSql, Type type)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer(type);
            try
            {
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(transaction, strSql[i]);
                    if (numArray[i] < 0)
                    {
                        return null;
                    }
                }
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int ExecuteNonQueryOnce(IDbTransaction transaction, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.ExecuteNonQuery(transaction, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int ExecuteNonQueryOnce(string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.ExecuteNonQuery(strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int[] ExecuteNonQueryOnce(string[] strSql, Type type, string connectionString)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer(type);
            try
            {
                if (!buffer.BeginTransaction())
                {
                    return null;
                }
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(strSql[i]);
                    if (numArray[i] < 0)
                    {
                        buffer.EndTransaction(false);
                        return null;
                    }
                }
                buffer.EndTransaction(true);
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int[] ExecuteNonQueryOnce(IDbTransaction transaction, string[] strSql, Type type, string connectionString)
        {
            int[] numArray2;
            DBHandler buffer = GetBuffer(type);
            try
            {
                int[] numArray = new int[strSql.Length];
                for (int i = 0; i < strSql.Length; i++)
                {
                    numArray[i] = buffer.ExecuteNonQuery(transaction, strSql[i]);
                    if (numArray[i] < 0)
                    {
                        return null;
                    }
                }
                numArray2 = numArray;
            }
            finally
            {
                buffer.FreeBuffer();
            }
            return numArray2;
        }

        public static int ExecuteNonQueryOnce(IDbTransaction transaction, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.ExecuteNonQuery(transaction, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public object ExecuteProc(string procName)
        {
            return ExecuteProc(m_Transaction, procName, null, null, null);
        }

        public object ExecuteProc(IDbTransaction transaction, string procName)
        {
            return ExecuteProc(transaction, procName, null, null, null);
        }

        public object ExecuteProc(string procName, IDbDataParameter[] parameters)
        {
            return ExecuteProc(m_Transaction, procName, parameters, null);
        }

        public object ExecuteProc(string procName, string[] outParameters)
        {
            return ExecuteProc(m_Transaction, procName, null, null, outParameters);
        }

        public object ExecuteProc(IDbTransaction transaction, string procName, string[] outParameters)
        {
            return ExecuteProc(transaction, procName, null, null, outParameters);
        }

        public object ExecuteProc(string procName, IDbDataParameter[] parameters, string[] outParameters)
        {
            return ExecuteProc(m_Transaction, procName, parameters, outParameters);
        }

        public object ExecuteProc(string procName, string[] parametersName, object[] parameters)
        {
            return ExecuteProc(m_Transaction, procName, parametersName, parameters, null);
        }

        public object ExecuteProc(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, string[] outParameters)
        {
            object obj3;
            bool flag = false;
            DateTime now = DateTime.Now;
            IDbCommand command = getCommand(transaction);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                    flag = true;
                }
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                command.CommandText = procName;
                int num = command.ExecuteNonQuery();
                object result = command.Parameters.Contains("ReturnValue") ? ((IDbDataParameter)command.Parameters["ReturnValue"]).Value : num;
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteProcLog(procName, result, DateTime.Now - now);
                }
                if ((outParameters != null) && (outParameters.Length > 0))
                {
                    object[] objArray = new object[outParameters.Length + 1];
                    objArray[0] = result;
                    for (int i = 0; i < outParameters.Length; i++)
                    {
                        if ((outParameters[i] != null) && (outParameters[i] != ""))
                        {
                            IDbDataParameter parameter2 = command.Parameters[outParameters[i]] as IDbDataParameter;
                            if (parameter2 != null)
                            {
                                objArray[i + 1] = parameter2.Value;
                            }
                        }
                    }
                    result = objArray;
                }
                obj3 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteProcExceptionLog(procName, exception, DateTime.Now - now);
                }
                HandleException(exception, procName);
                obj3 = null;
            }
            finally
            {
                if (flag)
                {
                    command.Connection.Close();
                }
                command.Transaction = null;
            }
            return obj3;
        }

        public object ExecuteProc(string procName, string[] parametersName, object[] parameters, string[] outParameters)
        {
            return ExecuteProc(m_Transaction, procName, parametersName, parameters, outParameters);
        }

        public object ExecuteProc(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, string[] outParameters)
        {
            object obj3;
            DateTime now = DateTime.Now;
            if ((parametersName != null) && (parametersName.Length > 0))
            {
                if (parameters == null)
                {
                    if (PlatformConfig.DBHandlerWriteLog)
                    {
                        DBHandlerLogSink.WriteExecuteProcLog(procName, "参数个数不匹配", DateTime.Now - now);
                    }
                    return null;
                }
                if (parameters.Length != parametersName.Length)
                {
                    if (PlatformConfig.DBHandlerWriteLog)
                    {
                        DBHandlerLogSink.WriteExecuteProcLog(procName, "参数个数不匹配", DateTime.Now - now);
                    }
                    return null;
                }
            }
            bool flag = false;
            IDbCommand cmd = getCommand(transaction);
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                    flag = true;
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;
                cmd.Parameters.Clear();
                if (!DBBuilder.DeriveParameters(cmd))
                {
                    if (PlatformConfig.DBHandlerWriteLog)
                    {
                        DBHandlerLogSink.WriteExecuteProcLog(procName, "获取存储过程参数错误", DateTime.Now - now);
                    }
                    return null;
                }
                if ((parametersName != null) && (parametersName.Length > 0))
                {
                    for (int i = 0; i < parametersName.Length; i++)
                    {
                        IDbDataParameter parameter = cmd.Parameters[parametersName[i]] as IDbDataParameter;
                        if (parameter != null)
                        {
                            if ((parameters[i] == null) || ((parameters[i] as string) == ""))
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else
                            {
                                parameter.Value = parameters[i];
                            }
                        }
                    }
                }
                int num2 = cmd.ExecuteNonQuery();
                object result = cmd.Parameters.Contains("ReturnValue") ? ((IDbDataParameter)cmd.Parameters["ReturnValue"]).Value : num2;
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteProcLog(procName, result, DateTime.Now - now);
                }
                if ((outParameters != null) && (outParameters.Length > 0))
                {
                    object[] objArray = new object[outParameters.Length + 1];
                    objArray[0] = result;
                    for (int j = 0; j < outParameters.Length; j++)
                    {
                        if ((outParameters[j] != null) && (outParameters[j] != ""))
                        {
                            IDbDataParameter parameter2 = cmd.Parameters[outParameters[j]] as IDbDataParameter;
                            if (parameter2 != null)
                            {
                                objArray[j + 1] = (parameter2.Value == DBNull.Value) ? null : parameter2.Value;
                            }
                        }
                    }
                    result = objArray;
                }
                obj3 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteProcExceptionLog(procName, exception, DateTime.Now - now);
                }
                HandleException(exception, procName);
                obj3 = null;
            }
            finally
            {
                if (flag)
                {
                    cmd.Connection.Close();
                }
                cmd.Transaction = null;
            }
            return obj3;
        }

        public static object ExecuteProcOnce(string procName)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, Type type)
        {
            return ExecuteProcOnce(procName, null, null, null, type);
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, Type type)
        {
            return ExecuteProcOnce(transaction, procName, null, null, null, type);
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, Type type, string connectionString)
        {
            return ExecuteProcOnce(procName, null, null, null, type, connectionString);
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] outParameters, Type type)
        {
            return ExecuteProcOnce(procName, null, null, outParameters, type);
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, Type type, string connectionString)
        {
            return ExecuteProcOnce(transaction, procName, null, null, null, type, connectionString);
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] outParameters, Type type)
        {
            return ExecuteProcOnce(transaction, procName, null, null, outParameters, type);
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters, string[] outParameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] outParameters, Type type, string connectionString)
        {
            return ExecuteProcOnce(procName, null, null, outParameters, type, connectionString);
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, string[] outParameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, string[] outParameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] outParameters, Type type, string connectionString)
        {
            return ExecuteProcOnce(transaction, procName, null, null, outParameters, type, connectionString);
        }

        public static object ExecuteProcOnce(string procName, IDbDataParameter[] parameters, string[] outParameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters, string[] outParameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, IDbDataParameter[] parameters, string[] outParameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(transaction, procName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, null);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, string[] outParameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(string procName, string[] parametersName, object[] parameters, string[] outParameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteProcOnce(IDbTransaction transaction, string procName, string[] parametersName, object[] parameters, string[] outParameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteProc(transaction, procName, parametersName, parameters, outParameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public IDataReader ExecuteReader(string strSql)
        {
            return ExecuteReader(strSql, CommandBehavior.Default);
        }

        public IDataReader ExecuteReader(string strSql, CommandBehavior behavior)
        {
            IDataReader reader2;
            bool flag = false;
            DateTime now = DateTime.Now;
            IDbCommand command = getCommand(m_Transaction);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                    flag = true;
                }
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                //strSql = GetReplaceSql(strSql);
                command.CommandText = strSql;
                IDataReader reader = command.ExecuteReader(behavior);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteReaderLog(strSql, behavior, DateTime.Now - now);
                }
                reader2 = reader;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteReaderExceptionLog(strSql, behavior, exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                reader2 = null;
            }
            finally
            {
                if (flag)
                {
                    command.Connection.Close();
                }
                command.Transaction = null;
            }
            return reader2;
        }

        public object ExecuteScalar(string strSql)
        {
            return ExecuteScalar(m_Transaction, strSql);
        }

        public object ExecuteScalar(string strSql, IDbDataParameter[] parameters)
        {
            return ExecuteScalar(m_Transaction, strSql, parameters);
        }

        public object ExecuteScalar(IDbTransaction transaction, string strSql)
        {
            return ExecuteScalar(transaction, strSql, null);
        }

        public object ExecuteScalar(IDbTransaction transaction, string strSql, IDbDataParameter[] parameters)
        {
            object obj3;
            bool flag = false;
            DateTime now = DateTime.Now;
            IDbCommand command = getCommand(transaction);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                    flag = true;
                }
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                //strSql = GetReplaceSql(strSql);
                command.CommandText = strSql;
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);    
                    }
                }

                object result = command.ExecuteScalar();
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteScalarLog(strSql, result, DateTime.Now - now);
                }
                obj3 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteExecuteScalarExceptionLog(strSql, exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                obj3 = null;
            }
            finally
            {
                if (flag)
                {
                    command.Connection.Close();
                }
                command.Transaction = null;
            }
            return obj3;
        }

        public static object ExecuteScalarOnce(string strSql)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteScalar(strSql);
            buffer.FreeBuffer();
            return obj2;
        }
        public static object ExecuteScalarOnce(string strSql, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteScalar(strSql, parameters);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteScalarOnce(IDbTransaction transaction, string strSql)
        {
            DBHandler buffer = GetBuffer();
            object obj2 = buffer.ExecuteScalar(transaction, strSql);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteScalarOnce(string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteScalar(strSql);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteScalarOnce(IDbTransaction transaction, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            object obj2 = buffer.ExecuteScalar(transaction, strSql);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteScalarOnce(string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteScalar(strSql);
            buffer.FreeBuffer();
            return obj2;
        }

        public static object ExecuteScalarOnce(IDbTransaction transaction, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            object obj2 = buffer.ExecuteScalar(transaction, strSql);
            buffer.FreeBuffer();
            return obj2;
        }

        public DataSet Fill(string strSql)
        {
            DataSet dataSet = new DataSet();
            if (Fill(dataSet, strSql) < 0)
            {
                return null;
            }
            return dataSet;
        }

        public int Fill(DataSet dataSet, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(dataSet, strSql);
            }
            return Fill(dataSet.Tables[0], strSql);
        }

        public int Fill(DataSet dataSet, string strSql, IDbDataParameter[] parameters)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(dataSet, strSql, parameters);
            }
            return Fill(dataSet.Tables[0], strSql, parameters);
        }

        public int Fill(DataTable dataTable, string strSql)
        {
            return Fill(m_Transaction, dataTable, strSql);
        }

        public int Fill(DataTable dataTable, string strSql, IDbDataParameter[] parameters)
        {
            return Fill(m_Transaction, dataTable, strSql, parameters);
        }

        public DataSet Fill(IDbTransaction transaction, string strSql)
        {
            DataSet dataSet = new DataSet();
            if (Fill(transaction, dataSet, strSql) < 0)
            {
                return null;
            }
            return dataSet;
        }

        public DataSet Fill(string strSql, string tableName)
        {
            DataTable table;
            DataSet dataSetByName = DataSetUtility.GetDataSetByName(tableName);
            if (dataSetByName != null)
            {
                table = dataSetByName.Tables[tableName];
            }
            else
            {
                dataSetByName = new DataSet();
                table = dataSetByName.Tables.Add(tableName);
            }
            if (Fill(table, strSql) < 0)
            {
                return null;
            }
            return dataSetByName;
        }

        public int Fill(DataSet dataSet, int tableIndex, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(dataSet, strSql);
            }
            return Fill(dataSet.Tables[tableIndex], strSql);
        }

        public int Fill(DataSet dataSet, string tableName, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(dataSet, tableName, strSql);
            }
            DataTable dataTable = dataSet.Tables[tableName] ?? dataSet.Tables.Add(tableName);
            return Fill(dataTable, strSql);
        }

        public int Fill(IDbTransaction transaction, DataSet dataSet, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(transaction, dataSet, strSql);
            }
            return Fill(transaction, dataSet.Tables[0], strSql);
        }

        public int Fill(IDbTransaction transaction, DataTable dataTable, string strSql)
        {
            if (dataTable.DataSet != null && dataTable.DataSet.GetType() == _dataSetType)
            {
                return FillNoName(transaction, dataTable, strSql);
            }
            return Fill(transaction, dataTable, strSql, null);
        }
        public int Fill(IDbTransaction transaction, DataTable dataTable, string strSql, IDbDataParameter[] parameters)
        {
            int num2;
            if (dataTable == null)
            {
                return -1;
            }
            if ((dataTable.DataSet == null) || (dataTable.DataSet.GetType() == _dataSetType))
            {
                return FillNoName(transaction, dataTable, strSql);
            }
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(transaction, dataTable.TableName);
            try
            {
                installAdapter(transaction, dataAdapter);
                //strSql = GetReplaceSql(strSql);
                dataAdapter.SelectCommand.Parameters.Clear();
                dataAdapter.SelectCommand.CommandText = strSql;
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(parameter);
                    }
                }
                int result = ((DbDataAdapter)dataAdapter).Fill(dataTable);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillLog(strSql, dataTable.GetType(), dataTable.TableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                dataTable.Clear();
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillExceptionLog(strSql, dataTable.GetType(), dataTable.TableName, exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                num2 = -1;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return num2;
        }

        public DataSet Fill(IDbTransaction transaction, string strSql, string tableName)
        {
            DataTable table;
            DataSet dataSetByName = DataSetUtility.GetDataSetByName(tableName);
            if (dataSetByName != null)
            {
                table = dataSetByName.Tables[tableName];
            }
            else
            {
                dataSetByName = new DataSet();
                table = dataSetByName.Tables.Add(tableName);
            }
            if (Fill(transaction, table, strSql) < 0)
            {
                return null;
            }
            return dataSetByName;
        }

        public DataSet Fill(string strSql, string tableName, string strNamespace)
        {
            DataTable table;
            DataSet dataSetByName = DataSetUtility.GetDataSetByName(strNamespace, tableName);
            if (dataSetByName != null)
            {
                table = dataSetByName.Tables[tableName];
            }
            else
            {
                dataSetByName = new DataSet();
                table = dataSetByName.Tables.Add(tableName);
            }
            if (Fill(table, strSql) < 0)
            {
                return null;
            }
            return dataSetByName;
        }

        public int Fill(IDbTransaction transaction, DataSet dataSet, int tableIndex, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(transaction, dataSet, strSql);
            }
            return Fill(transaction, dataSet.Tables[tableIndex], strSql);
        }

        public int Fill(IDbTransaction transaction, DataSet dataSet, string tableName, string strSql)
        {
            if (dataSet.GetType() == _dataSetType)
            {
                return FillNoName(transaction, dataSet, tableName, strSql);
            }
            DataTable dataTable = dataSet.Tables[tableName] ?? dataSet.Tables.Add(tableName);
            return Fill(transaction, dataTable, strSql);
        }

        public DataSet Fill(IDbTransaction transaction, string strSql, string tableName, string strNamespace)
        {
            DataTable table;
            DataSet dataSetByName = DataSetUtility.GetDataSetByName(strNamespace, tableName);
            if (dataSetByName != null)
            {
                table = dataSetByName.Tables[tableName];
            }
            else
            {
                dataSetByName = new DataSet();
                table = dataSetByName.Tables.Add(tableName);
            }
            if (Fill(transaction, table, strSql) < 0)
            {
                return null;
            }
            return dataSetByName;
        }

        public bool FillDataSetSchema(DataSet ds, string[] tableNames)
        {
            DateTime now = DateTime.Now;
            try
            {
                foreach (string text in tableNames)
                {
                    IDbDataAdapter dataAdapter = GetDataAdapter(text);
                    try
                    {
                        if (dataAdapter == null)
                        {
                            return false;
                        }
                        DataTable[] tableArray = dataAdapter.FillSchema(ds, SchemaType.Source);
                        if ((tableArray == null) || (tableArray.Length < 1))
                        {
                            return false;
                        }
                        tableArray[0].TableName = text;
                    }
                    finally
                    {
                        freeAdapter(dataAdapter);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillDataSetSchemaExceptionLog(tableNames, exception, DateTime.Now - now);
                }
                HandleException(exception);
                return false;
            }
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string tableName)
        {
            DBHandler buffer = GetBuffer();
            bool flag = buffer.FillDataSetSchema(ds, new[] { tableName });
            buffer.FreeBuffer();
            return flag;
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string[] tableNames)
        {
            DBHandler buffer = GetBuffer();
            bool flag = buffer.FillDataSetSchema(ds, tableNames);
            buffer.FreeBuffer();
            return flag;
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string[] tableNames, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            bool flag = buffer.FillDataSetSchema(ds, tableNames);
            buffer.FreeBuffer();
            return flag;
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            bool flag = buffer.FillDataSetSchema(ds, new[] { tableName });
            buffer.FreeBuffer();
            return flag;
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string[] tableNames, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            bool flag = buffer.FillDataSetSchema(ds, tableNames);
            buffer.FreeBuffer();
            return flag;
        }

        public static bool FillDataSetSchemaOnce(DataSet ds, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            bool flag = buffer.FillDataSetSchema(ds, new[] { tableName });
            buffer.FreeBuffer();
            return flag;
        }

        public int FillNoName(DataSet dataSet, string strSql)
        {
            return FillNoName(m_Transaction, dataSet, null, strSql);
        }

        public int FillNoName(DataSet dataSet, string strSql, IDbDataParameter[] parameters)
        {
            return FillNoName(m_Transaction, dataSet,strSql, parameters);
        }

        public int FillNoName(DataTable dataTable, string strSql)
        {
            return FillNoName(m_Transaction, dataTable, strSql);
        }

        public int FillNoName(DataTable dataTable, string strSql, IDbDataParameter[] parameters)
        {
            return FillNoName(m_Transaction, dataTable, strSql, parameters);
        }

        public int FillNoName(DataSet dataSet, string tableName, string strSql)
        {
            return FillNoName(m_Transaction, dataSet, tableName, strSql);
        }

        public int FillNoName(IDbTransaction transaction, DataSet dataSet, string strSql)
        {
            return FillNoName(transaction, dataSet, strSql, new IDbDataParameter[] { });
        }

        public int FillNoName(IDbTransaction transaction, DataSet dataSet, string strSql, IDbDataParameter[] parameters)
        {
            int num2;
            if (m_Adapter == null)
            {
                return -1;
            }
            if (dataSet == null)
            {
                return -1;
            }
            DateTime now = DateTime.Now;
            try
            {
                installAdapter(transaction, m_Adapter);
                //strSql = GetReplaceSql(strSql);
                m_Adapter.SelectCommand.Parameters.Clear();
                m_Adapter.SelectCommand.CommandText = strSql;
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        m_Adapter.SelectCommand.Parameters.Add(parameter);
                    }
                }
                int result = m_Adapter.Fill(dataSet);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillLog(strSql, dataSet.GetType(), result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                dataSet.Clear();
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillExceptionLog(strSql, dataSet.GetType(), exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                num2 = -1;
            }
            finally
            {
                freeAdapter(m_Adapter);
            }
            return num2;
        }


        public int FillNoName(IDbTransaction transaction, DataTable dataTable, string strSql, IDbDataParameter[] parameters)
        {
            int num2;
            if (m_Adapter == null)
            {
                return -1;
            }
            if (dataTable == null)
            {
                return -1;
            }
            DateTime now = DateTime.Now;
            try
            {
                installAdapter(transaction, m_Adapter);
                //strSql = GetReplaceSql(strSql);
                m_Adapter.SelectCommand.Parameters.Clear();
                m_Adapter.SelectCommand.CommandText = strSql;
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        m_Adapter.SelectCommand.Parameters.Add(parameter);
                    }
                }

                int result = ((DbDataAdapter)m_Adapter).Fill(dataTable);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillLog(strSql, dataTable.GetType(), dataTable.TableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                dataTable.Clear();
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillExceptionLog(strSql, dataTable.GetType(), dataTable.TableName, exception, DateTime.Now - now);
                }
                HandleException(exception, strSql);
                num2 = -1;
            }
            finally
            {
                freeAdapter(m_Adapter);
            }
            return num2;
        }

        public int FillNoName(IDbTransaction transaction, DataTable dataTable, string strSql)
        {
            return FillNoName(transaction, dataTable, strSql, null);
        }

        public int FillNoName(IDbTransaction transaction, DataSet dataSet, string tableName, string strSql)
        {
            int num2;
            if (m_Adapter == null)
            {
                return -1;
            }
            if (dataSet == null)
            {
                return -1;
            }
            if (tableName == null)
            {
                return FillNoName(transaction, dataSet, strSql);
            }
            DataTable dataTable = null;
            DateTime now = DateTime.Now;
            try
            {
                installAdapter(transaction, m_Adapter);
                dataTable = dataSet.Tables[tableName] ?? dataSet.Tables.Add(tableName);
                //strSql = GetReplaceSql(strSql);
                m_Adapter.SelectCommand.CommandText = strSql;
                int result = ((DbDataAdapter)m_Adapter).Fill(dataTable);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillLog(strSql, dataSet.GetType(), tableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (dataTable != null)
                {
                    dataTable.Clear();
                }
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteFillExceptionLog(strSql, dataSet.GetType(), tableName, exception, DateTime.Now - now);
                }
                HandleException(exception, "表名：" + tableName + "\r\n" + strSql);
                num2 = -1;
            }
            finally
            {
                freeAdapter(m_Adapter);
            }
            return num2;
        }

        public static int FillNoNameOnce(DataSet dataSet, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.FillNoName(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillNoNameOnce(DataSet dataSet, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.FillNoName(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillNoNameOnce(IDbTransaction transaction, DataSet dataSet, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.FillNoName(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillNoNameOnce(DataSet dataSet, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.FillNoName(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillNoNameOnce(IDbTransaction transaction, DataSet dataSet, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.FillNoName(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillNoNameOnce(IDbTransaction transaction, DataSet dataSet, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.FillNoName(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(string strSql)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static int FillOnce(DataSet dataSet, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string strSql, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataSet, strSql, parameters);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataTable dataTable, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataTable dataTable, string strSql, IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataTable, strSql, parameters);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(transaction, strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static int FillOnce(DataSet dataSet, int tableIndex, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string tableName, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataTable dataTable, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataTable dataTable, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(transaction, dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(transaction, strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(transaction, strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName, string strNamespace)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static int FillOnce(DataSet dataSet, int tableIndex, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string tableName, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataTable dataTable, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(transaction, dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string tableName, string strSql)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Fill(transaction, dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataTable dataTable, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(transaction, dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName, string strNamespace)
        {
            DBHandler buffer = GetBuffer();
            DataSet set = buffer.Fill(transaction, strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(transaction, strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(transaction, strSql);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName, string strNamespace, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static int FillOnce(DataSet dataSet, int tableIndex, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(DataSet dataSet, string tableName, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(transaction, dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string tableName, string strSql, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Fill(transaction, dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(transaction, dataSet, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataTable dataTable, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(transaction, dataTable, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName, string strNamespace, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet set = buffer.Fill(transaction, strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(transaction, strSql, tableName);
            buffer.FreeBuffer();
            return set;
        }

        public static DataSet FillOnce(string strSql, string tableName, string strNamespace, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(transaction, dataSet, tableIndex, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static int FillOnce(IDbTransaction transaction, DataSet dataSet, string tableName, string strSql, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Fill(transaction, dataSet, tableName, strSql);
            buffer.FreeBuffer();
            return num;
        }

        public static DataSet FillOnce(IDbTransaction transaction, string strSql, string tableName, string strNamespace, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet set = buffer.Fill(transaction, strSql, tableName, strNamespace);
            buffer.FreeBuffer();
            return set;
        }

        private void freeAdapter(IDbDataAdapter adapter)
        {
            if (adapter.SelectCommand != null)
            {
                adapter.SelectCommand.Transaction = null;
                adapter.SelectCommand.Connection = m_Connection;
            }
            if (adapter.InsertCommand != null)
            {
                adapter.InsertCommand.Transaction = null;
                adapter.InsertCommand.Connection = m_Connection;
            }
            if (adapter.UpdateCommand != null)
            {
                adapter.UpdateCommand.Transaction = null;
                adapter.UpdateCommand.Connection = m_Connection;
            }
            if (adapter.DeleteCommand != null)
            {
                adapter.DeleteCommand.Transaction = null;
                adapter.DeleteCommand.Connection = m_Connection;
            }
        }

        public void FreeBuffer()
        {
            ArrayList list;
            try
            {
                if (m_Transaction != null)
                {
                    PlatformLogSink.Write(StackTraceUtility.GetStack() + "\r\n存在可能没提交的事务");
                    EndTransaction(false);
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
            }
            try
            {
                if ((m_Connection != null) && (m_Connection.State == ConnectionState.Open))
                {
                    PlatformLogSink.Write(StackTraceUtility.GetStack() + "\r\n数据库连接未关闭");
                    m_Connection.Close();
                }
            }
            catch (Exception exception2)
            {
                ExceptionHelper.HandleException(exception2);
            }
            lock (m_UnusedDBHandlers.SyncRoot)
            {
                list = m_UnusedDBHandlers[m_DBHandlerTypeKey] as ArrayList;
                if (list == null)
                {
                    list = new ArrayList();
                    m_UnusedDBHandlers[m_DBHandlerTypeKey] = list;
                }
            }
            lock (list.SyncRoot)
            {
                list.Add(this);
            }
        }

        public static DBHandler GetBuffer()
        {
            return GetBuffer(m_DefaultType, m_DefaultConnectionString);
        }

        public static DBHandler GetBuffer(Type type)
        {
            return GetBuffer(type, null);
        }

        public static DBHandler GetBuffer(Type type, string connectionString)
        {
            object obj2;
            ArrayList list;
            CleanException();
            bool flag = false;
            if (isConnection(type))
            {
                flag = true;
                obj2 = type + connectionString;
            }
            else
            {
                obj2 = type;
            }
            lock (m_UnusedDBHandlers.SyncRoot)
            {
                list = m_UnusedDBHandlers[obj2] as ArrayList;
                if (list == null)
                {
                    list = new ArrayList();
                    m_UnusedDBHandlers[obj2] = list;
                }
            }
            DBHandler handler = null;
            lock (list.SyncRoot)
            {
                if (list.Count > 0)
                {
                    handler = list[list.Count - 1] as DBHandler;
                    list.RemoveAt(list.Count - 1);
                }
            }
            if (handler == null)
            {
                handler = new DBHandler();
                if (isConnection(type))
                {
                    handler.m_Object = Activator.CreateInstance(type);
                    ((IDbConnection)handler.m_Object).ConnectionString = connectionString;
                }
                else
                {
                    handler.m_Object = Activator.CreateInstance(type);
                }
                handler.m_DBHandlerTypeKey = obj2;
                handler.m_IsConnectionType = flag;
                handler.m_CurrentType = type;
                handler.initializeAdapterTable();
            }
            return handler;
        }

        private IDbCommand getCommand(IDbTransaction transaction)
        {
            if ((transaction != null) && (transaction != m_Transaction))
            {
                return DBBuilder.CreateCommand(transaction.Connection, transaction);
            }
            m_Command.Transaction = transaction;
            m_Command.Connection = m_Connection;
            return m_Command;
        }

        public IDbDataAdapter GetDataAdapter(string tableName)
        {
            return GetDataAdapter(m_Transaction, tableName);
        }

        public IDbDataAdapter GetDataAdapter(IDbTransaction transaction, string tableName)
        {
            IDbDataAdapter adapter = m_Adapters[tableName.ToUpper()] as IDbDataAdapter;
            if (adapter != null)
            {
                return adapter;
            }
            IDbConnection connection = ((transaction == m_Transaction) || (transaction == null)) ? m_Connection : transaction.Connection;
            adapter = DBBuilder.CreateAdapter(connection, tableName, transaction);
            if (adapter != null)
            {
                m_Adapters[tableName.ToUpper()] = adapter;
                return adapter;
            }
            return m_Adapter;
        }

        public DataSet GetDataSet(string tableName)
        {
            DataSet set3;
            Hashtable hashtable = m_DBHandlerTypeDataSets[m_DBHandlerTypeKey] as Hashtable;
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                m_DBHandlerTypeDataSets[m_DBHandlerTypeKey] = hashtable;
            }
            DataSet set = hashtable[tableName.ToUpper()] as DataSet;
            if (set != null)
            {
                return set.Clone();
            }
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(tableName);
            if (dataAdapter == null)
            {
                return null;
            }
            try
            {
                DataSet dataSet = new DataSet("DataSet_" + tableName.ToUpper());
                DataTable[] tableArray = dataAdapter.FillSchema(dataSet, SchemaType.Source);
                if ((tableArray == null) || (tableArray.Length < 1))
                {
                    return null;
                }
                tableArray[0].TableName = tableName;
                hashtable[tableName.ToUpper()] = dataSet;
                set3 = dataSet.Clone();
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteGetDataSetExceptionLog(tableName, exception, DateTime.Now - now);
                }
                HandleException(exception);
                set3 = null;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return set3;
        }

        public static DataSet GetDataSetOnce(string tableName)
        {
            DBHandler buffer = GetBuffer();
            DataSet dataSet = buffer.GetDataSet(tableName);
            buffer.FreeBuffer();
            return dataSet;
        }

        public static DataSet GetDataSetOnce(string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            DataSet dataSet = buffer.GetDataSet(tableName);
            buffer.FreeBuffer();
            return dataSet;
        }

        public static DataSet GetDataSetOnce(string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            DataSet dataSet = buffer.GetDataSet(tableName);
            buffer.FreeBuffer();
            return dataSet;
        }

        public static string GetException()
        {
            return (Thread.GetData(m_ExceptionSlot) as string);
        }

        public Hashtable GetProcedureParameters(string procName, out IDbDataParameter[] parameters)
        {
            return GetProcedureParameters(m_Connection, procName, out parameters);
        }

        public Hashtable GetProcedureParameters(IDbConnection connection, string procName, out IDbDataParameter[] parameters)
        {
            string text = procName.ToUpper();
            IDbCommand command = m_ProcParaeters[text] as IDbCommand;
            if (command == null)
            {
                command = DBBuilder.DeriveParameters(connection, procName);
                if (command == null)
                {
                    parameters = null;
                    return null;
                }
                m_ProcParaeters[text] = command;
            }
            parameters = DBBuilder.CreateParameter(connection, command.Parameters.Count);
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].DbType = ((IDbDataParameter)command.Parameters[i]).DbType;
                parameters[i].Direction = ((IDbDataParameter)command.Parameters[i]).Direction;
                parameters[i].ParameterName = ((IDbDataParameter)command.Parameters[i]).ParameterName;
                parameters[i].Precision = ((IDbDataParameter)command.Parameters[i]).Precision;
                parameters[i].Scale = ((IDbDataParameter)command.Parameters[i]).Scale;
                parameters[i].Size = ((IDbDataParameter)command.Parameters[i]).Size;
                hashtable[parameters[i].ParameterName] = parameters[i];
            }
            return hashtable;
        }

        public static Hashtable GetProcedureParametersOnce(string procName, out IDbDataParameter[] parameters)
        {
            DBHandler buffer = GetBuffer();
            Hashtable procedureParameters = buffer.GetProcedureParameters(procName, out parameters);
            buffer.FreeBuffer();
            return procedureParameters;
        }

        public static Hashtable GetProcedureParametersOnce(string procName, out IDbDataParameter[] parameters, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            Hashtable procedureParameters = buffer.GetProcedureParameters(procName, out parameters);
            buffer.FreeBuffer();
            return procedureParameters;
        }

        public static Hashtable GetProcedureParametersOnce(string procName, out IDbDataParameter[] parameters, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            Hashtable procedureParameters = buffer.GetProcedureParameters(procName, out parameters);
            buffer.FreeBuffer();
            return procedureParameters;
        }

        public static DataSet GetRegisterDataSet(string tableName)
        {
            Type type = m_DataSetTypeTable[tableName.ToUpper()] as Type;
            if (type == null)
            {
                return null;
            }
            return (Activator.CreateInstance(type) as DataSet);
        }

        private static string getReplaceSql(string sql)
        {
            if (EnableDBHandlerReplaceSql)
            {
                string text = _dbHanderSqlReplaceTable[sql.ToUpper().Trim()] as string;
                if (text != null)
                {
                    return text;
                }
            }
            return sql;
        }

        [StackTracePass]
        private void HandleException(Exception exp)
        {
            Thread.SetData(m_ExceptionSlot, exp.Message);
            if (PlatformConfig.LogDBException)
            {
                ExceptionHelper.HandleException(exp);
            }
        }

        [StackTracePass]
        private void HandleException(Exception exp, string message)
        {
            Thread.SetData(m_ExceptionSlot, exp.Message + "\r\n" + message);
            if (PlatformConfig.LogDBException)
            {
                ExceptionHelper.HandleException(exp, new[] { message });
            }
        }

        private void initializeAdapterTable()
        {
            m_Adapters.Clear();
            if (m_IsConnectionType)
            {
                m_Connection = m_Object as IDbConnection;
            }
            else
            {
                foreach (FieldInfo info in m_CurrentType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (isConnection(info.FieldType))
                    {
                        m_Connection = info.GetValue(m_Object) as IDbConnection;
                    }
                    else if (isDbAdapter(info.FieldType))
                    {
                        DbDataAdapter adapter = info.GetValue(m_Object) as DbDataAdapter;
                        if ((adapter != null) && (adapter.TableMappings.Count >= 1))
                        {
                            DataTableMapping mapping = adapter.TableMappings[0];
                            if (mapping != null)
                            {
                                m_Adapters[mapping.DataSetTable.ToUpper()] = adapter;
                            }
                        }
                    }
                }
                if (m_Connection == null)
                {
                    throw new Exception("无法找到有效的数据库连接");
                }
            }
            if (DBBuilder.GetDBType(m_Connection) < DBType.ODBC)
            {
                throw new Exception("不支持数据库连接类型");
            }
            m_Adapter = DBBuilder.CreateAdapter(m_Connection);
            m_Command = DBBuilder.CreateCommand(m_Connection);
        }

        private void installAdapter(IDbTransaction transaction, IDbDataAdapter adapter)
        {
            if (adapter.SelectCommand != null)
            {
                adapter.SelectCommand.Transaction = transaction;
                if ((transaction == null) || (transaction == m_Transaction))
                {
                    adapter.SelectCommand.Connection = m_Connection;
                }
                else
                {
                    adapter.SelectCommand.Connection = transaction.Connection;
                }
            }
            if (adapter.InsertCommand != null)
            {
                adapter.InsertCommand.Transaction = transaction;
                if ((transaction == null) || (transaction == m_Transaction))
                {
                    adapter.InsertCommand.Connection = m_Connection;
                }
                else
                {
                    adapter.InsertCommand.Connection = transaction.Connection;
                }
            }
            if (adapter.UpdateCommand != null)
            {
                adapter.UpdateCommand.Transaction = transaction;
                if ((transaction == null) || (transaction == m_Transaction))
                {
                    adapter.UpdateCommand.Connection = m_Connection;
                }
                else
                {
                    adapter.UpdateCommand.Connection = transaction.Connection;
                }
            }
            if (adapter.DeleteCommand != null)
            {
                adapter.DeleteCommand.Transaction = transaction;
                if ((transaction == null) || (transaction == m_Transaction))
                {
                    adapter.DeleteCommand.Connection = m_Connection;
                }
                else
                {
                    adapter.DeleteCommand.Connection = transaction.Connection;
                }
            }
        }

        private static bool isConnection(Type type)
        {
            if ((type != typeof(IDbConnection)) && (type.GetInterface("System.Data.IDbConnection") == null))
            {
                return false;
            }
            return true;
        }

        private static bool isDbAdapter(Type type)
        {
            if ((type != typeof(IDbDataAdapter)) && (type.GetInterface("System.Data.IDbDataAdapter") == null))
            {
                return false;
            }
            return true;
        }

        public bool OpenConnection()
        {
            if (m_Connection.State == ConnectionState.Open)
            {
                return true;
            }
            try
            {
                m_Connection.Open();
                return true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return false;
            }
        }

        public static void PreStoreBuffer(int count)
        {
            PreStoreBuffer(count, m_DefaultType, m_DefaultConnectionString);
        }

        public static void PreStoreBuffer(int count, Type type)
        {
            PreStoreBuffer(count, type, null);
        }

        public static void PreStoreBuffer(int count, Type type, string connectionString)
        {
            object obj2;
            bool flag = false;
            if (isConnection(type))
            {
                flag = true;
                obj2 = type + connectionString;
            }
            else
            {
                obj2 = type;
            }
            ArrayList list = m_UnusedDBHandlers[obj2] as ArrayList;
            if (list == null)
            {
                list = new ArrayList();
                m_UnusedDBHandlers[obj2] = list;
            }
            for (int i = 0; i < count; i++)
            {
                DBHandler handler = new DBHandler
                {
                    m_Object =
                        isConnection(type)
                            ? Activator.CreateInstance(type, new object[] {connectionString})
                            : Activator.CreateInstance(type),
                    m_DBHandlerTypeKey = obj2,
                    m_IsConnectionType = flag,
                    m_CurrentType = type
                };
                handler.initializeAdapterTable();
                list.Add(handler);
            }
        }

        public static bool RegisterAssemblyDataSets(Assembly asm)
        {
            if (asm == null)
            {
                return false;
            }
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    RegisterDataSet(type);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool RegisterAssemblyDataSets(Assembly asm, string nameSpace)
        {
            if (asm == null)
            {
                return false;
            }
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(_dataSetType) && (type.Namespace == nameSpace))
                    {
                        RegisterDataSet(type);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void RegisterDataSet(Type type)
        {
            if (type.IsSubclassOf(_dataSetType))
            {
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (info.PropertyType.IsSubclassOf(_dataTableType))
                    {
                        m_DataSetTypeTable[info.Name.ToUpper()] = type;
                    }
                }
            }
        }

        public static bool RegisterDBDefaultType(Type type)
        {
            if (
                !type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.DeclaredOnly).Any(info => isConnection(info.FieldType))) return false;
            m_DefaultType = type;
            m_DefaultConnectionString = null;
            return true;
        }

        public static bool RegisterDBDefaultType(Type type, string connectionString)
        {
            if (type == null)
            {
                return false;
            }
            if (!isConnection(type))
            {
                return false;
            }
            m_DefaultType = type;
            m_DefaultConnectionString = connectionString;
            return true;
        }

        public int Update(DataRow dataRow)
        {
            return Update(m_Transaction, new[] { dataRow });
        }

        public int Update(DataSet dataSet)
        {
            if (dataSet.Tables.Count < 1)
            {
                return -2;
            }
            return Update(dataSet.Tables[0]);
        }

        public int Update(DataTable dataTable)
        {
            return Update(m_Transaction, dataTable);
        }

        public int Update(DataRow[] dataRows)
        {
            return Update(m_Transaction, dataRows);
        }

        public int Update(DataSet dataSet, int tableIndex)
        {
            if ((dataSet.Tables.Count > tableIndex) && (tableIndex >= 0))
            {
                return Update(dataSet.Tables[tableIndex]);
            }
            return -1;
        }

        public int Update(DataSet dataSet, string tableName)
        {
            if (!dataSet.Tables.Contains(tableName))
            {
                return -2;
            }
            return Update(dataSet.Tables[tableName]);
        }

        public int Update(IDbTransaction transaction, DataRow dataRow)
        {
            return Update(transaction, new[] { dataRow });
        }

        public int Update(IDbTransaction transaction, DataRow[] dataRows)
        {
            int num2;
            if (dataRows == null)
            {
                return -3;
            }
            if (dataRows.Length < 1)
            {
                return 0;
            }
            if ((dataRows[0].Table == null) || (dataRows[0].Table.DataSet == null))
            {
                return -2;
            }
            string tableName = dataRows[0].Table.TableName;
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(transaction, tableName);
            try
            {
                installAdapter(transaction, dataAdapter);
                int result = ((DbDataAdapter)dataAdapter).Update(dataRows);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateLog(dataRows[0].GetType(), tableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateExceptionLog(dataRows[0].GetType(), tableName, exception, DateTime.Now - now);
                }
                HandleException(exception);
                num2 = -1;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return num2;
        }

        public int Update(IDbTransaction transaction, DataSet dataSet)
        {
            if (dataSet.Tables.Count < 1)
            {
                return -2;
            }
            return Update(transaction, dataSet.Tables[0]);
        }

        public int Update(IDbTransaction transaction, DataTable dataTable)
        {
            int num2;
            if ((dataTable == null) || (dataTable.DataSet == null))
            {
                return -2;
            }
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(transaction, dataTable.TableName);
            try
            {
                installAdapter(transaction, dataAdapter);
                int result = ((DbDataAdapter)dataAdapter).Update(dataTable);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateLog(dataTable.GetType(), dataTable.TableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateExceptionLog(dataTable.GetType(), dataTable.TableName, exception, DateTime.Now - now);
                }
                HandleException(exception);
                num2 = -1;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return num2;
        }

        public int Update(IDbTransaction transaction, DataSet dataSet, int tableIndex)
        {
            if ((dataSet.Tables.Count > tableIndex) && (tableIndex >= 0))
            {
                return Update(transaction, dataSet.Tables[tableIndex]);
            }
            return -1;
        }

        public int Update(IDbTransaction transaction, DataSet dataSet, string tableName)
        {
            if (!dataSet.Tables.Contains(tableName))
            {
                return -2;
            }
            return Update(transaction, dataSet.Tables[tableName]);
        }

        public static int UpdateOnce(DataRow dataRow)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataSet, 0);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataTable dataTable)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataRow[] dataRows)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataRow dataRow, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, int tableIndex)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataSet);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataTable dataTable, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataRow[] dataRows, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow dataRow)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataSet, 0);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow[] dataRows)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataTable dataTable)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataRow dataRow, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, int tableIndex, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataSet);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataTable dataTable, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataRow[] dataRows, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow dataRow, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.Update(transaction, dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataSet);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataTable dataTable, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow[] dataRows, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, int tableIndex, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(DataSet dataSet, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow dataRow, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataRow);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.Update(transaction, dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataSet);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataTable dataTable, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataTable);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataRow[] dataRows, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataRows);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataSet, tableIndex);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateOnce(IDbTransaction transaction, DataSet dataSet, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.Update(transaction, dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public int UpdateToTable(DataRow dataRow, string tableName)
        {
            return UpdateToTable(m_Transaction, new[] { dataRow }, tableName);
        }

        public int UpdateToTable(DataSet dataSet, string tableName)
        {
            return UpdateToTable(m_Transaction, dataSet, 0, tableName);
        }

        public int UpdateToTable(DataRow[] dataRows, string tableName)
        {
            return UpdateToTable(m_Transaction, dataRows, tableName);
        }

        public int UpdateToTable(DataTable dataTable, string tableName)
        {
            return UpdateToTable(m_Transaction, dataTable, tableName);
        }

        public int UpdateToTable(DataSet dataSet, int tableIndex, string tableName)
        {
            if ((dataSet.Tables.Count > tableIndex) && (tableIndex >= 0))
            {
                return UpdateToTable(m_Transaction, dataSet.Tables[tableIndex], tableName);
            }
            return -1;
        }

        public int UpdateToTable(DataSet dataSet, string orgTableName, string tableName)
        {
            int index = dataSet.Tables.IndexOf(orgTableName);
            if (index < 0)
            {
                return -1;
            }
            return UpdateToTable(m_Transaction, dataSet, index, tableName);
        }

        public int UpdateToTable(IDbTransaction transaction, DataRow dataRow, string tableName)
        {
            return UpdateToTable(transaction, new[] { dataRow }, tableName);
        }

        public int UpdateToTable(IDbTransaction transaction, DataRow[] dataRows, string tableName)
        {
            int num2;
            if (dataRows.Length < 1)
            {
                return 0;
            }
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(transaction, tableName);
            if (dataAdapter == null)
            {
                return -1;
            }
            try
            {
                installAdapter(transaction, dataAdapter);
                int result = ((DbDataAdapter)dataAdapter).Update(dataRows);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateLog(dataRows[0].GetType(), tableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateExceptionLog(dataRows[0].GetType(), tableName, exception, DateTime.Now - now);
                }
                HandleException(exception);
                num2 = -1;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return num2;
        }

        public int UpdateToTable(IDbTransaction transaction, DataSet dataSet, string tableName)
        {
            return UpdateToTable(transaction, dataSet, 0, tableName);
        }

        public int UpdateToTable(IDbTransaction transaction, DataTable dataTable, string tableName)
        {
            int num2;
            if (dataTable == null)
            {
                return -1;
            }
            DateTime now = DateTime.Now;
            IDbDataAdapter dataAdapter = GetDataAdapter(transaction, tableName);
            try
            {
                installAdapter(transaction, dataAdapter);
                int result = ((DbDataAdapter)dataAdapter).Update(dataTable);
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateLog(dataTable.GetType(), tableName, result, DateTime.Now - now);
                }
                num2 = result;
            }
            catch (Exception exception)
            {
                if (PlatformConfig.DBHandlerWriteLog)
                {
                    DBHandlerLogSink.WriteUpdateExceptionLog(dataTable.GetType(), tableName, exception, DateTime.Now - now);
                }
                HandleException(exception);
                num2 = -1;
            }
            finally
            {
                freeAdapter(dataAdapter);
            }
            return num2;
        }

        public int UpdateToTable(IDbTransaction transaction, DataSet dataSet, int tableIndex, string tableName)
        {
            if ((dataSet.Tables.Count > tableIndex) && (tableIndex >= 0))
            {
                return UpdateToTable(transaction, dataSet.Tables[tableIndex], tableName);
            }
            return -1;
        }

        public int UpdateToTable(IDbTransaction transaction, DataSet dataSet, string orgTableName, string tableName)
        {
            int index = dataSet.Tables.IndexOf(orgTableName);
            if (index < 0)
            {
                return -1;
            }
            return UpdateToTable(transaction, dataSet, index, tableName);
        }

        public static int UpdateToTableOnce(DataRow dataRow, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataSet, 0, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataRow[] dataRows, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataTable dataTable, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataRow dataRow, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, int tableIndex, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string orgTableName, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataTable dataTable, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow dataRow, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataSet, 0, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow[] dataRows, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataTable dataTable, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataRow[] dataRows, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataRow dataRow, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, int tableIndex, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string orgTableName, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataRow[] dataRows, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataTable dataTable, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow dataRow, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string orgTableName, string tableName)
        {
            DBHandler buffer = GetBuffer();
            int num = buffer.UpdateToTable(transaction, dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataTable dataTable, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow[] dataRows, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, int tableIndex, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(DataSet dataSet, string orgTableName, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow dataRow, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataRow, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string orgTableName, string tableName, Type type)
        {
            DBHandler buffer = GetBuffer(type);
            int num = buffer.UpdateToTable(transaction, dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataSet, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataTable dataTable, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataTable, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataRow[] dataRows, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataRows, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, int tableIndex, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataSet, tableIndex, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public static int UpdateToTableOnce(IDbTransaction transaction, DataSet dataSet, string orgTableName, string tableName, Type type, string connectionString)
        {
            DBHandler buffer = GetBuffer(type, connectionString);
            int num = buffer.UpdateToTable(transaction, dataSet, orgTableName, tableName);
            buffer.FreeBuffer();
            return num;
        }

        public IDbDataAdapter Adapter
        {
            get
            {
                return m_Adapter;
            }
        }

        public IDbCommand Command
        {
            get
            {
                return m_Command;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return m_Connection;
            }
        }

        public string ConnectionString
        {
            get
            {
                if (m_Connection == null)
                {
                    return null;
                }
                return m_Connection.ConnectionString;
            }
        }

        public ConnectionState ConnectonState
        {
            get
            {
                return m_Connection.State;
            }
        }

        public bool IsConnectionOpened
        {
            get
            {
                return (m_Connection.State == ConnectionState.Open);
            }
        }

        public IsolationLevel IsolationLevel
        {
            get
            {
                return m_Transaction.IsolationLevel;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return m_Transaction;
            }
        }


        public static IDbDataParameter BuilderDBParameter()
        {
            IDbDataParameter parameter = null;
            if (m_DefaultType != null)
            {
                if (m_DefaultType == typeof(SqlConnection))
                {
                    parameter = new SqlParameter();
                }
                else if (m_DefaultType == typeof(OracleConnection))
                {
                    parameter = new OracleParameter();
                }
            }
            return parameter;
        }
    }
}
