using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.Entity;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>, IDisposable where TEntity : EntityBase
    {
        protected IRepositoryContext IRepoContext;

        protected ServiceContext ServiceContext;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RepositoryBase()
        {


        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sContext"></param>
        public RepositoryBase(IRepositoryContext context, ServiceContext sContext)
        {
            IRepoContext = context;
            ServiceContext = sContext;
        }

        #region 查询方法

        /// <summary>
        /// 查询ef
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> Set()
        {
            return this.IRepoContext.GetSet<TEntity>();
        }

        /// <summary>
        /// 分离EF实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public virtual void Detach(TEntity o)
        {
            this.IRepoContext.Detach(o);
        }

        /// <summary>
        /// 附加实体
        /// </summary>
        /// <param name="o"></param>
        public virtual void Attach(TEntity o)
        {
            this.IRepoContext.Attach(o);
        }

        /// <summary>
        /// 查询ef无缓存
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> QuerySet()
        {
            return this.IRepoContext.GetSet<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// 查询ef
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<TEntity1> Set<TEntity1>() where TEntity1 : EntityBase
        {
            return this.IRepoContext.GetSet<TEntity1>();
        }

        /// <summary>
        /// 查询ef无缓存
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<TEntity1> QuerySet<TEntity1>() where TEntity1 : EntityBase
        {
            return this.IRepoContext.GetSet<TEntity1>().AsNoTracking();
        }

        #endregion 

        /// <summary>
        /// 根据主键获取实体（参数顺序需要与实体中定义的顺序一致）
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual TEntity GetByKey(params object[] keyValues)
        {
            var t = this.IRepoContext.GetSet<TEntity>().Find(keyValues);
            if (t != null)
            {
                t.Initialize(this, this.ServiceContext);
            }
            return t;
        }

        #region 新增

        /// <summary>
        /// 注册为新增
        /// </summary>
        /// <param name="item"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public virtual TEntity RegisterAdd(TEntity item, bool autoSaveChanges = true)
        {
            return this.IRepoContext.RegisterAdd<TEntity>(item, autoSaveChanges);
        }

        /// <summary>
        /// 注册为新增
        /// </summary>
        /// <param name="items"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> RegisterAdd(IEnumerable<TEntity> items, bool autoSaveChanges = false)
        {
            return this.IRepoContext.RegisterAdd<TEntity>(items, autoSaveChanges);
        }

        #endregion

        #region 删除

        public virtual TEntity RegisterDelete(TEntity item, bool autoSaveChanges = false)
        {
            return this.IRepoContext.RegisterDelete<TEntity>(item, autoSaveChanges);
        }

        public virtual IEnumerable<TEntity> RegisterDelete(IEnumerable<TEntity> items, bool autoSaveChanges = false)
        {
            return this.IRepoContext.RegisterDelete<TEntity>(items, autoSaveChanges);
        }

        #endregion

        #region 更新

        public virtual TEntity RegisterUpdate(TEntity item, bool autoSaveChanges = false)
        {
            return this.IRepoContext.RegisterUpdate<TEntity>(item, autoSaveChanges);
        }

        public virtual IEnumerable<TEntity> RegisterUpdate(IEnumerable<TEntity> items, bool autoSaveChanges = false)
        {
            return this.IRepoContext.RegisterUpdate<TEntity>(items, autoSaveChanges);
        }

        #endregion

        #region 对象析构方法

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    IRepoContext.Dispose();
                    IRepoContext = null;
                    ServiceContext = null;
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion region

        #region 缓存处理

        protected virtual T GetFromCache<T>(string cacheKey) where T : class
        {
            return IRepoContext.GetFromCache<T>(cacheKey);
        }

        protected virtual bool AddToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            return IRepoContext.AddToCache<T>(cacheKey, cacheObject);
        }

        protected virtual void PutToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            IRepoContext.PutToCache<T>(cacheKey, cacheObject);
        }

        protected virtual T UpdateToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            return IRepoContext.UpdateToCache<T>(cacheKey, cacheObject);
        }

        protected virtual bool RemoveFromCache<T>(string cacheKey) where T : class
        {
            return IRepoContext.RemoveFromCache<T>(cacheKey);
        }

        protected virtual bool ExistInCache<T>(string cacheKey) where T : class
        {
            return IRepoContext.ExistInCache<T>(cacheKey);
        }

        #endregion

        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="XuHaoMC"></param>
        /// <param name="QianZhui"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public List<string> GetOrder(string XuHaoMC, string QianZhui = null, int Count = 1)
        {
            return IRepoContext.GetOrder(XuHaoMC, QianZhui, Count);
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetSYSTime()
        {
            return IRepoContext.GetSYSTime();
        }

        /// <summary>
        /// 保存变更
        /// </summary>
        /// <returns></returns>
        protected virtual int SaveChanges()
        {
            return this.IRepoContext.SaveChanges();
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        protected virtual int BulkSaveChanges(bool validateOnSaveEnabled, int bulkSize = 64)
        {
            return this.IRepoContext.BulkSaveChanges(validateOnSaveEnabled, bulkSize);
        }

        /// <summary>
        /// 注册新增
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="item"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public TEntity1 RegisterAdd<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterAdd<TEntity1>(item, autoSaveChanges);
        }

        /// <summary>
        /// 注册新增
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="items"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public IEnumerable<TEntity1> RegisterAdd<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterAdd<TEntity1>(items, autoSaveChanges);
        }

        /// <summary>
        /// 注册修改
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="item"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public TEntity1 RegisterUpdate<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterUpdate<TEntity1>(item, autoSaveChanges);
        }

        /// <summary>
        /// 注册修改
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="items"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public IEnumerable<TEntity1> RegisterUpdate<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterUpdate<TEntity1>(items, autoSaveChanges);
        }

        /// <summary>
        /// 注册删除
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="item"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public TEntity1 RegisterDelete<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterDelete<TEntity1>(item, autoSaveChanges);
        }

        /// <summary>
        /// 注册删除
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="items"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        public IEnumerable<TEntity1> RegisterDelete<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase
        {
            return this.IRepoContext.RegisterDelete<TEntity1>(items, autoSaveChanges);
        }

        /// <summary>
        /// 锁表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public List<TEntity> LockTable<TEntity>(string where, int waitTime) where TEntity : EntityBase
        {
            return this.IRepoContext.LockTable<TEntity>(where, waitTime);
        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected IEnumerable<TEntity1> SqlQuery<TEntity1>(string sql, params object[] parameters) where TEntity1 : class
        {
            return this.IRepoContext.SqlQuery<TEntity1>(sql, parameters);
        }

        /// <summary>
        /// 直接执行SQL语句（谨慎使用）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return this.IRepoContext.ExecuteSqlCommand(sql, parameters);
        }

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public TEntity1 GetByKey<TEntity1>(params object[] keyValues) where TEntity1 : EntityBase
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据参数获取
        /// </summary>
        /// <param name="yingyongId"></param>
        /// <param name="canShuId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetCanShu(string yingyongId, string canShuId, string defaultValue)
        {
            return this.IRepoContext.GetCanShu(yingyongId, canShuId, defaultValue);
        }

        /// <summary>
        /// 获取ef实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> GetSet<T>() where T : class
        {
            return this.IRepoContext.GetSet<T>();
        }

        /// <summary>
        /// 初始化缓存（全表缓存）
        /// </summary>
        private void InitCache()
        {
            //***************************************************************************
            // 这里可以考虑增加全表缓存限制，比如：当表行数超过一定数量后，禁止全表缓存 *
            //***************************************************************************
            var list = GetFromCache<List<TEntity>>(typeof(TEntity).FullName);
            if (list == null)
            {
                list = this.QuerySet<TEntity>().ToList();
                lock (wLock)
                {
                    if (GetFromCache<List<TEntity>>(typeof(TEntity).FullName) == null)
                    {
                        lock (wLock)
                        {
                            AddToCache<List<TEntity>>(typeof(TEntity).FullName, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化缓存（条件缓存）
        /// </summary>
        /// <param name="predicate">缓存条件</param>
        private void InitCache(Expression<Func<TEntity, bool>> predicate)
        {
            var list = GetFromCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate);
            if (list == null)
            {
                list = this.QuerySet<TEntity>().Where(predicate).ToList();
                lock (wLock)
                {
                    if (GetFromCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate) == null)
                    {
                        lock (wLock)
                        {
                            AddToCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate, list);
                        }
                    }
                }
            }
        }

        // 写锁
        private static readonly object wLock = new object();
        /// <summary>
        /// 获取全表缓存
        /// </summary>
        /// <returns></returns>
        public List<TEntity> UseCache()
        {
            var list = GetFromCache<List<TEntity>>(typeof(TEntity).FullName);
            if (list == null)
            {
                list = this.QuerySet<TEntity>().ToList();
                lock (wLock)
                {
                    if (GetFromCache<List<TEntity>>(typeof(TEntity).FullName) == null)
                    {
                        lock (wLock)
                        {
                            AddToCache<List<TEntity>>(typeof(TEntity).FullName, list);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 缓存条件缓存
        /// </summary>
        /// <param name="predicate">缓存条件</param>
        /// <returns></returns>
        public List<TEntity> UseCache(Expression<Func<TEntity, bool>> predicate)
        {
            var list = GetFromCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate);
            if (list == null)
            {
                list = this.QuerySet<TEntity>().Where(predicate).ToList();
                lock (wLock)
                {
                    if (GetFromCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate) == null)
                    {
                        lock (wLock)
                        {
                            AddToCache<List<TEntity>>(typeof(TEntity).FullName + "=>" + predicate, list);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return this.IRepoContext.GetDataTable(sql);
        }

        public void ExecuteProc(string procName, params DbParameter[] dbParameter)
        {
            this.IRepoContext.ExecuteProc(procName, dbParameter);
        }
    }
}
