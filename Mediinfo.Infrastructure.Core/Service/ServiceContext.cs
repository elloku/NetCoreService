using Mediinfo.Enterprise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Service.UnitOfWork
{
    /// <summary>
    /// 客户端调用服务的上下文信息
    /// 注意：如果在此处进行修改了，那么需要在MediProxy赋值的地方手工处理
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
        public static string KESHIMC { get; set; }
        /// <summary>
        /// 病区ID
        /// </summary>
        public static string BINGQUID { get; set; }

        /// <summary>
        /// 病区名称
        /// </summary>
        public static string BINGQUMC { get; set; }

        /// <summary>
        /// 就诊科室id
        /// </summary>
        public static string JIUZHENKSID { get; set; }

        /// <summary>
        /// 就诊科室名称
        /// </summary>
        public static string JiuZhenKSMC { get; set; }
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

    }
}
