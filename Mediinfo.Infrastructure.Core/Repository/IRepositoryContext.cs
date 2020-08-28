using Mediinfo.Enterprise.PagedResult;
using Mediinfo.Infrastructure.Core.Entity;
using Mediinfo.Infrastructure.Core.UnitOfWork;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 仓储上下文
    /// </summary>
    public interface IRepositoryContext : ICacheMangager, ISaveChanges,IGetOrder,ISYSTime,ILockEntity, ICanShu,IDisposable
    {
        /// <summary>
        /// 获取DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbSet<T> GetSet<T>() where T : class;

        /// <summary>
        /// 获取domian
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters) where TEntity : class;
        
        /// <summary>
        /// 获取domian
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<TEntity> SqlQuery<TEntity>(string sql,Dictionary<string,object> parameters) where TEntity : class;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="sort"></param>
        /// <param name="isAsc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IPagedResult<TEntity> PagedQuery<TEntity>(string sql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20,  string sort = "") where TEntity : class;
        
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable GetDataTable(string sql);

        IPagedTableResult GetPagedDataTable(string sql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "");
        
        /// <summary>
        /// 分离EF实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        void Detach<T>(T o) where T : EntityBase;

        void Attach<T>(T o) where T : EntityBase;

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        void ExecuteProc(string procName, params DbParameter[] dbParameter);

        //List<T> SqlQuery
    }
}
