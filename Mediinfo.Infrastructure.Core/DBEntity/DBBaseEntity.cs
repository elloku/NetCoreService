using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Infrastructure.Core.DBContext;
using Mediinfo.Infrastructure.Core.Domain;
using Mediinfo.Infrastructure.Core.EventBus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.DBEntity
{
    //class GY_XuHao
    //{
    //    public string XULIEMC { get; set; }
    //    public int DANGQIANZHI { get; set; }
    //    public int? CHANGDU { get; set; }
    //    public string ZUHEFS { get; set; }
    //}
    /// <summary>
    /// ORM实体继承此类
    /// </summary>
    public class DBBaseEntity : DBBaseMap
    {
        [NotMapped]
        public DBContextBase DBContext { get; set; }
        [NotMapped]
        public ServiceContext ServiceContext { get; set; }

        public void Init(DBContextBase dbContext, ServiceContext serviceContext)
        {
            DBContext = dbContext;
            ServiceContext = serviceContext;
        }
        public DateTime? GetSYSDate()
        {
            return DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault().Date;
        }
        public DateTime? GetSYSTime()
        {
            return DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault();
        }
        public List<string> GetOrder(string XuHaoMC, string QianZhui = null, int Count = 1)
        {
            if (QianZhui == null)
            {
                QianZhui = string.Empty;
            }
            if (String.IsNullOrWhiteSpace(XuHaoMC))
            {
                throw new DomainException("序号名称不能为空");
            }
            bool isYaoFang = false;
            int val = 0;
            if (XuHaoMC.Length > 3 && int.TryParse(XuHaoMC.Substring(1, 2), out val))
            {
                isYaoFang = true;
            }
            List<string> ids = new List<string>();
            GY_XuHao XuHao = null;

            XuHao = DBContext.Database.SqlQuery<GY_XuHao>("select XULIEMC, DANGQIANZHI,ZUHEFS,CHANGDU from GY_XUHAO where XUHAOMC = :Name for update wait 1", XuHaoMC).FirstOrDefault();
            if (XuHao == null)
            {
                if (isYaoFang)
                {
                    var success = DBContext.Database.ExecuteSqlCommand("insert into GY_XUHAO (XUHAOMC, DANGQIANZHI, ZUIXIAOZHI, ZUIDAZHI, CHANGDU, ZUHEFS) values (:Name ,:ZUIXIAOZHI ,1 ,999999 ,12 ,'1')", XuHaoMC, Count);
                    if (success < 0)
                    {
                        throw new DBException("序号表新增失败");
                    }
                    else
                    {
                        XuHao = new GY_XuHao()
                        {
                            DANGQIANZHI = Count,
                            CHANGDU = 12,
                            ZUHEFS = "1"
                        };
                        for (int i = 1; i <= Count; i++)
                        {
                            ids.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    throw new DomainException("序号名称在序号表中不存在");
                }
            }
            else
            {
                if (isYaoFang)
                {
                    var success = DBContext.Database.ExecuteSqlCommand("update GY_XUHAO set DANGQIANZHI = :DANGQIANZHI where XUHAOMC = :Name", XuHao.DANGQIANZHI + Count, XuHaoMC);
                    if (success < 0)
                    {
                        throw new DBException("序号表更新失败");
                    }
                    else
                    {
                        for (int i = XuHao.DANGQIANZHI + 1; i <= XuHao.DANGQIANZHI + Count; i++)
                        {
                            ids.Add(i.ToString());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        ids.Add(DBContext.Database.SqlQuery<Decimal>(string.Format(sqlSEQ, XuHao.XULIEMC)).FirstOrDefault().ToString());
                    }
                }
            }
            List<string> IDS = new List<string>();
            if ((XuHao.ZUHEFS == null || XuHao.ZUHEFS == "0") && !XuHao.CHANGDU.HasValue)
            {
                return ids;
            }
            else if ((XuHao.ZUHEFS == null || XuHao.ZUHEFS == "0") && XuHao.CHANGDU.HasValue)
            {
                ids.ForEach(o =>
                {
                    if (o.Length <= XuHao.CHANGDU)
                    {
                        IDS.Add(o.PadLeft(XuHao.CHANGDU.Value, '0'));
                    }
                });
                return IDS;
            }
            else if (XuHao.ZUHEFS != null && XuHao.ZUHEFS != "0" && (!XuHao.CHANGDU.HasValue || XuHao.CHANGDU.Value == 0))
            {
                ids.ForEach(o =>
                {
                    IDS.Add(QianZhui + o);
                });
                return IDS;
            }
            else if (XuHao.ZUHEFS != null && XuHao.ZUHEFS != "0" && XuHao.CHANGDU.HasValue)
            {
                ids.ForEach(o =>
                {
                    if ((o.Length + QianZhui.Length) > XuHao.CHANGDU)
                    {
                        throw new DomainException("序号超出定义的长度");
                    }
                    else if ((o.Length + QianZhui.Length) == XuHao.CHANGDU)
                    {
                        IDS.Add(QianZhui + o);
                    }
                    else
                    {
                        IDS.Add(QianZhui + o.PadLeft(XuHao.CHANGDU.Value - QianZhui.Length, '0'));
                    }
                });
                return IDS;
            }
            return ids;
        }

        [NotMapped]
        const string sqlSEQ = "Select {0}.nextval from dual";

        /// <summary>
        /// 立即发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void SendMessageImmediately<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.Add<T>(args);
        }
        /// <summary>
        /// 等待提交时发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void SendMessage<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.AddWaitCommit<T>(args);
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
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }

        }

        public virtual List<T> Update<T>(T parm, Expression<Func<T, bool>> where) where T : EntityBase, new()
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
            typeof(T).GetProperties().Where(o => !KeyValues.ContainsKey(o.Name) && o.GetCustomAttribute(typeof(NotMappedAttribute)) == null && !o.PropertyType.IsGenericType && !o.PropertyType.IsArray).ToList().ForEach(o =>
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

        public void SaveChanges()
        { 
            DBContext.SaveChanges();
        }

        public void Delete()
        {
            DBContext.Entry(this).State = EntityState.Deleted;
        }
    }
}