using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Infrastructure.Core.DBContext;
using Mediinfo.Infrastructure.Core.DBEntity;
using Mediinfo.Infrastructure.Core.EventBus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Domain
{
    public abstract class DomainBase : DomainBaseOption, IDomainBaseOption, IDomainBase
    {
        public void SendMessage<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.Add<T>(args);
        }

        public void SendMessageAfterCommit<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.AddWaitCommit<T>(args);
        }

        public DomainBase(DBContextBase dbContext = null, ServiceContext serviceContext = null) : base(dbContext, serviceContext)
        { 

        }

        public virtual T Insert<T>(T parm) where T : DBBaseEntity, new()
        {
            try
            {
                DBEntityPropertys.GetKeys(typeof(T)).ForEach(o =>
                {
                    if (o.GetValue(parm) == null)
                    {
                        throw new DBException("主键不能为空");
                    }
                });
                DBContext.Set<T>().Add(parm);
                DBContext.SaveChanges();
                return parm;
            }
            catch(DbEntityValidationException ex)
            {
                throw ex;
            }
          
        }

        public virtual List<T> Update<T>(T parm, Expression<Func<T, bool>> where) where T : DBBaseEntity, new()
        {
            var UpdateList = DBContext.Set<T>().Where(where).ToList();
            UpdateList.ForEach(o =>
            {
                o.MargeDB<T>(parm);
            });
            DBContext.SaveChanges();
            return UpdateList;
        }

        public virtual T Update<T>(T parm) where T : DBBaseEntity, new()
        {
            //try
            //{
 
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            DBEntityPropertys.GetKeys(typeof(T)).ForEach(o =>
            {
                KeyValues.Add(o.Name, o.GetValue(parm));
                if (KeyValues[o.Name] == null)
                {
                    throw new DBException("主键不能为空");
                }
            });
            DbEntityEntry<T> entry = DBContext.Entry<T>(parm);
            if (entry.State == EntityState.Detached)
            {
                DBContext.Set<T>().Attach(parm);
            }
            typeof(T).GetProperties().Where(o => !KeyValues.ContainsKey(o.Name)).ToList().ForEach(o =>
            {
                if (o.GetValue(parm) != null)
                {
                    entry.Property(o.Name).IsModified = true;
                }
            });
            DBContext.SaveChanges();
            return parm;

        //}
        //    catch(DbEntityValidationException ex)
        //    {
        //        throw ex;
        //    }
}

        public virtual int Delete<T>(T parm) where T : DBBaseEntity, new()
        {
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            DBEntityPropertys.GetKeys(typeof(T)).ForEach(o =>
            {
                KeyValues.Add(o.Name, o.GetValue(parm));
                if (KeyValues[o.Name] == null)
                {
                    throw new DBException("主键不能为空");
                }
            });
            var where = string.Empty;
            if (KeyValues.Count == 0)
            {
                throw new DBException("没有主键的表无法使用该方法");
            }
            KeyValues.ToList().ForEach(o =>
            {
                where += o.Key + "='" + o.Value.ToString() + "' and ";
            });
            where = where.Remove(where.Length - 4);
            var attr = typeof(T).GetCustomAttribute(typeof(TableAttribute));
            if (attr == null)
            {
                throw new DBException(typeof(T).Name + "类未设置TableAttribute");
            }
            var sql = string.Format("delete from {0} where {1}", (attr as TableAttribute).Name, where);
            var seccuess = DBContext.Database.ExecuteSqlCommand(sql);
            if (seccuess < 0)
            {
                throw new DBException("数据未能删除");
            }
            return seccuess;
        }

        public T GetByKey<T>(T parm) where T : DBBaseEntity, new()
        {
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            DBEntityPropertys.GetKeys(typeof(T)).ForEach(o =>
            {
                KeyValues.Add(o.Name, o.GetValue(parm));
                if (KeyValues[o.Name] == null)
                {
                    throw new DBException("主键不能为空");
                }
            });
            var where = string.Empty;
            if (KeyValues.Count == 0)
            {
                throw new DBException("没有主键的表无法使用该方法");
            }
            KeyValues.ToList().ForEach(o =>
            {
                where += o.Key + "='" + o.Value.ToString() + "' and ";
            });
            where = where.Remove(where.Length - 4);
            return DBContext.Database.SqlQuery<T>("select * from {0} where {1}", typeof(T).GetType().Name, where).FirstOrDefault();
        }
        /// <summary>
        /// 中等
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public List<T> GetByExpression<T>(Expression<Func<T, bool>> where) where T : DBBaseEntity, new()
        {
            return DBContext.Set<T>().Where(where).ToList();
        }
        /// <summary>
        /// 危险级
        /// </summary>
        /// <param name="where"></param>
        /// <param name="lockSEC"></param>
        /// <returns></returns>
        public List<T> LockTable<T>(string where, int lockSEC) where T : DBBaseEntity, new()
        {
            string sql = string.Format("select * from {0} where {1} for update wait {2}", typeof(T).Name, where, lockSEC);
            return DBContext.Database.SqlQuery<T>(sql).ToList();
        }
    }
}
