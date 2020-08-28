using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Mediinfo.Infrastructure.Core.Cache
{
    /// <summary>
    /// 上下文缓存
    /// </summary>
    public class ContextCache:IDisposable
    {
        private MemoryCache cache = null;
        private CacheItemPolicy cp = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContextCache()
        {
            cache = new MemoryCache("ContextCache");
            cp = new CacheItemPolicy();
        }

        #region 缓存

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            var list = cache.ToList();
            foreach (var item in list)
            {
                cache.Remove(item.Key);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public virtual T GetFromCache<T>(string cacheKey) where T : class
        {
            if (null == this.cache)
                return default(T);

            return (T)this.cache.Get(cacheKey);
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public virtual bool AddToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.cache)
                return false;

            return this.cache.Add(cacheKey, cacheObject, cp);
        }

        /// <summary>
        /// 新增缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        public virtual void PutToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.cache)
                return;

            this.cache.Add(cacheKey, cacheObject,cp);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObject"></param>
        /// <returns></returns>
        public virtual T UpdateToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.cache)
                return default(T);

            cache.Remove(cacheKey);


            bool o = this.cache.Add(cacheKey, cacheObject, cp);
            if (!o)
                return default(T);

            return (T)cacheObject;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public virtual bool RemoveFromCache<T>(string cacheKey) where T : class
        {
            if (null == this.cache)
                return true;

            return this.cache.Remove(cacheKey) != null;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public virtual bool ExistInCache<T>(string cacheKey) where T : class
        {
            if (null == this.cache)
                return false;

            return this.cache.Where(m=>m.Key == cacheKey).Count() > 0;
        }
       
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <param name="XuHaoMC"></param>
        /// <param name="QianZhui"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public virtual List<string> GetOrder(string XuHaoMC, string QianZhui = null, int Count = 1)
        {
            return new List<string>();
        }

        private bool disposed = false;

        /// <summary>
        /// 析构
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (null != cache)
                    {
                        cache.Dispose();
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
