namespace Mediinfo.Infrastructure.Core.Cache
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// 缓存接口
        /// </summary>
        private ICache cache = null;

        /// <summary>
        /// 加载缓存
        /// </summary>
        private CacheManager()
        {
            cache = new InMemoryCache();
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static CacheManager Cache { get; } = new CacheManager();

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            return cache.Get(cacheKey);
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public bool Add(string cacheKey, object cacheObject)
        {
            return cache.Add(cacheKey, cacheObject);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool Exist(string cacheKey)
        {
            return cache.Exist(cacheKey);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool Remove(string cacheKey)
        {
            return Remove(cacheKey);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public object Update(string cacheKey, object cacheObject)
        {
            return cache.Update(cacheKey, cacheObject);
        }
    }
}
