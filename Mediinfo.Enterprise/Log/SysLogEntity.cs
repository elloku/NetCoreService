using Mediinfo.Utility.Extensions;

using System;
using System.Collections.Generic;

namespace Mediinfo.Enterprise.Log
{
    /// <summary>
    /// 系统日志模型
    /// </summary>
    public class SysLogEntity
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public string RiZhiID { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 创建时间
        /// </summary>
        public string ChuangJianSj { get; set; } = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");

        /// <summary>
        /// 日志标题
        /// </summary>
        public string RiZhiBt { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string RiZhiNr { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string FuWuMc { get; set; }

        /// <summary>
        /// 请求来源
        /// </summary>
        public string QingQiuLy { get; set; }

        /// <summary>
        /// 日志类型：1.菜单打开，2.客户端异常，3.服务调用，4服务端异常，5.SQL日志，6.性能日志 7.控件操作
        /// </summary>
        public int RiZhiLx { get; set; }

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
        public string JIUZHENKSMC { get; set; }

        /// <summary>
        /// 坐诊类型
        /// </summary>
        public string ZUOZHENLX { get; set; }

        /// <summary>
        /// 院区ID
        /// </summary>
        public string YUANQUID { get; set; }

        /// <summary>
        /// 工作站ID
        /// </summary>
        public string GONGZUOZID { get; set; }

        /// <summary>
        /// 服务耗时
        /// </summary>
        public float FuWuHs { get; set; }
    }

    public class SysLogEntityList : List<SysLogEntity>
    {
        public int Hits { get; set; }

        public int Took { get; set; }
    }

    public class SysRootLogEntity
    {
        public int took { get; set; }

        public bool timed_out { get; set; }

        public SysShards _shards { get; set; }

        public SysHit hits { get; set; }
    }

    public class SysShards
    {
        public int total { get; set; }

        public int successful { get; set; }

        public int skipped { get; set; }

        public int failed { get; set; }
    }

    public class SysHit
    {
        public SysTotal total { get; set; }

        public string max_score { get; set; }

        public List<SysHits> hits { get; set; }
    }

    public class SysTotal
    {
        public int value { get; set; }

        public string relation { get; set; }
    }

    public class SysHits
    {
        public string _index { get; set; }

        public string _type { get; set; }

        public string _id { get; set; }

        public string _score { get; set; }

        public SysLogEntity _source { get; set; }

        //public List<int> sort { get; set; }
    }
}
