namespace Platform.Net
{
    using Platform.Compress;
    using Platform.IO;
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Web;

    public sealed class HttpUtility
    {
        public static Platform.IO.MemoryStream GetFile(string fileUrl, string userName, string password)
        {
            return GetFile(fileUrl, userName, password, (HttpGetFileEventHandler) null, null);
        }

        public static Platform.IO.MemoryStream GetFile(string fileUrl, string userName, string password, HttpGetFileEventHandler handler)
        {
            return GetFile(fileUrl, userName, password, handler, null);
        }

        public static bool GetFile(string fileName, string fileUrl, string userName, string password)
        {
            return GetFile(fileName, fileUrl, userName, password, null, null);
        }

        public static Platform.IO.MemoryStream GetFile(string fileUrl, string userName, string password, HttpGetFileEventHandler handler, object state)
        {
            HttpInformation information = new HttpInformation();
            information.State = state;
            WebRequest request = WebRequest.Create(System.Web.HttpUtility.UrlPathEncode(fileUrl));
            if ((userName != null) && (password != null))
            {
                request.PreAuthenticate = true;
                NetworkCredential credential = new NetworkCredential(userName, password);
                request.Credentials = credential;
            }
            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (handler != null)
                {
                    information.MessageType = 0;
                    try
                    {
                        information.FileName = "";
                    }
                    catch
                    {
                    }
                    information.Length = (int) response.ContentLength;
                    handler(information);
                }
                Platform.IO.MemoryStream stream2 = new Platform.IO.MemoryStream();
                int bufferLength = stream2.BufferLength;
                int bufferIndex = stream2.BufferIndex;
                int bufferOffset = stream2.BufferOffset;
                ArrayList buffers = stream2.Buffers;
                byte[] buffer = (byte[]) buffers[bufferIndex];
                int num4 = 0;
                int num5 = 0;
                do
                {
                    if (bufferOffset >= bufferLength)
                    {
                        bufferOffset -= bufferLength;
                        bufferIndex++;
                        if (bufferIndex >= buffers.Count)
                        {
                            buffer = new byte[bufferLength];
                            buffers.Add(buffer);
                        }
                        else
                        {
                            buffer = (byte[]) buffers[bufferIndex];
                        }
                    }
                    num5 = responseStream.Read(buffer, bufferOffset, bufferLength - bufferOffset);
                    num4 += num5;
                    bufferOffset += num5;
                    if (handler != null)
                    {
                        information.MessageType = 1;
                        information.ReadLength = num5;
                        information.ReadTotal = num4;
                        handler(information);
                    }
                }
                while (num5 > 0);
                if (handler != null)
                {
                    information.MessageType = 2;
                    information.ReadTotal = num4;
                    handler(information);
                }
                stream2.SetLength(stream2.Position + num4);
                return stream2;
            }
            catch (Exception exception)
            {
                if (handler != null)
                {
                    information.MessageType = -1;
                    information.Message = exception.Message;
                    handler(information);
                }
                return null;
            }
        }

        public static bool GetFile(string fileName, string fileUrl, string userName, string password, HttpGetFileEventHandler handler)
        {
            return GetFile(fileName, fileUrl, userName, password, handler, null);
        }

        public static bool GetFile(string fileName, string fileUrl, string userName, string password, HttpGetFileEventHandler handler, object state)
        {
            HttpInformation information = new HttpInformation();
            information.State = state;
            WebRequest request = WebRequest.Create(System.Web.HttpUtility.UrlPathEncode(fileUrl));
            if ((userName != null) && (password != null))
            {
                request.PreAuthenticate = true;
                NetworkCredential credential = new NetworkCredential(userName, password);
                request.Credentials = credential;
            }
            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (handler != null)
                {
                    information.MessageType = 0;
                    try
                    {
                        information.FileName = Path.GetFileName(fileName);
                    }
                    catch
                    {
                    }
                    information.Length = (int) response.ContentLength;
                    handler(information);
                }
                using (FileStream stream2 = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    stream2.SetLength((long) 0);
                    byte[] buffer = new byte[0x400];
                    int num = 0;
                    int count = 0;
                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream2.Write(buffer, 0, count);
                        num += count;
                        if (handler != null)
                        {
                            information.MessageType = 1;
                            information.ReadLength = count;
                            information.ReadTotal = num;
                            handler(information);
                        }
                    }
                    stream2.Flush();
                    if (handler != null)
                    {
                        information.MessageType = 2;
                        information.ReadTotal = num;
                        handler(information);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                if (handler != null)
                {
                    information.MessageType = -1;
                    information.Message = exception.Message;
                    handler(information);
                }
                return false;
            }
        }

        public static bool GetFile(string fileName, string fileUrl, string userName, string password, bool gzipData, HttpGetFileEventHandler handler, object state)
        {
            bool flag;
            HttpInformation information = new HttpInformation();
            information.State = state;
            WebRequest request = WebRequest.Create(System.Web.HttpUtility.UrlPathEncode(fileUrl));
            if ((userName != null) && (password != null))
            {
                request.PreAuthenticate = true;
                NetworkCredential credential = new NetworkCredential(userName, password);
                request.Credentials = credential;
            }
            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (handler != null)
                {
                    information.MessageType = 0;
                    try
                    {
                        information.FileName = Path.GetFileName(fileName);
                    }
                    catch
                    {
                    }
                    information.Length = (int) response.ContentLength;
                    handler(information);
                }
                if (gzipData)
                {
                    System.IO.MemoryStream stream2 = new System.IO.MemoryStream();
                    byte[] buffer = new byte[0x400];
                    int num = 0;
                    int count = 0;
                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream2.Write(buffer, 0, count);
                        num += count;
                        if (handler != null)
                        {
                            information.MessageType = 1;
                            information.ReadLength = count;
                            information.ReadTotal = num;
                            handler(information);
                        }
                    }
                    stream2.Flush();
                    if (handler != null)
                    {
                        information.MessageType = 2;
                        information.ReadTotal = num;
                        handler(information);
                    }
                    byte[] input = stream2.ToArray();
                    byte[] buffer3 = GZipUtility.GUnzip(input, 0, input.Length);
                    if ((buffer3 == null) && (handler != null))
                    {
                        information.MessageType = -1;
                        information.Message = "½âÑ¹Êý¾Ý´íÎó£¡";
                        handler(information);
                    }
                    using (FileStream stream3 = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        stream3.SetLength((long) 0);
                        stream3.Write(buffer3, 0, buffer3.Length);
                        stream3.Flush();
                        goto Label_021F;
                    }
                }
                using (FileStream stream4 = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    stream4.SetLength((long) 0);
                    byte[] buffer4 = new byte[0x400];
                    int num3 = 0;
                    int num4 = 0;
                    while ((num4 = responseStream.Read(buffer4, 0, buffer4.Length)) > 0)
                    {
                        stream4.Write(buffer4, 0, num4);
                        num3 += num4;
                        if (handler != null)
                        {
                            information.MessageType = 1;
                            information.ReadLength = num4;
                            information.ReadTotal = num3;
                            handler(information);
                        }
                    }
                    stream4.Flush();
                    if (handler != null)
                    {
                        information.MessageType = 2;
                        information.ReadTotal = num3;
                        handler(information);
                    }
                }
            Label_021F:
                flag = true;
            }
            catch (Exception exception)
            {
                if (handler != null)
                {
                    information.MessageType = -1;
                    information.Message = exception.Message;
                    handler(information);
                }
                flag = false;
            }
            return flag;
        }
    }
}
