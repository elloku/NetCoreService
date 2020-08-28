using CacheManager.Core;
using Mediinfo.APIGateway.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediinfo.APIGateway.Core.Cache
{
    /// <summary>
    /// 灰度发布缓存
    /// </summary>
    public class ABTestingCache
    {
        private static readonly ABTestingCache _instance = new ABTestingCache();
        private readonly ICacheManager<object> _cache;
        private bool initCaching = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private ABTestingCache()
        {
            _cache = CacheFactory.Build(settings => settings
       .WithUpdateMode(CacheUpdateMode.Up)
       .WithSystemRuntimeCacheHandle()
           .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10)));
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static ABTestingCache Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 获取服务状态
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public async Task<string> GetServiceStatus(string serviceName, string serviceVersion, string serviceId)
        {
            var result = Get<string>("status-" + serviceId);
            if (string.IsNullOrEmpty(result))
            {
                result = await ConsulKVManager.Instance.GetValue(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ServiceStatus,
                    serviceName, serviceVersion, serviceId));
                InitCache();
            }
            return result;
        }

        /// <summary>
        /// 获取灰度IP
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetServiceTestIp(string serviceName, string serviceVersion, string serviceId)
        {
            var result = Get<List<string>>("testip-" + serviceId);
            if (result == null || result.Count <= 0)
            {
                result = (await ConsulKVManager.Instance.GetValue(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ABTestingIp,
                    serviceName, serviceVersion, serviceId))).Split('\n').ToList();
                InitCache();
            }

            return result;
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitCache()
        {
            if (!initCaching)
            {
                var serviceList = await ConsulKVManager.Instance.ListValue("service/abtesting/");
                var statusList = serviceList.Where(m => m.Key.Split('/').Length == 5);
                foreach (var item in statusList)
                {
                    await AddOrUpdateAsync("status-" + item.Key.Split('/')[4], item.Value);
                }

                var ipList = serviceList.Where(m => m.Key.Split('/').Length == 6);
                foreach (var item in ipList)
                {
                    await AddOrUpdateAsync("testip-" + item.Key.Split('/')[4], item.Value.Split('\n').ToList());
                }
            }
            initCaching = false;
            return true;
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> Add(string key, object value)
        {
            return await Task.Run<bool>(() =>
            {
                return _cache.Add(key, value);
            });
        }

        /// <summary>
        /// 新增或更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object AddOrUpdate(string key, object value)
        {
            return _cache.AddOrUpdate(key, value, (old) => { return value; });
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Update(string key, object value)
        {
            return _cache.Update(key, (old) => { return value; });
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return _cache.Remove(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Get(string key, object value)
        {
            return _cache.GetOrAdd(key, value);
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(string key, object value)
        {
            return await Task.Run<bool>(() =>
            {
                return _cache.Add(key, value);
            });
        }

        /// <summary>
        /// 新增、修改缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<object> AddOrUpdateAsync(string key, object value)
        {
            return await Task.Run<object>(() =>
            {
                return _cache.AddOrUpdate(key, value, (old) => { return value; });
            });
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<object> UpdateAsync(string key, object value)
        {
            return await Task.Run<object>(() =>
            {
                return _cache.Update(key, (old) => { return value; });
            });
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key)
        {
            return await Task.Run<bool>(() =>
            {
                return _cache.Remove(key);
            });
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run<T>(() =>
            {
                return _cache.Get<T>(key);
            });
        }

        /// <summary>
        /// 获取和更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<object> GetOrAddAsync(string key, object value)
        {
            return await Task.Run<object>(() =>
            {
                return _cache.GetOrAdd(key, value);
            });
        }
    }
}
