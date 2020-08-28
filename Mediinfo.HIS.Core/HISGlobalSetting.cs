using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 特别注意：请不要随便更改TypeName,ElementName等属性，包括大小写！！！！
    /// 变种单例模式，因为xml序列化必须要求public的序列化方法，暂时不考虑线程安全
    /// </summary>
    [XmlType(TypeName = "HISGlobalSetting")]
    public class HISGlobalSetting : HISSetting
    {
        private static HISGlobalSetting _instance = null;

        public static readonly string LoginDBUser = "数据库登录用户";
        public static readonly string MainDBUser = "数据库主用户";
        public static readonly string MainFTP = "主FTP";
        public static readonly string BakFTP = "备FTP";
        public static readonly string MainMQ = "MQ";
        public static readonly string Key = "MediHis5";

        public static string XiTongID = string.Empty;
        public static string UpdateDirectorys = string.Empty;

        /// <summary>
        /// 是否是HTTP更新
        /// </summary>
        public static bool IsHttp { get; set; } = false;

        public static string zxt;

        /// <summary>
        /// 构造函数中需要进行初始化成员变量
        /// </summary>
        public HISGlobalSetting() : base()
        {
            FTPINFO = new FTPUpdateConfig();
        }
        public static List<HTTPUpdateConfig> LoadHttpInfos()
        {
            List<HTTPUpdateConfig> https = new List<HTTPUpdateConfig>();
            if (!string.IsNullOrEmpty(UpdateDirectorys))
            {
                _instance = null;
                HTTPUpdateConfig hTTPUpdateConfig = Load(UpdateDirectorys.ToLower()).HTTPINFO;
                https.Add(hTTPUpdateConfig);
            }
            return https;
        }
        public static List<HTTPUpdateConfig> LoadZxtInfos(string path)
        {
            List<HTTPUpdateConfig> https = new List<HTTPUpdateConfig>();
            if (!string.IsNullOrEmpty(zxt))
            {
                _instance = null;
                _instance = Load<HISGlobalSetting>(path);
                https.Add(_instance.HTTPINFO);
            }
            return https;
        }
        /// <summary>
        /// 加载HISGlobalSettingHttp.xml文件
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        public static HISGlobalSetting Load(string directories)
        {
            DirectoryInfo rootPathInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);//根目录
            string destinationSubdir = rootPathInfo.FullName + "CONFIG\\" + "HISGlobalSettingHttp.xml";
            _instance = Load<HISGlobalSetting>(destinationSubdir);
            _instance.HTTPINFO.localConfigPath = rootPathInfo.FullName + "CONFIG\\";
            return _instance;
        }
        /// <summary>
        /// 加载默认的全局配置文件（HISGlobalSetting.xml）
        /// </summary>
        /// <returns></returns>
        public static HISGlobalSetting Load()
        {

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (_instance == null)
            {
                _instance = Load<HISGlobalSetting>(AppDomain.CurrentDomain.BaseDirectory + "HISGlobalSetting.xml");
            }
            return _instance;
        }
        /// <summary>
        /// 客户端更新FTP配置信息
        /// </summary>
        [XmlElement(ElementName = "客户端更新FTP配置信息")]
        public FTPUpdateConfig FTPINFO { get; set; }
        [XmlElement(ElementName = "客户端更新HTTP配置信息")]
        public HTTPUpdateConfig HTTPINFO { get; set; }

    }
    /// <summary>
    /// HTTP配置
    /// </summary>
    public class HTTPUpdateConfig
    {
        public HTTPUpdateConfig(string banbenhao, string jixianmc, string localPath)
        {
            JIXIANMC = jixianmc;
            BanBenHao = banbenhao;
            localConfigPath = localPath;
        }
        public HTTPUpdateConfig() { }
        /// <summary>
        /// 基线名称
        /// </summary>
        public string JIXIANMC { get; set; }
        /// <summary>
        /// 下载版本号 对应服务器包ID
        /// </summary>
        public string BanBenHao { get; set; }
        /// <summary>
        /// HISGlobalSetting.xml配置文件位置
        /// </summary>
        public string localConfigPath { get; set; }
    }
    public class FTPUpdateConfig
    {
        /// <summary>
        /// ftp用户名
        /// </summary>
        public string FtpUser { get; set; }

        /// <summary>
        /// ftp密码
        /// </summary>
        public string FtpPwd { get; set; }

        /// <summary>
        /// 备用ftp地址
        /// </summary>
        public string FtpSpareIp { get; set; }

        /// <summary>
        /// ftp地址
        /// </summary>
        public string FtpIp { get; set; }

        /// <summary>
        /// 更新程序名称
        /// </summary>
        public string UpdateExeName { get; set; }

        /// <summary>
        /// 登录用户窗口
        /// </summary>
        public string LoginFormName { get; set; }

        /// <summary>
        /// ftp根文件夹
        /// </summary>
        //public string FtpRootDirectoryName { get; set; }

        /// <summary>
        /// ftp子文件夹
        /// </summary>
        public string FtpFirstSubDirectoryName { get; set; }
    }
}
