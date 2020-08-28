using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.Repository;

using NLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mediinfo.Infrastructure.Core.Entity
{
    /// <summary>
    /// 实体抽象基类
    /// </summary>
    public abstract class EntityBase : IAggregateRoot, ICanShu,IDefaultValue
    {
        public IRepositoryBase IRepositoyBase;
        protected ServiceContext ServiceContext;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EntityBase()
        {
            this.IRepositoyBase = null;
            this.ServiceContext = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="sc"></param>
        public EntityBase(IRepositoryBase repo,ServiceContext sc)
        {
            this.Initialize(repo, sc);
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="sc"></param>
        public void Initialize(IRepositoryBase repo, ServiceContext sc)
        {
            this.IRepositoyBase = repo;
            this.ServiceContext = sc;
        }

        ///// <summary>
        ///// 实体注册到DbSet
        ///// </summary>
        ///// <param name="configurations"></param>
        //public virtual void RegistTo(ConfigurationRegistrar configurations)
        //{
        //    Type type = typeof(EntityTypeConfiguration<>).MakeGenericType(this.GetType());

        //    object param = Activator.CreateInstance(type);

        //    typeof(ConfigurationRegistrar).GetMethods()[1]
        //        .MakeGenericMethod(new Type[] { this.GetType() })
        //        .Invoke(configurations, new object[] { param });
        //}

        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="xuHaoMing"></param>
        /// <param name="qianZhui"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual List<string> GetOrder(string xuHaoMing, string qianZhui = null, int count = 1)
        {
            return IRepositoyBase.GetOrder(xuHaoMing, qianZhui, count);
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        protected virtual DateTime GetSYSTime()
        {
            return IRepositoyBase.GetSYSTime();
        }

        /// <summary>
        /// 获取实体（仅建议在聚合的子实体实现延迟加载时使用）
        /// </summary>
        /// <typeparam name="TEtntiy"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TEtntiy> GetByPredicate<TEtntiy>(Expression<Func<TEtntiy, bool>> predicate) where TEtntiy : EntityBase
        {
            return this.IRepositoyBase.GetSet<TEtntiy>().Where(predicate).ToList();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="yingyongId">应用id</param>
        /// <param name="canShuId">参数ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public string GetCanShu(string yingyongId, string canShuId, string defaultValue)
        {
            return this.IRepositoyBase.GetCanShu(yingyongId, canShuId, defaultValue);
        }

        /// <summary>
        /// 获取本应用参数
        /// </summary>
        /// <param name="canShuId">参数ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public string GetCanShu(string canShuId, string defaultValue)
        {
            return IRepositoyBase.GetCanShu(this.ServiceContext.YINGYONGID, canShuId, defaultValue);
        }

        #region 增、删、改

        /// <summary>
        /// 将实体标记为新增状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">需要新增的实体</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected TEntity RegisterAdd<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterAdd<TEntity>(entity, autoSaveChanges);
        }

        /// <summary>
        /// 将实体标记为新增状态（批量）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities">需要标记为新增状态的实体列表</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected IEnumerable<TEntity> RegisterAdd<TEntity>(IEnumerable<TEntity> entities, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterAdd<TEntity>(entities, autoSaveChanges);
        }

        /// <summary>
        /// 将实体标记为修改状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">需要提交更新的实体</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected TEntity RegisterUpdate<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterUpdate<TEntity>(entity, autoSaveChanges);
        }

        /// <summary>
        /// 将实体标记为修改状态（批量）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities">需要提交更新的实体列表</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected IEnumerable<TEntity> RegisterUpdate<TEntity>(IEnumerable<TEntity> entities, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterUpdate<TEntity>(entities, autoSaveChanges);
        }

        /// <summary>
        /// 将实体标记为删除状态
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">需要提交删除的实体</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected TEntity RegisterDelete<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterDelete<TEntity>(entity, autoSaveChanges);
        }

        /// <summary>
        /// 将实体标记为删除状态（批量）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities">需要提交删除的实体列表</param>
        /// <param name="autoSaveChanges">是否自动提交保存</param>
        /// <returns></returns>
        protected IEnumerable<TEntity> RegisterDelete<TEntity>(IEnumerable<TEntity> entities, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            return this.IRepositoyBase.RegisterDelete<TEntity>(entities, autoSaveChanges);
        }

        [Ignore]
        public virtual void SetDefaultValue()
        {
            //throw new NotImplementedException("未实现ApplyDefaultValue，请重新生成Domain对象！");
        }

        #endregion

        //public virtual Lock(int waitTime)
        //{
        //    //this.iRepositoyContext.LockTable<this.GetType()>
        //}
    }
}
