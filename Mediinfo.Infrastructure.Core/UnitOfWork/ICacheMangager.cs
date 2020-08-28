namespace Mediinfo.Infrastructure.Core.UnitOfWork
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICacheMangager
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        T GetFromCache<T>(string cacheKey) where T : class;

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        bool AddToCache<T>(string cacheKey, object cacheObject) where T : class;

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        void PutToCache<T>(string cacheKey, object cacheObject) where T : class;

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        T UpdateToCache<T>(string cacheKey, object cacheObject) where T : class;

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool RemoveFromCache<T>(string cacheKey) where T : class;

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        bool ExistInCache<T>(string cacheKey) where T : class;
    }

}
