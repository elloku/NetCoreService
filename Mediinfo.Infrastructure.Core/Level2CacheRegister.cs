using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using Mediinfo.DTO.Core;

namespace Mediinfo.Infrastructure.Core
{
    // 2级缓存注册
    public class Level2CacheRegister
    {
        static MemoryCache Cache = MemoryCache.Default;
        static CacheItemPolicy policy = new CacheItemPolicy();

        /// <summary>
        /// 构造函数
        /// </summary>
        static Level2CacheRegister()
        {
            // 10分钟不被访问就移除
            policy.Priority = CacheItemPriority.Default;
            policy.SlidingExpiration = new TimeSpan(0, 10, 0);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="GetData"></param>
        /// <returns></returns>
        public static List<T> Get<T>(string sql, Func<string, List<T>> GetData) where T : DTOBase
        {
            if (Cache.Contains(sql))
            {
                return (List<T>)Cache.Get(sql);
            }
            var list = GetData(sql);
            Cache.Set(sql, GetData(sql), policy);
            return list;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="GetData"></param>
        /// <returns></returns>
        public static DataTable Get(string sql, Func<string, DataTable> GetData)
        {
            if (Cache.Contains(sql))
            {
                return (DataTable)Cache.Get(sql);
            }
            var list = GetData(sql);
            Cache.Set(sql, GetData(sql), policy);
            return list;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="Parameties"></param>
        /// <param name="GetData"></param>
        /// <returns></returns>
        public static List<T> Get<T>(string sql, Dictionary<string, object> Parameties, Func<string, Dictionary<string, object>, List<T>> GetData) where T : DTOBase
        {
            string newSql = sql;
            if (Parameties != null)
            {
                Parameties.ToList().ForEach(o =>
                {
                    newSql += "|" + o.Key + "|" + (o.Value == null ? string.Empty : o.Value.ToString());
                });
            }
            if (Cache.Contains(sql))
            {
                return (List<T>)Cache.Get(newSql);
            }
            var list = GetData(sql, Parameties);
            Cache.Set(sql, GetData(sql, Parameties), policy);
            return list;
        }
    }
}
