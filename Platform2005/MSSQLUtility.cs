namespace Platform
{
    using Platform.ExceptionHandling;
    using Platform.IO;
    using Platform.Tracing;
    using Platform.Utils;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.ServiceProcess;

    public class MSSQLUtility
    {
        public static bool CheckLocalSqlServerDataBase(string connectStringBase, string dbname)
        {
            bool flag;
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            try
            {
                connection.ConnectionString = connectStringBase.Replace("%DBNAME%", "Master");
                connection.Open();
                if (connection.State != ConnectionState.Open)
                {
                    throw new Exception("不能打开SQL Master数据库");
                }
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM sysdatabases WHERE name='" + dbname + "'";
                int num = (int)command.ExecuteScalar();
                if (num < 1)
                {
                    return false;
                }
                connection.Close();
                connection.ConnectionString = connectStringBase.Replace("%DBNAME%", dbname);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    return true;
                }
                flag = false;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                flag = false;
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }

        public static bool CheckLocalSqlServerInstance()
        {
            return CheckLocalSqlServerInstance(false);
        }

        public static bool CheckLocalSqlServerInstance(bool setRuning)
        {
            ServiceController windowsService = ServiceUtility.GetWindowsService("MSSQLSERVER");
            if (windowsService == null)
            {
                throw new Exception("在本地机器上找不到 Microsoft SQL SERVER ！请安装MS SQl SERVER 2000 或 MSDE 2000！");
            }
            if (setRuning)
            {
                return ServiceUtility.SetWindowsServiceStatus(windowsService, ServiceControllerStatus.Running);
            }
            return true;
        }

        public static bool SqlServerDataBaseAttachFile(string connectStringBase, string dbname, string filename)
        {
            return SqlServerDataBaseAttachFile(connectStringBase, dbname, filename, false);
        }

        public static bool SqlServerDataBaseAttachFile(string connectStringBase, string dbname, string filename, bool singleFile)
        {
            bool flag;
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            try
            {
                connection.ConnectionString = connectStringBase.Replace("%DBNAME%", "Master");
                connection.Open();
                if (connection.State != ConnectionState.Open)
                {
                    throw new Exception("不能打开SQL Master数据库");
                }
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM sysdatabases WHERE name='" + dbname + "'";
                int num = (int)command.ExecuteScalar();
                if (num < 1)
                {
                    if (!File.Exists(filename))
                    {
                        throw new Exception("找不到数据库文件：" + filename);
                    }
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                    string directoryName = Path.GetDirectoryName(filename);
                    string fullPath = Path.GetFullPath(directoryName + @"\" + fileNameWithoutExtension + "_log.LDF");
                    string text4 = Path.GetFullPath(directoryName + @"\" + dbname + "_log.LDF");
                    PathUtility.RemoveFile(fullPath);
                    PathUtility.RemoveFile(text4);
                    command.CommandText = (singleFile ? "sp_attach_single_file_db" : "sp_attach_db") + " '" + dbname + "' , '" + filename + "'";
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
                    command.ExecuteNonQuery();
                    if (((int)command.Parameters["ReturnValue"].Value) != 0)
                    {
                        return false;
                    }
                }
                connection.Close();
                connection.ConnectionString = connectStringBase.Replace("%DBNAME%", dbname);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    return true;
                }
                TraceHelper.WriteLine("打开数据库 " + dbname + " 失败！");
                flag = false;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                flag = false;
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }

        public static bool SqlServerDataBaseDetachFile(string connectStringBase, string dbname)
        {
            return SqlServerDataBaseDetachFile(connectStringBase, dbname, false);
        }

        public static bool SqlServerDataBaseDetachFile(string connectStringBase, string dbname, bool skipChecks)
        {
            bool flag;
            SqlConnection connection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            try
            {
                connection.ConnectionString = connectStringBase.Replace("%DBNAME%", "Master");
                connection.Open();
                if (connection.State != ConnectionState.Open)
                {
                    return false;
                }
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM sysdatabases WHERE name='" + dbname + "'";
                int num = (int)command.ExecuteScalar();
                if (num < 1)
                {
                    return true;
                }
                command.CommandText = "sp_detach_db '" + dbname + "' , " + (skipChecks ? "true" : "false");
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
                command.ExecuteNonQuery();
                if (((int)command.Parameters["ReturnValue"].Value) != 0)
                {
                    return false;
                }
                command.CommandText = "SELECT COUNT(*) FROM sysdatabases WHERE name='" + dbname + "'";
                num = (int)command.ExecuteScalar();
                if (num < 1)
                {
                    return true;
                }
                flag = false;
            }
            catch (Exception exception)
            {
                ExceptionHelper.HandleException(exception);
                flag = false;
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }
    }
}
