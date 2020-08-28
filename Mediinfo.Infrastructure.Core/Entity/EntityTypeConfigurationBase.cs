using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;

namespace Mediinfo.Infrastructure.Core.Entity
{
    /// <summary>
    /// domain个性化配置基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityTypeConfigurationBase<TEntity> : EntityTypeConfiguration<TEntity> where TEntity : EntityBase
    { 
        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityTypeConfigurationBase()
        {
            var properties = typeof(TEntity).GetProperties().ToList().Where(p => p.PropertyType == typeof(string)).ToList();
            foreach (var o in properties)
            {
                var notMappedAttrs = (NotMappedAttribute[])o.GetCustomAttributes(typeof(NotMappedAttribute), false);
                if(notMappedAttrs.Count() > 0)
                {
                    continue;
                }

                ParameterExpression param = Expression.Parameter(typeof(TEntity), "t1");
                var body = Expression.Property(param, o.Name);
                Expression<Func<TEntity, string>> expr = Expression.Lambda<Func<TEntity, string>>(body, param);
                this.Property(expr)
                .HasColumnType("VARCHAR2");
            };

            var dateProperties = typeof(TEntity).GetProperties().ToList().Where(p => p.PropertyType == typeof(DateTime)).ToList();
            foreach (var o in dateProperties)
            {
                var notMappedAttrs = (NotMappedAttribute[])o.GetCustomAttributes(typeof(NotMappedAttribute), false);
                if (notMappedAttrs.Count() > 0)
                {
                    continue;
                }
                ParameterExpression param = Expression.Parameter(typeof(TEntity), "t1");
                var body = Expression.Property(param, o.Name);
                Expression<Func<TEntity, DateTime>> expr = Expression.Lambda<Func<TEntity, DateTime>>(body, param);
                this.Property(expr)
                .HasColumnType("DATE");
            };

            var nulladbleDateProperties = typeof(TEntity).GetProperties().ToList().Where(p => p.PropertyType == typeof(DateTime?)).ToList();
            foreach (var o in nulladbleDateProperties)
            {
                var notMappedAttrs = (NotMappedAttribute[])o.GetCustomAttributes(typeof(NotMappedAttribute), false);
                if (notMappedAttrs.Count() > 0)
                {
                    continue;
                }
                ParameterExpression param = Expression.Parameter(typeof(TEntity), "t1");
                var body = Expression.Property(param, o.Name);
                Expression<Func<TEntity, DateTime?>> expr = Expression.Lambda<Func<TEntity, DateTime?>>(body, param);
                this.Property(expr)
                .HasColumnType("DATE");
            };
        }
    }
}
