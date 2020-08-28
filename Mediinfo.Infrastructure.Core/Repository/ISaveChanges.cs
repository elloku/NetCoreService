using Mediinfo.Infrastructure.Core.Entity;

using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// SaveChagnes接口
    /// </summary>
    public interface ISaveChanges
    {
        /// <summary>
        /// 注册为新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        TEntity RegisterAdd<TEntity>(TEntity entity,bool autoSaveChanges=false) where TEntity : EntityBase;
        
        /// <summary>
        /// 批量注册为新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitsy"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterAdd<TEntity>(IEnumerable<TEntity> entitsy,bool autoSaveChanges= false) where TEntity : EntityBase;

        /// <summary>
        /// 注册为修改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        TEntity RegisterUpdate<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase;
        
        /// <summary>
        /// 批量注册为修改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterUpdate<TEntity>(IEnumerable<TEntity> entitys, bool autoSaveChanges = false) where TEntity : EntityBase;

        /// <summary>
        /// 注册为删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        TEntity RegisterDelete<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase;
        
        /// <summary>
        /// 注册为删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="autoSaveChanges"></param>
        /// <returns></returns>
        IEnumerable<TEntity> RegisterDelete<TEntity>(IEnumerable<TEntity> entitys, bool autoSaveChanges = false) where TEntity : EntityBase;

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="bulkSize">批量保存的记录数</param>
        /// <returns></returns>
        int BulkSaveChanges(bool validateOnSaveEnabled = true,int bulkSize = 64);
    }
}
