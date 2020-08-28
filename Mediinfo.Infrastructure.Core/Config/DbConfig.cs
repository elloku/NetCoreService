using Mediinfo.Utility;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mediinfo.Infrastructure.Core.Config
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbConfig
    {
        private static readonly DbConfig dbConfig = new DbConfig();
        
        /// <summary>
        /// his6 数据库连接
        /// </summary>
        private string HIS6;
        
        /// <summary>
        /// 医保数据库连接
        /// </summary>
        private string YB;

        private DbConfig()
        {
            string json = IOHelper.Read(AppDomain.CurrentDomain.BaseDirectory + "DbConfig.json");
            JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
            HIS6 = jObject["HIS6"].ToString();
            YB = jObject["YB"].ToString();
        }

        public static DbConfig Instance
        {
            get
            {
                return dbConfig;
            }
        }

        public string GetHisConfig()
        {
            return HIS6;
        }

        public string GetYbConfig()
        {
            return YB;
        }
    }
}
