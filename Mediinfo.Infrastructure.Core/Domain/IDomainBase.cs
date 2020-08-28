using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Domain
{
    public interface IDomainBase
    {
        /// <summary>
        /// 新增无事务
        /// </summary>
        /// <param name="parm">需要新增的实体</param>
        /// <returns>返回无状态的新增的实体</returns>
       T Insert<T>(T parm) where T : DBBaseEntity, new();

        /// <summary>
        /// 更新无事务
        /// </summary>
        /// <param name="parm">需要更新的实体</param>
        /// <returns>返回无状态的更新的实体</returns>
        T Update<T>(T parm) where T : DBBaseEntity, new();
        
        /// <summary>
        /// 删除无事务
        /// </summary>
        /// <param name="parm">需要删除的实体必须有主键</param>
        /// <returns>返回数据库影响的行数</returns>
        int Delete<T>(T parm) where T : DBBaseEntity, new();
        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns>返回的实体</returns>
        T GetByKey<T>(T parm) where T : DBBaseEntity, new();
        /// <summary>
        /// LINQ查询
        /// </summary>
        /// <param name="where">Where条件</param>
        /// <returns></returns>
        List<T> GetByExpression<T>(Expression<Func<T, bool>> where) where T : DBBaseEntity, new();

        /// <summary>
        /// 在事务中获得行级锁取的记录
        /// </summary>
        /// <param name="Where">Where条件</param>
        /// <param name="lockSEC">需要锁行的时间</param>
        /// <returns></returns>
        List<T> LockTable<T>(string where,int lockSEC) where T : DBBaseEntity, new();
    }
}
