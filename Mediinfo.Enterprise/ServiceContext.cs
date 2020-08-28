using System.Collections.Generic;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 服务上下文（客户端调用时传递的相关信息）
    /// </summary>
    public class ServiceContext
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string YINGYONGID { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        public string XITONGID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string YINGYONGMC { get; set; }

        /// <summary>
        /// 应用简称
        /// </summary>
        public string YINGYONGJC { get; set; }

        /// <summary>
        /// 系统版本
        /// </summary>
        public string VERSION { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 网卡物理地址
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// 客户端计算机名
        /// </summary>
        public string COMPUTERNAME { get; set; }

        /// <summary>
        /// 用户名（客户端登陆用户）
        /// </summary>
        public string USERNAME { get; set; }

        /// <summary>
        /// 用户ID（客户端登陆用户）
        /// </summary>
        public string USERID { get; set; }

        /// <summary>
        /// 输入码类型
        /// </summary>
        public string SHURUMLX { get; set; }

        /// <summary>
        /// 科室ID
        /// </summary>
        public string KESHIID { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string KESHIMC { get; set; }

        /// <summary>
        /// 病区ID
        /// </summary>
        public string BINGQUID { get; set; }

        /// <summary>
        /// 病区ID
        /// </summary>
        public string DANGQIANBQ { get; set; }

        /// <summary>
        /// 病区名称
        /// </summary>
        public string DANGQIANBQMC { get; set; }

        /// <summary>
        /// 病区名称
        /// </summary>
        public string BINGQUMC { get; set; }

        /// <summary>
        /// 就诊科室id
        /// </summary>
        public string JIUZHENKSID { get; set; }

        /// <summary>
        /// 就诊科室名称
        /// </summary>
        public string JiuZhenKSMC { get; set; }

        /// <summary>
        /// 院区ID
        /// </summary>
        public string YUANQUID { get; set; }

        /// <summary>
        /// 库存应用ID
        /// </summary>
        public string KUCUNYYID { get; set; }

        /// <summary>
        /// 工作站ID
        /// </summary>
        public string GONGZUOZID { get; set; }

        /// <summary>
        /// 门诊价格体系
        /// </summary>
        public int? MENZHENJGTX { get; set; }

        /// <summary>
        /// 住院价格体系
        /// </summary>
        public int? ZHUYUANJGTX { get; set; }

        /// <summary>
        /// 库房_库存管理类型
        /// </summary>
        public string KUCUNGLLX { get; set; }

        /// <summary>
        /// 当前窗口名称
        /// </summary>
        public string DANGQIANCKMC { get; set; }

        /// <summary>
        /// 自定义上下文字典
        /// </summary>
        public Dictionary<string, object> Contexts { get; set; } = new Dictionary<string, object>();
    }
}
