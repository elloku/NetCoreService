using ICSharpCode.SharpZipLib.Zip;
using Mediinfo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Mediinfo.HIS.Core
{
    public class HTTPHelper
    {
        /// <summary>
        /// 下载压缩包
        /// </summary>
        /// <param name="url"></param>
        /// <param name="localFilePath"></param>
        /// <param name="baoId"></param>
        /// <param name="jiXianMC"></param>
        /// <returns></returns>
        public static bool DownloadZipFiles(string url, string localFilePath, string baoId, string jiXianMC)
        {
            try
            {
                using (HttpWebResponse web_resp = (HttpWebResponse)GetRequestZip(url, baoId, jiXianMC).GetResponse())
                {
                    using (Stream responseStream = web_resp.GetResponseStream())
                    {
                        System.IO.FileStream writeStream = null; // 写入本地文件流对象
                        if (File.Exists(Path.Combine(localFilePath, jiXianMC + ".zip")))
                        {
                            writeStream = File.OpenWrite(Path.Combine(localFilePath, jiXianMC + ".zip")); // 存在则打开要下载的文件
                        }
                        else
                        {
                            writeStream = new FileStream(Path.Combine(localFilePath, jiXianMC + ".zip"), FileMode.Create);// 文件不保存创建一个文件
                        }
                        byte[] read_bytes = new Byte[2048];
                        int count = 0;
                        while (true)
                        {
                            count = responseStream.Read(read_bytes, 0, read_bytes.Length);
                            if (count > 0)
                                writeStream.Write(read_bytes, 0, count);// 先写入本地临时文件
                            else
                                break;
                        }
                        writeStream.Close();
                        responseStream.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Date {0} ,Class: {1}, Error: {3}", DateTime.Now, "class:HTTPHelper",
                     ex.Message,
                    ex.InnerException));
                return false;
            }
        }
        /// <summary>
        /// 下载配置文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baoId">包ID 版本号</param>
        /// <param name="jiXianMC">基线名称</param>
        /// <returns></returns>
        public static string DownloadConfigFiles(string url, string baoId, string jiXianMC)
        {
            var ipList = NetworkHeler.GetAvailableNetwork().Find(o => o.Name == "以太网");
            if (ipList == null)
                ipList = NetworkHeler.GetAvailableNetwork().Find(o => o.Name == "WLAN");
            string ip = string.Format("{0}-{1}", ipList.Ip, ipList.Ip);
            try
            {
                using (HttpWebResponse web_resp = (HttpWebResponse)GetRequestConfigs(url, baoId, jiXianMC,ip).GetResponse())
                {
                    using (Stream responseStream = web_resp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                        {
                            byte[] read_bytes = new Byte[2048];
                            string s = reader.ReadToEnd();
                            return s;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Date {0} ,Class: {1}, Error: {3}", DateTime.Now, "class:HTTPHelper",
                     ex.Message,
                    ex.InnerException));
                return "";
            }
        }
        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLog(string value)
        {
            IsExistDirectory();
            string path = AppDomain.CurrentDomain.BaseDirectory + "AssemblyClient\\logs\\updateerror\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\errorLog.txt";
            FileStream f = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine(value);
            sw.Flush();
            sw.Close();
            f.Close();
        }
        /// <summary>
        /// 检查是否存在文件夹
        /// </summary>
        public static void IsExistDirectory()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "AssemblyClient\\logs\\updateerror\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\errorLog.txt";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "AssemblyClient\\logs\\updateerror\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "AssemblyClient\\logs\\updateerror\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(AppDomain.CurrentDomain.BaseDirectory + "AssemblyClient\\logs\\updateerror\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\errorLog.txt");
                fs.Close();
            }
        }
        private static HttpWebRequest GetRequestConfigs(string url, string baoId, string jiXianMC,string ip)
        {
            HttpWebRequest result = (System.Net.HttpWebRequest)WebRequest.Create(new Uri("http://" + url + "/DaiMaKu_ZXT/DownLoadConfigs?baoid=" + baoId + "&jiXianMC=" + jiXianMC + "&ipAdress=" + ip));
            return result;
        }
        private static HttpWebRequest GetRequestZip(string url, string baoId, string jiXianMC)
        {
            HttpWebRequest result = (HttpWebRequest)WebRequest.Create(new Uri("http://" + url + "/DaiMaKu_ZXT/DownLoad?baoid=" + baoId + "&jiXianMC=" + jiXianMC));
            return result;
        }
    }
    public class GlobalXmlHelper
    {
        public static void ModifyAttribute(string path, string elements, string s)
        {
            if (File.Exists(path))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(path);
                System.Xml.XmlElement element = (System.Xml.XmlElement)xmlDoc.SelectSingleNode("HISGlobalSetting/客户端更新HTTP配置信息/" + elements);
                element.InnerText = s;
                //element.SetAttribute("Value", s)
                xmlDoc.Save(path);
            }
        }
    }

    public class FileHelper
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirName"></param>
        public static void CreateDir(string dirName)
        {
            string path = dirName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// 获取程序集目录
        /// </summary>
        /// <returns></returns>
        public static string GetBasePath()
        {
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            return dirPath;
        }
        /// <summary>
        /// 解压zip文件到指定目录
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="uzipDir">解压后的目录</param>
        public static void DeZip(string filePath, string uzipDir)
        {
            CreateDir(uzipDir);
            ZipInputStream zipInputStream = new ZipInputStream(File.Open(filePath, FileMode.Open));
            ZipEntry zipEntryFromZippedFile = zipInputStream.GetNextEntry();
            while (zipEntryFromZippedFile != null)
            {
                if (zipEntryFromZippedFile.IsFile)
                {
                    FileInfo fInfo = new FileInfo(string.Format(uzipDir + "/{0}", zipEntryFromZippedFile.Name));
                    if (!fInfo.Directory.Exists) fInfo.Directory.Create();

                    FileStream file = fInfo.Create();
                    int count = 0;
                    byte[] bufferFromZip = new byte[2048];
                    while (true)
                    {
                        count = zipInputStream.Read(bufferFromZip, 0, bufferFromZip.Length);
                        if (count > 0)
                            file.Write(bufferFromZip, 0, count);
                        else
                            break;

                    }
                    file.Close();
                }
                zipEntryFromZippedFile = zipInputStream.GetNextEntry();
            }
            zipInputStream.Close();
        }
        public static void DelectFile(string srcPath)
        {
            try
            {
                if (File.Exists(srcPath))
                {
                    File.Delete(srcPath);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
