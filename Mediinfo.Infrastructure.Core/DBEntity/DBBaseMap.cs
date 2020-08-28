using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.DBEntity
{
    /// <summary>
    /// 注入Context的接口IEntityMapper的实现抽象类
    /// </summary>
    public abstract class DBBaseMap : IEntityMapper
    {
        public void RegistTo(ConfigurationRegistrar configurations)
        {
            Type type = typeof(EntityTypeConfiguration<>).MakeGenericType(this.GetType());
            object param = Activator.CreateInstance(type);
            //Type[] types = new Type[] { type };
            //var v = typeof(ConfigurationRegistrar).GetMethod("Add", types);
            typeof(ConfigurationRegistrar).GetMethods()[1]
                .MakeGenericMethod(new Type[] { this.GetType() })
                .Invoke(configurations, new object[]{ param });   
        }
    }
}
