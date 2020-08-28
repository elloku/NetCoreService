using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Mediinfo.Utility
{
    /// <summary>
    /// FTP工具类
    /// </summary>
    public class FTPHelper
    {
        /// <summary>  
        /// FTP请求对象  
        /// </summary>  
        FtpWebRequest request = null;

        /// <summary>  
        /// FTP响应对象  
        /// </summary>  
        FtpWebResponse response = null;

        /// <summary>  
        /// FTP服务器地址  
        /// </summary>  
        public string ServerUrl { get; private set; }

        /// <summary>  
        /// FTP服务器登录用户名  
        /// </summary>  
        public string UserID { get; private set; }

        /// <summary>  
        /// FTP服务器登录密码  
        /// </summary>  
        public string Password { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ftpServerUrl">FTP连接地址</param>
        /// <param name="ftpUserID">用户名</param>
        /// <param name="ftpPassword">密码</param>
        public FTPHelper(string ftpServerUrl, string ftpUserID, string ftpPassword)
        {
            this.ServerUrl = ftpServerUrl;
            this.UserID = ftpUserID;
            this.Password = ftpPassword;
        }

        public FTPHelper(string ftpServerUrl)
        {
            this.ServerUrl = ftpServerUrl;
            this.UserID = string.Empty;
            this.Password = string.Empty;
        }

        ~FTPHelper()
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
            if (request != null)
            {
                request.Abort();
                request = null;
            }
        }

        /// <summary>  
        /// 建立FTP链接,返回响应对象  
        /// </summary>  
        /// <param name="uri">FTP地址</param>  
        /// <param name="ftpMethod">操作命令</param>  
        /// <returns></returns>  
        private FtpWebResponse Open(Uri uri, string ftpMethod)
        {
            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;

            if (!string.IsNullOrWhiteSpace(this.UserID) && !string.IsNullOrWhiteSpace(this.Password))
                request.Credentials = new NetworkCredential(this.UserID, this.Password);

            //FtpWebRequest.Create()
            return (FtpWebResponse)request.GetResponse();
        }

        /// <summary>         
        /// 建立FTP链接,返回请求对象         
        /// </summary>        
        /// <param name="uri">FTP地址</param>         
        /// <param name="ftpMethod">操作命令</param>         
        private FtpWebRequest OpenRequest(Uri uri, string ftpMethod)
        {
            request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;

            if (!string.IsNullOrWhiteSpace(this.UserID) && !string.IsNullOrWhiteSpace(this.Password))
                request.Credentials = new NetworkCredential(this.UserID, this.Password);

            return request;
        }

        public bool IsConnected()
        {
            try
            {
                using (var resp = Open(new Uri(this.ServerUrl), "List"))
                {
                    ;
                }
                return true;

            }
            catch (WebException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>  
        /// 创建目录  
        /// </summary>  
        /// <param name="remoteDirectoryName">目录名</param> 
        public void CreateDirectory(string remoteDirectoryName)
        {
            response = Open(new Uri(this.ServerUrl + remoteDirectoryName), WebRequestMethods.Ftp.MakeDirectory);
            this.ServerUrl += remoteDirectoryName + "/";
        }

        public void NextDirectory(string remoteDirectoryName)
        {
            ServerUrl += remoteDirectoryName + "/";
        }

        public void PreviousDirectory()
        {
            if (ServerUrl.Substring(ServerUrl.Length - 1) == "/")
            {
                ServerUrl = ServerUrl.Substring(0, ServerUrl.Length - 1);
            }
            var index = ServerUrl.LastIndexOf("/");
            if (index > 0)
            {
                ServerUrl = ServerUrl.Substring(0, index + 1);
            }
        }

        /// <summary>  
        /// 更改目录或文件名  
        /// </summary>  
        /// <param name="currentName">当前名称</param>  
        /// <param name="newName">修改后新名称</param>  
        public void ReName(string currentName, string newName)
        {
            request = OpenRequest(new Uri(ServerUrl + currentName), WebRequestMethods.Ftp.Rename);
            request.RenameTo = newName;
            response = (FtpWebResponse)request.GetResponse();
        }

        /// <summary>  
        /// 删除目录(包括下面所有子目录和子文件)  
        /// </summary>  
        /// <param name="remoteDirectoryName">要删除的带路径目录名：如web/test</param>  
        /* 
         * 例：删除test目录 
         FTPHelper helper = new FTPHelper("x.x.x.x", "web", "user", "password");                   
         helper.RemoveDirectory("web/test"); 
         */
        public void RemoveDirectory(string remoteDirectoryName)
        {
            this.ServerUrl += remoteDirectoryName + "/";
            var listAll = ListFilesAndDirectories();
            foreach (var m in listAll)
            {
                if (m.Name.IndexOf(".", StringComparison.Ordinal) < 0)
                {
                    RemoveDirectory(m.Name);
                }
                else
                    DeleteFile(m.Name);
            }
            response = Open(new Uri(ServerUrl), WebRequestMethods.Ftp.RemoveDirectory);
            this.ServerUrl = this.ServerUrl.Replace(remoteDirectoryName + "/", string.Empty);
        }

        /// <summary>  
        /// 文件上传  
        /// </summary>  
        /// <param name="localFilePath">本地文件路径</param>  
        public void Upload(string localFilePath)
        {
            FileInfo fileInf = new FileInfo(localFilePath);
            request = OpenRequest(new Uri(ServerUrl + fileInf.Name), WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            using var fs = fileInf.OpenRead();
            using var strm = request.GetRequestStream();
            var contentLen = fs.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }
        }

        /// <summary>
        /// ftp测试连接是否成功  
        /// </summary>
        /// <returns></returns>
        public bool TestFtpConnection()
        {
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create(ServerUrl);
                ftprequest.Credentials = new NetworkCredential(UserID, Password);

                ftprequest.KeepAlive = false;
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftprequest.UseBinary = true;
                ftprequest.UsePassive = false;
                ftprequest.Timeout = 3000;
                ftprequest.ReadWriteTimeout = 3000;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();
                ftpResponse.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContext"></param>
        public void Upload(string fileName, FileStream fileContext)
        {
            request = OpenRequest(new Uri(ServerUrl + fileName), WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = fileContext.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            using var strm = request.GetRequestStream();
            var contentLen = fileContext.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = fileContext.Read(buff, 0, buffLength);
            }
        }

        /// <summary>    
        /// 删除文件    
        /// </summary>    
        /// <param name="remoteFileName">要删除的文件名</param>  
        public void DeleteFile(string remoteFileName)
        {
            response = Open(new Uri(ServerUrl + remoteFileName), WebRequestMethods.Ftp.DeleteFile);
        }

        /// <summary>  
        /// 获取当前目录的文件和一级子目录信息  
        /// </summary>  
        /// <returns></returns>  
        public List<FileStruct> ListFilesAndDirectories()
        {
            var fileList = new List<FileStruct>();
            response = Open(new Uri(ServerUrl), WebRequestMethods.Ftp.ListDirectoryDetails);
            using var stream = response.GetResponseStream();
            using var sr = new StreamReader(stream);
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                //line的格式如下：  
                //08-18-13  11:05PM       <DIR>          aspnet_client  
                //09-22-13  11:39PM                 2946 Default.aspx  
                //DateTime dtDate = DateTime.ParseExact(line.Substring(0, 8), "MM-dd-yy", null);
                //DateTime dtDateTime = DateTime.Parse(dtDate.ToString("yyyy-MM-dd") + line.Substring(8, 9));
                string[] arrs = line.Split(' ');
                if (arrs[^1].StartsWith("."))
                {
                    continue;
                }
                var model = new FileStruct()
                {
                    Name = arrs[^1]
                };
                fileList.Add(model);
            }

            return fileList;
        }

        /// <summary>         
        /// 列出当前目录的所有文件         
        /// </summary>         
        public List<FileStruct> ListFiles()
        {
            var listAll = ListFilesAndDirectories();
            var listFile = listAll.Where(m => m.Name.IndexOf(".", StringComparison.Ordinal) > -1).ToList();
            return listFile;
        }

        /// <summary>         
        /// 列出当前目录的所有一级子目录         
        /// </summary>         
        public List<FileStruct> ListDirectories()
        {
            var listAll = ListFilesAndDirectories();
            var listFile = listAll.Where(m => m.Name.IndexOf(".", StringComparison.Ordinal) < 0).ToList();
            return listFile;
        }

        /// <summary>         
        /// 判断当前目录下指定的子目录或文件是否存在         
        /// </summary>         
        /// <param name="remoteName">指定的目录或文件名</param>        
        public bool IsExist(string remoteName)
        {
            var list = ListFilesAndDirectories();
            if (list.Count(m => m.Name == remoteName) > 0)
                return true;
            return false;
        }

        /// <summary>         
        /// 判断当前目录下指定的一级子目录是否存在         
        /// </summary>         
        /// <param name="remoteDirectoryName">指定的目录名</param>        
        public bool IsDirectoryExist(string remoteDirectoryName)
        {
            var listDir = ListDirectories();
            if (listDir.Count(m => m.Name == remoteDirectoryName) > 0)
                return true;
            return false;
        }

        /// <summary>         
        /// 判断当前目录下指定的子文件是否存在        
        /// </summary>         
        /// <param name="remoteFileName">远程文件名</param>         
        public bool IsFileExist(string remoteFileName)
        {
            var listFile = ListFiles();
            if (listFile.Count(m => m.Name == remoteFileName) > 0)
                return true;
            return false;
        }

        /// <summary>  
        /// 下载  
        /// </summary>  
        /// <param name="saveFilePath">下载后的保存路径</param>  
        /// <param name="downloadFileName">要下载的文件名</param>  
        public void Download(string saveFilePath, string downloadFileName)
        {
            using FileStream outputStream = new FileStream(saveFilePath + "\\" + downloadFileName, FileMode.Create);
            response = Open(new Uri(ServerUrl + downloadFileName), WebRequestMethods.Ftp.DownloadFile);
            using Stream ftpStream = response.GetResponseStream();
            int bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            if (ftpStream != null)
            {
                var readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
            }
        }

        public KeyValuePair<string, byte[]> ComputeHash(string downloadFileName)
        {
            System.IO.MemoryStream stream = new MemoryStream();
            response = Open(new Uri(ServerUrl + downloadFileName), WebRequestMethods.Ftp.DownloadFile);
            using (Stream ftpStream = response.GetResponseStream())
            {
                long cl = response.ContentLength;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                if (ftpStream != null)
                {
                    var readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        stream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
            }
            stream.Position = 0;
            Byte[] byData = new Byte[stream.Length];
            stream.Read(byData, 0, (int)stream.Length);
            stream.Close();
            var md5 = System.Security.Cryptography.MD5.Create();
            var output = md5.ComputeHash(byData);
            return new KeyValuePair<string, byte[]>(BitConverter.ToString(output), byData);
        }

        #region 字段

        private string ftpURI;
        private string ftpUserID;
        private string ftpServerIP;
        private string ftpPassword;
        private string ftpRemotePath;

        #endregion 字段

        /// <summary>
        /// 连接FTP服务器
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        public FTPHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            ftpServerIP = FtpServerIP;
            ftpRemotePath = FtpRemotePath;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        }

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        public string[] GetFilesDetailList()
        {
            try
            {
                StringBuilder result = new StringBuilder();
                var ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return new string[0];

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n", StringComparison.Ordinal), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 全局FTP访问变量  
        /// </summary>  
        public FTPClient Ftp { get; set; }

        /// <summary>  
        /// 测试FTP服务器是否可登陆  
        /// </summary>  
        public bool CanConnect(string serverHost, string user, string password)
        {
            Ftp ??= this.getFtpClient(serverHost, user, password);
            try
            {
                Ftp.Connect();
                Ftp.DisConnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 得到FTP传输对象  
        /// </summary>
        /// <param name="serverHost"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public FTPClient getFtpClient(string serverHost, string user, string password)
        {
            FTPClient ft = new FTPClient {RemoteHost = serverHost, RemoteUser = user, RemotePass = password};
            return ft;
        }
    }

    /// <summary>  
    /// FTP 操作类客户端  
    /// </summary>  
    public class FTPClient
    {
        public static object obj = new object();

        #region 字段  

        private Boolean bConnected;
        private string strRemotePass;
        private string strRemoteUser;
        private string strRemotePath;

        /// <summary>  
        /// 服务器返回的应答信息(包含应答码)  
        /// </summary>  
        private string strMsg;
        /// <summary>  
        /// 服务器返回的应答信息(包含应答码)  
        /// </summary>  
        private string strReply;
        /// <summary>  
        /// 服务器返回的应答码  
        /// </summary>  
        private int iReplyCode;
        /// <summary>  
        /// 进行控制连接的socket  
        /// </summary>  
        private Socket socketControl;
        /// <summary>  
        /// 接收和发送数据的缓冲区  
        /// </summary>  
        private static int BLOCK_SIZE = 512;
        /// <summary>  
        /// 编码方式  
        /// </summary>  
        Encoding ASCII = Encoding.ASCII;
        /// <summary>  
        /// 字节数组  
        /// </summary>  
        Byte[] buffer = new Byte[BLOCK_SIZE];

        #endregion

        #region 属性  

        /// <summary>  
        /// FTP服务器IP地址  
        /// </summary>  
        public string RemoteHost { get; set; }

        /// <summary>  
        /// 登录用户账号  
        /// </summary>  
        public string RemoteUser
        {
            set => strRemoteUser = value;
        }

        /// <summary>  
        /// 用户登录密码  
        /// </summary>  
        public string RemotePass
        {
            set => strRemotePass = value;
        }

        #endregion

        #region 链接  

        /// <summary>  
        /// 建立连接   
        /// </summary>  
        public void Connect()
        {
            lock (obj)
            {
                socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(RemoteHost), 0);
                try
                {
                    socketControl.Connect(ep);
                }
                catch (Exception)
                {
                    throw new IOException("不能连接ftp服务器");
                }
            }
            ReadReply();
            if (iReplyCode != 220)
            {
                DisConnect();
                throw new IOException(strReply.Substring(4));
            }
            SendCommand("USER " + strRemoteUser);
            if (!(iReplyCode == 331 || iReplyCode == 230))
            {
                CloseSocketConnect();
                throw new IOException(strReply.Substring(4));
            }
            if (iReplyCode != 230)
            {
                SendCommand("PASS " + strRemotePass);
                if (!(iReplyCode == 230 || iReplyCode == 202))
                {
                    CloseSocketConnect();
                    throw new IOException(strReply.Substring(4));
                }
            }
            bConnected = true;
            ChDir(strRemotePath);
        }

        /// <summary>  
        /// 关闭连接  
        /// </summary>  
        public void DisConnect()
        {
            if (socketControl != null)
            {
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }

        #endregion

        #region 传输模式  

        /// <summary>  
        /// 传输模式:二进制类型、ASCII类型  
        /// </summary>  
        public enum TransferType { Binary, ASCII };

        #endregion

        /// <summary>  
        /// 改变目录  
        /// </summary>  
        /// <param name="strDirName">新的工作目录名</param>  
        public void ChDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
            this.strRemotePath = strDirName;
        }

        #region 内部函数  

        /// <summary>  
        /// 将一行应答字符串记录在strReply和strMsg,应答码记录在iReplyCode  
        /// </summary>  
        private void ReadReply()
        {
            strMsg = "";
            strReply = ReadLine();
            iReplyCode = Int32.Parse(strReply.Substring(0, 3));
        }

        /// <summary>  
        /// 关闭socket连接(用于登录以前)  
        /// </summary>  
        private void CloseSocketConnect()
        {
            lock (obj)
            {
                if (socketControl != null)
                {
                    socketControl.Close();
                    socketControl = null;
                }
                bConnected = false;
            }
        }

        /// <summary>  
        /// 读取Socket返回的所有字符串  
        /// </summary>  
        /// <returns>包含应答码的字符串行</returns>  
        private string ReadLine()
        {
            lock (obj)
            {
                while (true)
                {
                    int iBytes = socketControl.Receive(buffer, buffer.Length, 0);
                    strMsg += ASCII.GetString(buffer, 0, iBytes);
                    if (iBytes < buffer.Length)
                    {
                        break;
                    }
                }
            }
            char[] seperator = { '\n' };
            string[] mess = strMsg.Split(seperator);
            strMsg = strMsg.Length > 2 ? mess[^2] : mess[0];
            if (!strMsg.Substring(3, 1).Equals(" ")) //返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)  
            {
                return ReadLine();
            }
            return strMsg;
        }

        /// <summary>  
        /// 发送命令并获取应答码和最后一行应答字符串  
        /// </summary>  
        /// <param name="strCommand">命令</param>  
        public void SendCommand(string strCommand)
        {
            lock (obj)
            {
                Byte[] cmdBytes = Encoding.ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
                socketControl.Send(cmdBytes, cmdBytes.Length, 0);
                Thread.Sleep(500);
                ReadReply();
            }
        }

        #endregion
    }

    public class FileStruct
    {
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 文件或目录名称
        /// </summary>
        public string Name { get; set; }
    }
}
