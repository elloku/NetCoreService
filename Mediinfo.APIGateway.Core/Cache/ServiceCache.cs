using CacheManager.Core;

using System;
using System.Threading.Tasks;

namespace Mediinfo.APIGateway.Core.Cache
{
    /// <summary>
    /// API网关缓存的实现，基于CacheManager的单例模式
    /// </summary>
    public class ServiceCache
    {
        private readonly ICacheManager<object> _cache;
        private static readonly ServiceCache _instance = new ServiceCache();

        /// <summary>
        /// 服务缓存
        /// </summary>
        public ServiceCache()
        {
            _cache = CacheFactory.Build(settings => settings
       .WithUpdateMode(CacheUpdateMode.Up)
       .WithSystemRuntimeCacheHandle()
           .WithExpiration(ExpirationMode.None, TimeSpan.FromDays(3650)));
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static ServiceCache Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 新增
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
        /// 新增或更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object AddOrUpdate(string key, object value)
        {
            return _cache.AddOrUpdate(key, value, (old) => { return value; });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Update(string key, object value)
        {
            return _cache.Update(key, (old) => { return value; });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return _cache.Remove(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 获取或新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Get(string key, object value)
        {
            return _cache.GetOrAdd(key, value);
        }

        /// <summary>
        /// 新增
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
        /// 新增或更新
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
        /// 更新
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
        /// 删除
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
        /// 获取
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
        /// 获取
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
