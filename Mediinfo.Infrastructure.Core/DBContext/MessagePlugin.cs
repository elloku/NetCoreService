using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Config;
using Mediinfo.Infrastructure.Core.DbMessage;

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mediinfo.Infrastructure.Core.DBContext
{
    public class MessagePlugin : IMessagePlugin
    {
        protected DbMessage.DbMessage dbMessage = new DbMessage.DbMessage();
        protected List<Assembly> AssemblyList { get; set; } = new List<Assembly>();
        // 数据库上下文
        protected DBContextBase dbContext = null;
        public ApiController apiController { get; set; }
        // 服务信息
        protected ServiceInfo serviceInfo = new ServiceInfo();
        protected string PluginDBConfig { get; set; } = MediinfoConfig.GetValue("DbConfig.xml", "HIS6");
        // 需要跟踪的表列表
        protected List<string> EntityList = MediinfoConfig.GetValue("SystemConfig.xml", "EntityListForHIS1").Split(',').ToList();
        // 默认构造函数
        public MessagePlugin(DBContextBase dbContext)
        {
            this.dbContext = dbContext;
            AssemblyListInit();
        }

        // 应用更改
        public int ApplyChange()
        {
            var entries = dbContext.ChangeTracker.Entries();
            // 过滤掉不需要发送的Entity
            entries = entries.Where(m => EntityList.Contains(m.Entity.GetType().Name, StringComparer.InvariantCultureIgnoreCase));

            // 获取新增实体
            var addedEntries = entries.Where(m => m.State == EntityState.Added);
            foreach (DbEntityEntry entity in addedEntries)
            {
                // 创建一个Messager实体
                MessageEntity messageEntity = new MessageEntity();
                messageEntity.EntityName = entity.Entity.GetType().Name;
                messageEntity.Type = OperationType.ADDED;
                messageEntity.Entity = entity.Entity;

                // 反射出entity中的所有属性
                var properties = entity.Entity.GetType().GetProperties();
                foreach (var propertie in properties)
                {
                    // 获取主键
                    if (propertie.GetCustomAttributes(true).Any(m => m.GetType() == typeof(KeyAttribute)))
                    {
                        messageEntity.KeyNameList.Add(propertie.Name);
                    }
                }

                // 记录当前值
                foreach (var pName in entity.CurrentValues.PropertyNames)
                {
                    messageEntity.CurrentValues.Add(pName, entity.CurrentValues[pName]);
                }

                // 保存到当前待发送队列
                dbMessage.AddedEntityList.Add(messageEntity);
                dbMessage.EntityNameList.Add(messageEntity.EntityName);
            }

            // 获取删除实体
            var deletedEntries = entries.Where(m => m.State == EntityState.Deleted);
            foreach (var entity in deletedEntries)
            {
                // 创建一个Messager实体
                MessageEntity messageEntity = new MessageEntity();
                messageEntity.EntityName = entity.Entity.GetType().Name;
                messageEntity.Type = OperationType.DELETED;
                messageEntity.Entity = entity.Entity;

                // 反射出entity中的所有属性
                var properties = entity.Entity.GetType().GetProperties();
                foreach (var propertie in properties)
                {
                    // 获取主键
                    if (propertie.GetCustomAttributes(true).Any(m => m.GetType() == typeof(KeyAttribute)))
                    {
                        messageEntity.KeyNameList.Add(propertie.Name);
                    }
                }

                // 记录历史值
                foreach (var pName in entity.OriginalValues.PropertyNames)
                {
                    messageEntity.OriginalValues.Add(pName, entity.OriginalValues[pName]);
                }

                // 保存到当前待发送队列
                dbMessage.DeletedEntityList.Add(messageEntity);
                dbMessage.EntityNameList.Add(messageEntity.EntityName);
            }

            // 获取修改实体
            var modifiedEntries = entries.Where(m => m.State == EntityState.Modified);
            foreach (var entity in modifiedEntries)
            {
                // 创建一个Messager实体
                MessageEntity messageEntity = new MessageEntity();
                messageEntity.EntityName = entity.Entity.GetType().Name;
                messageEntity.Type = OperationType.MODIFIED;
                messageEntity.Entity = entity.Entity;

                // 反射出entity中的所有属性
                var properties = entity.Entity.GetType().GetProperties();
                foreach (var propertie in properties)
                {
                    // 获取主键
                    if (propertie.GetCustomAttributes(true).Any(m => m.GetType() == typeof(KeyAttribute)))
                    {
                        messageEntity.KeyNameList.Add(propertie.Name);
                    }
                }

                // 记录当前值
                foreach (var pName in entity.CurrentValues.PropertyNames)
                {
                    messageEntity.CurrentValues.Add(pName, entity.CurrentValues[pName]);
                }
                // 记录历史值
                foreach (var pName in entity.OriginalValues.PropertyNames)
                {
                    messageEntity.OriginalValues.Add(pName, entity.OriginalValues[pName]);
                }

                // 保存到当前待发送队列
                dbMessage.ModifiedEntityList.Add(messageEntity);
                dbMessage.EntityNameList.Add(messageEntity.EntityName);
            }

            return dbMessage.AddedEntityList.Count +
                dbMessage.ModifiedEntityList.Count +
                dbMessage.DeletedEntityList.Count;
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="dbMessage"></param>
        public virtual void Handler(DBContextBase dbContext)
        {
            if (!IsCallHandler()) return;
            if (!dbMessage.Changed) return;
            
            // 获取服务信息
            string pathAndQuery = apiController.ActionContext.Request.RequestUri.PathAndQuery;
            string moKuaiMc = pathAndQuery.Split('/')[1];
            string yeWuMc = pathAndQuery.Split('/')[2];
            string caoZuoMc = pathAndQuery.Split('/')[3];

            var controller = apiController.ActionContext.ControllerContext.Controller;
            var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            ServiceContext serviceContext = serviceContextProperty.GetValue(controller) as ServiceContext;

            // 记录当前服务的基本信息
            serviceInfo.Context = serviceContext;
            serviceInfo.MoKuaiMc = moKuaiMc;
            serviceInfo.YeWuMc = yeWuMc;
            serviceInfo.CaoZuoMc = caoZuoMc;

            bool async = false;
            var asyncList = MediinfoConfig.GetValue("SystemConfig.xml", "AsyncInterfaceServiceList").Split(',');
            foreach (var item in asyncList)
            {
                var list = item.Split('/');
                if (list.Count() != 3) continue;
                var mokuaiming = list[0];
                var yewuming = list[1];
                var caozuoming = list[2];

                if ((mokuaiming == "*" || mokuaiming.ToUpper() == serviceInfo.MoKuaiMc.ToUpper()) &&
                    (yewuming == "*" || yewuming.ToUpper() == serviceInfo.YeWuMc.ToUpper()) &&
                    (caozuoming == "*" || caozuoming.ToUpper() == serviceInfo.CaoZuoMc.ToUpper())
                    )
                {
                    async = true;
                    break;
                }
                
            }

            if (async)
            {
                Task.Run(() =>
                {
                    OracleConnectionStringBuilder haloOraConn = new OracleConnectionStringBuilder(MediinfoConfig.GetValue("DbConfig.xml", "HIS6"));
                    OracleConnection haloconn = new OracleConnection();
                    haloconn.ConnectionString = haloOraConn.ConnectionString;
                    DBContextBase haloDbContext = new DBContextBase(haloconn, true, haloOraConn.UserID.ToUpper());

                    OracleConnectionStringBuilder pgOraConn = new OracleConnectionStringBuilder(PluginDBConfig);
                    OracleConnection pgconn = new OracleConnection();
                    pgconn.ConnectionString = pgOraConn.ConnectionString;
                    DBContextBase pgDbContext = new DBContextBase(pgconn, true, pgOraConn.UserID.ToUpper());

                    var haloTrans = haloDbContext.Database.BeginTransaction();
                    var pgTrans = pgDbContext.Database.BeginTransaction();

                    try
                    {
                        var pluginDll = Mediinfo.Enterprise.Config.MediinfoConfig.GetValue("SystemConfig.xml", "MessagePlugin");
                        var assembly = Assembly.Load(pluginDll);

                        var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDbMessageHandler))).ToArray();

                        var impls = types.Where(t => !t.IsInterface).ToList();

                        foreach (var impl in impls)
                        {
                            var implInstance = (IDbMessageHandler)assembly.CreateInstance(impl.FullName);
                            implInstance.Handler(haloDbContext, pgDbContext, serviceInfo, dbMessage);
                        }

                        haloTrans.Commit();
                        pgTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        haloTrans.Rollback();
                        pgTrans.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        haloDbContext.Dispose();
                        pgDbContext.Dispose();
                        dbMessage = new DbMessage.DbMessage();
                    }
                });
            }
            else
            {
                OracleConnectionStringBuilder pgOraConn = new OracleConnectionStringBuilder(PluginDBConfig);
                OracleConnection pgconn = new OracleConnection();
                pgconn.ConnectionString = pgOraConn.ConnectionString;
                DBContextBase pgDbContext = new DBContextBase(pgconn, true, pgOraConn.UserID.ToUpper());

                var pgTrans = pgDbContext.Database.BeginTransaction();

                try
                {
                    var pluginDll = Mediinfo.Enterprise.Config.MediinfoConfig.GetValue("SystemConfig.xml", "MessagePlugin");
                    var assembly = Assembly.Load(pluginDll);
                    var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDbMessageHandler))).ToArray();

                    var impls = types.Where(t => !t.IsInterface).ToList();

                    foreach (var impl in impls)
                    {
                        var implInstance = (IDbMessageHandler)assembly.CreateInstance(impl.FullName);
                        implInstance.Handler(dbContext, pgDbContext, serviceInfo, dbMessage);
                    }
                    
                    pgTrans.Commit();
                }
                catch (Exception ex)
                {
                    pgTrans.Rollback();
                    throw ex;
                }
                finally
                {
                    pgDbContext.Dispose();
                    dbMessage = new DbMessage.DbMessage();
                }
            }
        }

        /// <summary>
        /// 初始化程序集列表
        /// </summary>
        protected virtual void AssemblyListInit()
        {

        }
        /// <summary>
        /// add by hujian @2020/7/24  HR6-4389(604460) 急诊护士站--插件里面不触发！
        /// 自定义是否调用Handler
        /// </summary>
        /// <returns>true:调用,false:不调用</returns>
        protected virtual bool IsCallHandler()
        {
            return true;
        }
    }
}
