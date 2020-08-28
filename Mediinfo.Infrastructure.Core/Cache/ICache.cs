namespace Mediinfo.Infrastructure.Core.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        object Get(string cacheKey);

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        bool Add(string cacheKey, object cacheObject);

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        object Update(string cacheKey, object cacheObject);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool Remove(string cacheKey);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool Exist(string cacheKey);
    }
}
