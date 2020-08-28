using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.DBContext;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    /// <summary>
    /// 消息队列
    /// </summary>
    public class Messager
    {
        public Messager()
        {
            ID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public string FaSongSJ { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string MoKuaiMc { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string YeWuMc { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string CaoZuoMc { get; set; }

        /// <summary>
        /// 涉及到的实体列表
        /// </summary>
        public ISet<string> EntityNameList { get; set; } = new HashSet<string>();

        /// <summary>
        /// 环境上下文
        /// </summary>
        public ServiceContext Context { get; set; }

        /// <summary>
        /// 是否发送标识（默认实体为空的情况不发送）
        /// </summary>
        public bool IsPublish { get; set; }

        /// <summary>
        /// 新增实体列表
        /// </summary>
        public List<MessageEntity> AddedEntityList { get; set; } = new List<MessageEntity>();

        /// <summary>
        /// 删除实体列表
        /// </summary>
        public List<MessageEntity> DeletedEntityList { get; set; } = new List<MessageEntity>();

        /// <summary>
        /// 修改实体列表
        /// </summary>
        public List<MessageEntity> ModifiedEntityList { get; set; } = new List<MessageEntity>();

        /// <summary>
        /// 过滤列表
        /// </summary>
        public static List<string> DuiXingList = new List<string>();

        /// <summary>
        /// 待检测实体列表(用于消息重发)
        /// </summary>
        public static List<string> DaiJianCeSTList = new List<string>();

        /// <summary>
        /// 发送配置 0：默认不发送，1：默认全部发送
        /// </summary>
        public static int FaSongPz = 0;

        /// <summary>
        /// 是否初始化配置
        /// </summary>
        public static PeiZhiZt PeiZhiZt { get; set; } = PeiZhiZt.WeiPeiZhi;

        public static readonly object _lock = new object();

        public static void SendMessage(DBContextBase dbContext, Messager CurrentMessager)
        {
            //********************消息队列******************************

            var entries = dbContext.ChangeTracker.Entries();
            // 如果默认不发送，则启动允许列表
            if (FaSongPz == 0)
            {
                entries = entries.Where(m => DuiXingList.Contains(m.Entity.GetType().Name, StringComparer.InvariantCultureIgnoreCase));
            }
            else if (FaSongPz == 1) // 如果配置默认全部发送，则启动过滤列表
            {
                entries = entries.Where(m => !DuiXingList.Contains(m.Entity.GetType().Name.ToUpper()));
            }

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
                CurrentMessager.AddedEntityList.Add(messageEntity);
                CurrentMessager.EntityNameList.Add(messageEntity.EntityName);
                CurrentMessager.IsPublish = true;
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
                CurrentMessager.DeletedEntityList.Add(messageEntity);
                CurrentMessager.EntityNameList.Add(messageEntity.EntityName);
                CurrentMessager.IsPublish = true;
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
                CurrentMessager.ModifiedEntityList.Add(messageEntity);
                CurrentMessager.EntityNameList.Add(messageEntity.EntityName);
                CurrentMessager.IsPublish = true;
            }

            //***********************消息队列***************************
        }
    }

    public enum PeiZhiZt : int
    {
        WeiPeiZhi = 0,
        YiPeiZhi = 1
    }
}
