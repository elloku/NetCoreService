using System;
using System.Collections.Generic;

namespace Mediinfo.Enterprise.Log
{
    /// <summary>
    /// 日志模型
    /// </summary>
    public class LogEntity
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public string RiZhiID { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 创建时间
        /// </summary>
        public string ChuangJianSj { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        /// <summary>
        /// 自定义日志索引
        /// </summary>
        public string SuoYin { get; set; }

        /// <summary>
        /// 日志级别:警告、错误、信息等
        /// </summary>
        public string RiZhiJb { get; set; }

        /// <summary>
        /// 日志标题
        /// </summary>
        public string RiZhiBt { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string RiZhiNr { get; set; }
    }

    public class LogEntityList : List<LogEntity>
    {
        public int Hits { get; set; }

        public int Took { get; set; }
    }

    public class CustRootLogEntity
    {
        public int took { get; set; }

        public bool timed_out { get; set; }

        public CustShards _shards { get; set; }

        public CustHit hits { get; set; }
    }

    public class CustShards
    {
        public int total { get; set; }

        public int successful { get; set; }

        public int skipped { get; set; }

        public int failed { get; set; }
    }

    public class CustHit
    {

        public CustTotal total { get; set; }

        public string max_score { get; set; }

        public List<CustHits> hits { get; set; }
    }

    public class CustTotal
    {
        public int value { get; set; }

        public string relation { get; set; }
    }

    public class CustHits
    {
        /// <summary>
        /// his_cus_系统日志
        /// </summary>
        public string _index { get; set; }

        public string _type { get; set; }

        public string _id { get; set; }

        public string _score { get; set; }

        public LogEntity _source { get; set; }

    }
}
