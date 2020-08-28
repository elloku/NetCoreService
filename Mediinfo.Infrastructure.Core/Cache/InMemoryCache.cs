using System.Runtime.Caching;

namespace Mediinfo.Infrastructure.Core.Cache
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public class InMemoryCache : ICache
    {
        // 缓存初始化
        private MemoryCache cache = null;
        private CacheItemPolicy cp = null;

        /// <summary>
        /// 使用内存缓存
        /// </summary>
        public InMemoryCache()
        {
            cache = new MemoryCache("HALOCache");
            cp = new CacheItemPolicy();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            if (null == this.cache)
                return null;

            return this.cache.Get(cacheKey);
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public bool Add(string cacheKey, object cacheObject)
        {
            if (null == this.cache)
                return false;

            return this.cache.Add(cacheKey, cacheObject, cp);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool Exist(string cacheKey)
        {
            if (null == this.cache)
                return false;

            return this.cache.Get(cacheKey) != null;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool Remove(string cacheKey)
        {
            if (null == this.cache)
                return true;

            return this.cache.Remove(cacheKey) != null;
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public object Update(string cacheKey, object cacheObject)
        {
            if (null == this.cache)
                return null;

            // 先删除
            cache.Remove(cacheKey);


            bool o = this.cache.Add(cacheKey, cacheObject, cp);
            if (!o)
                return null;

            return cacheObject;
        }
    }
}
