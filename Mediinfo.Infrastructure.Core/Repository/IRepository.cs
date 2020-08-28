using Mediinfo.Infrastructure.Core.Entity;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 仓储基类接口
    /// </summary>
    public interface IRepositoryBase : IGetOrder, ICanShu, ISYSTime, ILockEntity
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbSet<T> GetSet<T>() where T : class;

        #region 泛型方法，暂时不启用

        TEntity1 RegisterAdd<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        IEnumerable<TEntity1> RegisterAdd<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        TEntity1 RegisterUpdate<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        IEnumerable<TEntity1> RegisterUpdate<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        TEntity1 RegisterDelete<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        IEnumerable<TEntity1> RegisterDelete<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        #endregion
    }

    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IRepositoryBase, IDisposable where TEntity : EntityBase
    {
        #region 查询接口，暂时不开放

        //IQueryable<TEntity> Set();
        //IQueryable<TEntity1> Set<TEntity1>() where TEntity1 :EntityBase;

        //IQueryable<TEntity> QuerySet();
        //IQueryable<TEntity1> QuerySet<TEntity1>() where TEntity1 : EntityBase;

        #endregion

        /// <summary>
        /// 初始化缓存（全表缓存）
        /// </summary>
        //void InitCache();

        /// <summary>
        /// 初始化缓存（条件缓存）
        /// </summary>
        /// <param name="predicate">缓存条件</param>
        //void InitCache(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取全表缓存
        /// </summary>
        /// <returns></returns>
        List<TEntity> UseCache();

        /// <summary>
        /// 获取条件缓存
        /// </summary>
        /// <param name="predicate">缓存条件</param>
        /// <returns></returns>
        List<TEntity> UseCache(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 分离实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="o"></param>
        void Detach(TEntity o);

        /// <summary>
        /// 附加实体
        /// </summary>
        /// <param name="o"></param>
        void Attach(TEntity o);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity GetByKey(params object[] keyValues);

        /// <summary>
        /// 将实体标记为新增
        /// </summary>
        /// <param name="item">需要新增的实体</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        TEntity RegisterAdd(TEntity item, bool autoSaveChanges = false);

        /// <summary>
        /// 将实体标记为新增（批量）
        /// </summary>
        /// <param name="items">需要新增的实体列表</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterAdd(IEnumerable<TEntity> items, bool autoSaveChanges = false);

        /// <summary>
        /// 将实体标记为删除
        /// </summary>
        /// <param name="item">需要删除的实体</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        TEntity RegisterDelete(TEntity item, bool autoSaveChanges = false);

        /// <summary>
        /// 将实体标记为删除（批量）
        /// </summary>
        /// <param name="items">需要删除的实体列表</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterDelete(IEnumerable<TEntity> items, bool autoSaveChanges = false);

        /// <summary>
        /// 将实体标记为修改
        /// </summary>
        /// <param name="item">需要更新的实体列表</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        TEntity RegisterUpdate(TEntity item, bool autoSaveChanges = false);

        /// <summary>
        /// 将实体标记为修改（批量）
        /// </summary>
        /// <param name="items">需要修改的实体列表</param>
        /// <param name="autoSaveChanges">是否自动保存</param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterUpdate(IEnumerable<TEntity> items, bool autoSaveChanges = false);
        
        #region 泛型方法，暂时不启用

        //TEntity1 RegisterAdd<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        //IEnumerable<TEntity1> RegisterAdd<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        //TEntity1 RegisterUpdate<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        //IEnumerable<TEntity1> RegisterUpdate<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        //TEntity1 RegisterDelete<TEntity1>(TEntity1 item, bool autoSaveChanges = false) where TEntity1 : EntityBase;
        //IEnumerable<TEntity1> RegisterDelete<TEntity1>(IEnumerable<TEntity1> items, bool autoSaveChanges = false) where TEntity1 : EntityBase;

        #endregion
    }
}
