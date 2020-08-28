using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.Entity;
using Mediinfo.Infrastructure.Core.Repository;

using System.Collections.Generic;
using System.Linq;

namespace Mediinfo.Infrastructure.Core.DBEntity
{
    /// <summary>
    /// 返回带有上下文的实体
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// 扩展IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="iRepositoryBase"></param>
        /// <param name="serviceContext"></param>
        /// <returns></returns>
        public static T FirstOrDefaultWithContext<T>(this IQueryable<T> entity, IRepositoryBase iRepositoryBase, ServiceContext serviceContext)
            where T : EntityBase
        {
            var firstEnrity = entity.FirstOrDefault();
            if (firstEnrity != null)
            {
                firstEnrity.Initialize(iRepositoryBase, serviceContext);  
            }
            return firstEnrity;
        }

        //public static List<T> ToListWithContext<T>(this IQueryable<T> entity, IRepositoryBase iRepositoryBase, ServiceContext serviceContext)
        //    where T : EntityBase
        //{
        //    return WithContext<T>(entity.ToList(), iRepositoryBase, serviceContext);
        //}

        /// <summary>
        /// 扩展List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="iRepositoryBase"></param>
        /// <param name="serviceContext"></param>
        /// <returns></returns>
        public static List<T> WithContext<T>(this List<T> entity, IRepositoryBase iRepositoryBase, ServiceContext serviceContext)
            where T : EntityBase
        {
            if (entity == null)
                return null;

            entity.ForEach(m =>
            { 
                m.Initialize(iRepositoryBase, serviceContext);
            });

            return entity;
        }

        /// <summary>
        /// 扩展EntityBase
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="iRepositoryBase"></param>
        /// <param name="serviceContext"></param>
        /// <returns></returns>
        public static  T  WithContext<T>(this  T entity, IRepositoryBase iRepositoryBase, ServiceContext serviceContext)
           where T : EntityBase
        {
            if (entity == null)
                return null;
            entity.Initialize(iRepositoryBase, serviceContext);
           
            return entity;
        }
    }
}
