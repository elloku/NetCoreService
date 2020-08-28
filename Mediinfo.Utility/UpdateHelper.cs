using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

namespace Mediinfo.Utility
{
    public static class UpdateHelper
    {
        /// <summary>
        /// ftp用户名
        /// </summary>
        public static string FtpUser { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public static string FtpPwd { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public static string FtpIp { get; set; }

        /// <summary>
        /// 更新程序名称
        /// </summary>
        public static string UpdateExeName { get; set; }

        /// <summary>
        /// 一级节点文件夹
        /// </summary>
        public static string FtpFirstSubDirectoryName { get; set; }
        private static string _loginFormName = "Mediinfo.WinForm.HIS.Main";
        /// <summary>
        /// 登录窗口名称
        /// </summary>
        public static string LoginFormName
        {
            get => _loginFormName;
            set => _loginFormName = value;
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        public static bool InitialUserCustomInfo(UpdateConfigInfo updateftpconfigInfo)
        {
            FtpUser = updateftpconfigInfo.FtpUser;
            FtpPwd = updateftpconfigInfo.FtpPwd;
            FtpIp = updateftpconfigInfo.FtpIp;
            UpdateExeName = updateftpconfigInfo.UpdateExeName;
            if (!string.IsNullOrWhiteSpace(updateftpconfigInfo.LoginFormName))
                _loginFormName = updateftpconfigInfo.LoginFormName;
            FtpFirstSubDirectoryName = updateftpconfigInfo.FtpFirstSubDirectoryName;
            if (!string.IsNullOrWhiteSpace(FtpUser) && !string.IsNullOrWhiteSpace(FtpPwd) && !string.IsNullOrWhiteSpace(FtpIp) && !string.IsNullOrWhiteSpace(UpdateExeName) && !string.IsNullOrWhiteSpace(LoginFormName) && !string.IsNullOrWhiteSpace(FtpFirstSubDirectoryName))
                return true;
            return false;
        }

        /// <summary>
        /// HTTP 下载初始化用户信息
        /// </summary>
        /// <returns></returns>
        public static void InitialUserCustomInfo()
        {
            UpdateExeName = "Mediinfo.WinForm.HIS.Update";
            LoginFormName = "Mediinfo.WinForm.HIS.Main";
        }

        /// <summary>
        /// ftp下载文件
        /// </summary>
        /// <param name="ftpFolder"></param>
        /// <param name="ftpFileName"></param>
        /// <param name="localDir"></param>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        public static bool GetFileNoBinary(string ftpFolder, string ftpFileName, string localDir, string localFileName)
        {
            try
            {
                string URI = "ftp://" + FtpIp + "\\" + ftpFolder + "\\" + ftpFileName;//所下载的文件在服务器所在的位置
                URI = URI.Replace("\\", "//");
                string localfile = localDir + @"\" + localFileName;//本地目录
                try
                {
                    if (File.Exists(localDir + "\\" + ftpFileName))
                    {
                        File.Delete(localDir + "\\" + ftpFileName);
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(localfile))
                    {
                        File.Delete(localfile);
                    }
                    LocalLog.WriteLog("请检查服务端" + ftpFolder + "文件夹下是否包含" + ftpFileName + "", ex);
                    return false;
                }
                System.Net.FtpWebRequest ftpwr = GetRequest(URI, FtpUser, FtpPwd);

                ftpwr.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;

                ftpwr.UseBinary = true;
                // 摘要:
                //     获取或设置一个 System.Boolean 值，该值指定文件传输的数据类型。
                //
                // 返回结果:
                //     true，指示服务器要传输的是二进制数据；false，指示数据为文本。默认值为 true。

                ftpwr.UsePassive = false;
                // 摘要:
                //     获取或设置客户端应用程序的数据传输过程的行为。
                //
                // 返回结果:
                // 如果客户端应用程序的数据传输过程侦听数据端口上的连接，则为 false；
                //如果客户端应在数据端口上启动连接，则为 true。默认值为 true。

                try
                {
                    using FtpWebResponse response = (FtpWebResponse)ftpwr.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(localfile, FileMode.CreateNew))
                        {
                            try
                            {
                                byte[] buffer = new byte[2048];

                                int read = 0;

                                do
                                {
                                    if (responseStream != null) read = responseStream.Read(buffer, 0, buffer.Length);
                                    fs.Write(buffer, 0, read);
                                } while (read != 0);

                                responseStream.Close();
                                fs.Flush();
                                fs.Close();
                            }
                            catch (Exception e)
                            {
                                fs.Close();

                                if (File.Exists(localfile))
                                {
                                    File.Delete(localfile);
                                }

                                LocalLog.WriteLog("请检查服务端" + ftpFolder + "文件夹下是否包含" + ftpFileName + "", e);
                                return false;
                            }
                        }
                        responseStream.Close();
                    }
                    response.Close();
                }
                catch (Exception ex)
                {
                    LocalLog.WriteLog(localfile + "", ex);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog("请检查服务端" + ftpFolder + "文件夹下是否包含" + ftpFileName + "", ex);
                return false;
            }
        }

        private static FtpWebRequest GetRequest(string URI, string username, string password)
        {
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            result.Credentials = new System.Net.NetworkCredential(username, password);
            result.KeepAlive = false;
            return result;
        }
    }

    /// <summary>
    /// 更新文件夹
    /// </summary>
    public static class UpdateDirectory
    {
        /// <summary>
        /// 获取项目文件夹
        /// </summary>
        /// <param name="startRootPath"></param>
        /// <param name="programDirectories"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static List<UpdateDirectories> GetUpdateDirectories(string startRootPath, string[] programDirectories, out string errorMessage)
        {
            errorMessage = string.Empty;
            OperateIniFile updateOperateIniFile = new OperateIniFile(startRootPath + "\\update.ini");

            string unupadteDirectories = updateOperateIniFile.ReadString("FTPINFO", "UnUpdateCatelog", "");

            List<UpdateDirectories> directorylist = new List<UpdateDirectories>();

            //比较版本配置文件和temp配置文件版本号是否一致，如果一致不作处理，否则获取temp文件节点信息加载到该窗体

            foreach (var subDirectory in programDirectories)
            {
                if (!Directory.Exists(startRootPath + "\\" + subDirectory))
                {
                    Directory.CreateDirectory(startRootPath + "\\" + subDirectory);

                    if (!string.IsNullOrWhiteSpace(unupadteDirectories))
                    {
                        if (!unupadteDirectories.Contains(subDirectory))
                        {
                            directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                        }

                        Directory.CreateDirectory(startRootPath + "\\" + subDirectory + "\\Temp");
                        try
                        {
                            UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                        }
                        catch (WebException exp)
                        {
                            errorMessage = "更新程序" + exp.Message;
                        }
                    }
                    else
                    {
                        directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                        Directory.CreateDirectory(startRootPath + "\\" + subDirectory + "\\Temp");
                        try
                        {
                            UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                        }
                        catch (WebException exp)
                        {
                            errorMessage = "更新程序" + exp.Message;
                        }
                    }
                }
                else
                {
                    //获取当前文件夹下的ini文件判断版本号
                    if (!File.Exists(startRootPath + "\\" + subDirectory + "\\version.ini"))
                    {
                        if (!string.IsNullOrWhiteSpace(unupadteDirectories))
                        {
                            if (!unupadteDirectories.Contains(subDirectory))
                            {
                                //服务端获取
                                directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                            }

                            Directory.CreateDirectory(startRootPath + "\\" + subDirectory + "\\Temp");
                            try
                            {
                                UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                            }
                            catch (WebException exp)
                            {
                                errorMessage = "更新程序" + exp.Message;
                            }
                        }
                        else
                        {
                            //服务端获取
                            directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                            Directory.CreateDirectory(startRootPath + "\\" + subDirectory + "\\Temp");
                            try
                            {
                                UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                            }
                            catch (WebException exp)
                            {
                                errorMessage = "更新程序" + exp.Message;
                            }
                        }
                    }
                    else
                    {
                        //服务端获取Version.ini文件放在temp文件下
                        if (!Directory.Exists(startRootPath + "\\" + subDirectory + "\\Temp"))
                        {
                            Directory.CreateDirectory(startRootPath + "\\" + subDirectory + "\\Temp");
                            try
                            {
                                UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                            }
                            catch (WebException exp)
                            {
                                errorMessage = "更新程序" + exp.Message;
                            }
                        }
                        else//存在
                        {
                            //服务端获取Version.ini文件放在temp文件下
                            if (!Directory.Exists(startRootPath + "\\" + "Temp"))
                            {
                                Directory.CreateDirectory(startRootPath + "\\" + "Temp");
                                try
                                {
                                    UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini");
                                }
                                catch (WebException exp)
                                {
                                    errorMessage = "更新程序" + exp.Message;
                                }
                            }
                            else//存在
                            {
                                if (File.Exists(startRootPath + "\\" + subDirectory + "\\Temp\\" + "version.ini"))
                                {
                                    string fileName = startRootPath + "\\" + subDirectory + "\\Temp\\" + "version.ini";//要检查被那个进程占用的文件

                                    try
                                    {
                                        File.Delete(startRootPath + "\\" + subDirectory + "\\Temp\\" + "version.ini");

                                    }
                                    catch (Exception)
                                    {
                                        errorMessage = fileName + "\r\n正在被其他应用使用,请退出程序重试!";
                                        return null;
                                    }

                                }

                                try
                                {
                                    if (UpdateHelper.GetFileNoBinary(subDirectory, "version.ini", startRootPath + "\\" + subDirectory + "\\Temp", "version.ini"))
                                    {
                                    }
                                }
                                catch (WebException exp)
                                {
                                    errorMessage = "更新程序" + exp.Message;
                                }
                            }
                        }

                        //比较本地和temp文件夹下版本号是否一致

                        OperateIniFile tempoperateIniFile = new OperateIniFile(startRootPath + "\\" + subDirectory + "\\Temp\\" + "version.ini");

                        OperateIniFile operateIniFile = new OperateIniFile(startRootPath + "\\" + subDirectory + "\\" + "version.ini");
                        string tempVersionNo = tempoperateIniFile.ReadString("version", "version", string.Empty);
                        string versionNo = operateIniFile.ReadString("version", "version", string.Empty);

                        if (!tempVersionNo.Equals(versionNo) && !string.IsNullOrWhiteSpace(tempVersionNo))
                        {
                            if (!string.IsNullOrWhiteSpace(unupadteDirectories))
                            {
                                if (!unupadteDirectories.Contains(subDirectory) || (unupadteDirectories.Contains(subDirectory) && (!tempVersionNo.Equals(versionNo))))
                                {
                                    directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                                }
                            }
                            else
                            {
                                directorylist.Add(new UpdateDirectories { DirectoryName = subDirectory });
                            }
                        }
                    }
                }
            }
            return directorylist;
        }
    }



    /// <summary>
    /// 待更新文件夹类
    /// </summary>
    public class UpdateDirectories
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string DirectoryName { get; set; }
    }

