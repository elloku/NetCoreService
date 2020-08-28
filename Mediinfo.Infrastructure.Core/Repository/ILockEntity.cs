using Mediinfo.Infrastructure.Core.Entity;

using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 锁定实体
    /// </summary>
    public interface ILockEntity
    {
        /// <summary>
        /// 锁定实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        List<T> LockTable<T>(string where, int waitTime) where T : EntityBase;
    }
}
