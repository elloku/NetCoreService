using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 客户端配置文件辅助类（只供四层，五层使用，三层的Service不要调用）
    /// 特别注意：请不要随便更改TypeName,ElementName等属性，包括大小写！！！！
    /// 变种单例模式，因为xml序列化必须要求public的序列化方法，暂时不考虑线程安全
    /// </summary>
    [XmlType(TypeName = "HISClientSetting")]
    public class HISClientSetting : HISSetting
    {
        private static HISClientSetting _instance = null;

        /// <summary>
        /// 请不要直接从这种方式初始化
        /// </summary>
        public HISClientSetting() : base()
        {
            LastLoginInfo = new LoginInfo();
            Workstation = new Workstation();
        }

        /// <summary>
        /// 加载客户端配置文件（外部调用的唯一方式）
        /// </summary>
        /// <returns></returns>
        public static HISClientSetting Load()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (_instance == null)
                _instance = Load<HISClientSetting>(AppDomain.CurrentDomain.BaseDirectory + "HISClientSetting.xml");

            return _instance;
        }

        /// <summary>
        /// 上次登录信息
        /// </summary>
        [XmlElement(ElementName = "登录信息")]
        public LoginInfo LastLoginInfo { get; set; }

        /// <summary>
        /// 工作站信息
        /// </summary>
        [XmlElement(ElementName = "工作站信息")]
        public Workstation Workstation { get; set; }

        /// <summary>
        /// 用户配置
        /// </summary>
        [XmlArray(ElementName = "用户配置")]
        public List<UserConfig> UserConfigList { get; set; }
    }

    /// <summary>
    /// 工作站信息
    /// </summary>
    public class Workstation
    {
        /// <summary>
        /// 工作站ID
        /// </summary>
        public string GongZuoZhanID { get; set; }

        /// <summary>
        /// 工作站名称
        /// </summary>
        public string GongZuoZhanMC { get; set; }

        /// <summary>
        /// 位置ID
        /// </summary>
        public string WeiZhiID { get; set; }

        /// <summary>
        /// 位置名称
        /// </summary>
        public string WeiZhiMC { get; set; }
    }

    /// <summary>
    /// 上次登录的信息
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 职工ID
        /// </summary>
        public string ZhiGongID { get; set; }

        /// <summary>
        /// 职工工号
        /// </summary>
        public string ZhiGongGH { get; set; }

        /// <summary>
        /// 职工姓名
        /// </summary>
        public string ZhiGongXM { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string YingYongID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string YingYongMC { get; set; }
    }

    /// <summary>
    /// 用户配置
    /// </summary>
    public class UserConfig
    {

        /// <summary>
        /// 职工ID
        /// </summary>
        public string ZhiGongID { get; set; }

        /// <summary>
        /// 模糊查询
        /// </summary>
        public string MoHuCX { get; set; }

    }
}