    public static class ZipCommon
    {
        #region 解压文件 .zip文件

        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)
        /// </summary>
        /// <param name = "FileToUpZip" > 待解压的文件 </ param >
        /// < param name="ZipedFolder">指定解压目标目录</param>
        /// < param name="isUpdatedStarted">指定解压目标目录</param>
        public static void UnZip(string FileToUpZip, string ZipedFolder, ref bool isUpdatedStarted)
        {
            if (!File.Exists(FileToUpZip))
            {
                return;
            }

            if (!Directory.Exists(ZipedFolder))
            {
                Directory.CreateDirectory(ZipedFolder);
            }

            ZipInputStream s = null;

            FileStream streamWriter = null;
            try
            {
                s = new ZipInputStream(File.OpenRead(FileToUpZip));
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.Name != string.Empty)
                    {
                        var fileName = Path.Combine(ZipedFolder, theEntry.Name);
                        //判断文件路径是否是文件夹
                        if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        List<string> updateFileNameArray = new List<string>();
                        Process myproc = new Process();
                        foreach (Process thisproc in Process.GetProcessesByName(UpdateHelper.UpdateExeName))
                            if (thisproc.MainModule != null)
                                updateFileNameArray.Add(thisproc.MainModule.FileName);

                        if (!updateFileNameArray.Contains(AppDomain.CurrentDomain.BaseDirectory + UpdateHelper.UpdateExeName + ".exe"))
                        {
                            streamWriter = File.Create(fileName);
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                var size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isUpdatedStarted = true;
                        }

                    }
                }
            }
            finally
            {
                streamWriter?.Close();
                s?.Close();
                GC.Collect();
                GC.Collect(1);
            }
        }

        #endregion 解压文件 .zip文件
    }

    /// <summary>
    /// 更新文件类
    /// </summary>
    public struct UpdateConfigInfo
    {
        public string FtpUser;
        public string FtpPwd;
        public string FtpIp;
        public string UpdateExeName;
        public string LoginFormName;
        public string FtpFirstSubDirectoryName;
    }
}