using Mediinfo.Enterprise.Config;
using Mediinfo.Enterprise.PagedResult;
using Mediinfo.Infrastructure.Core.Cache;
using Mediinfo.Infrastructure.Core.DBContext;
using Mediinfo.Infrastructure.Core.Entity;
using Mediinfo.Infrastructure.Core.MessageQueue;
using Mediinfo.Infrastructure.Core.Repository;
using Mediinfo.Utility.Reflection;

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Mediinfo.Infrastructure.Core.UnitOfWork
{

    /// <summary>
    /// 工作单元基类
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork, IRepositoryContext
    {
        // 消息插件
        public IMessagePlugin MessagePlugin { get; set; }
        public Messager CurrentMessager { get; set; } = new Messager();
        public Guid UnitOfWorkID { get; set; }
        public StringBuilder SqlLog { get; set; } = new StringBuilder();

        public static bool DebugSql { get; set; } = false;

        public DbTransaction CurrentDbTransaction
        {
            get
            {
                if (currentTrans != null)
                    return currentTrans.UnderlyingTransaction;

                return null;
            }
        }

        protected DBContextBase dbContext = null;
        protected DbContextTransaction currentTrans = null;
        public ContextCache contextCache = null;
        private long ticks = 0;

        //public UnitOfWorkBase(ContextCache contextCache)
        //{
        //    this.contextCache = contextCache;
        //}

        //public UnitOfWorkBase(DbContext dbcontext)
        //{
        //    dbContext = dbcontext;
        //    ticks = DateTime.Now.Ticks;
        //}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contextCache"></param>
        /// <param name="dbcontext"></param>
        public UnitOfWorkBase(ContextCache contextCache, DBContextBase dbcontext)
        {
            dbContext = dbcontext;
            this.contextCache = contextCache;
            ticks = DateTime.Now.Ticks;
            // 记录sql
            dbContext.Database.Log = (sql) =>
            {
                if (string.IsNullOrEmpty(sql) == false)
                {
                    SqlLog.AppendLine(sql);
                    if (DebugSql)
                    {
                        Debug.WriteLine(sql);
                    }
                }
            };
        }

        public void Detach<T>(T o) where T : EntityBase
        {
            if (o != null)
            {
                dbContext.Entry<T>(o).State = EntityState.Detached;
            }
        }

        public void Attach<T>(T o) where T : EntityBase
        {
            dbContext.Set<T>().Attach(o);
        }

        //protected virtual void CacheInitialize()
        //{

        //    iCacheManager = CacheFactory.Build("UnitOfWork", settings =>
        //    {
        //        settings.WithSystemRuntimeCacheHandle("UnitOfWork")
        //                .WithExpiration(ExpirationMode.None, TimeSpan.FromSeconds(60));
        //    });
        //}

        /// <summary>
        /// 开启事务
        /// </summary>
        public virtual void BeginTransaction()
        {
            if (currentTrans != null)
                currentTrans.Dispose();

            currentTrans = this.dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// 加入事务
        /// </summary>
        /// <param name="dbTransaction"></param>
        public virtual void UseTransaction(DbTransaction dbTransaction)
        {
            this.dbContext.Database.UseTransaction(dbTransaction);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public virtual void Commit()
        {
            if (this.currentTrans != null)
            {
                // 处理消息
                this.MessagePlugin?.Handler(dbContext);

                this.currentTrans.Commit();
                this.currentTrans.Dispose();
                this.currentTrans = null;
            }
        }
        
        /// <summary>
        /// 检查是否存在文件夹
        /// </summary>
        public void change()
        {
            string path = System.Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\TextMessage.txt";
            if (!Directory.Exists(System.Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd")))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd"));
            }
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(@"" + DateTime.Now.ToString("yyyy-MM-dd") + "\\TextMessage.txt");
                fs.Close();
            }
        }

        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="value"></param>
        public void text(string value)
        {
            change();
            string path = System.Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\TextMessage.txt";
            FileStream f = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine(value);
            sw.Flush();
            sw.Close();
            f.Close();
        }

        // 需要跟踪的表列表
        protected List<string> IgnoreEntityEntityList = MediinfoConfig.GetValue("SystemConfig.xml", "IgnoreEntityToSave").Split(',').ToList();

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        public virtual int SaveChanges()
        {

            try
            {

                // 发送消息
                Messager.SendMessage(dbContext, CurrentMessager);
                if (MessagePlugin != null)
                {
                    MessagePlugin.ApplyChange();
                }

                ObjectContext objectContext = ((IObjectContextAdapter)this.dbContext).ObjectContext;
                // 获取变化的实体信息
                var changeList = objectContext.ObjectStateManager
                                          .GetObjectStateEntries(EntityState.Added | EntityState.Modified | System.Data.Entity.EntityState.Deleted)
                                          .Select(c => new EntityInfo
                                          {
                                              TableName = this.GetTableName(c.Entity),
                                              StateEntity = c,
                                              ColumnList = GetEntityInfo(c)
                                          })
                                          .Where(c => !string.IsNullOrWhiteSpace(c.TableName))
                                          .ToList();

                foreach (var item in changeList)
                {
                    // 判断该表是否忽略保存
                    if (IgnoreEntityEntityList.Contains(item.TableName))
                    {
                        item.StateEntity.AcceptChanges();
                    }
                }
                return this.dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        text(string.Format("Class: {0}, Property: {1}, Error: {2}", validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage));


                    }
                }
                throw dbEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 回滚
        /// </summary>
        public virtual void Rollback()
        {
            //if (null != contextCache)
            //    contextCache.Clear();

            if (this.currentTrans != null)
            {
                try
                {

                    this.currentTrans.Rollback();
                    this.currentTrans.Dispose();
                    this.currentTrans = null;

                }
                catch (Exception ex)
                {
                    Enterprise.Log.LogHelper.Intance.Error("系统日志", "Rollback发生错误", ex.ToString());
                }
                finally
                {
                    //this.currentTrans.Dispose();
                    this.currentTrans = null;
                }
            }
        }

        #region 析构

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.currentTrans != null)
                    {
                        this.currentTrans.Rollback();
                        this.currentTrans.Dispose();
                    }

                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }

                    //if (null != contextCache)
                    //{
                    //    contextCache.Clear();
                    //    contextCache.Dispose();
                    //}
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 缓存

        public virtual T GetFromCache<T>(string cacheKey) where T : class
        {
            if (null == this.contextCache)
                return default(T);

            return this.contextCache.GetFromCache<T>(cacheKey);
        }

        public virtual bool AddToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.contextCache)
                return false;

            return this.contextCache.AddToCache<T>(cacheKey, cacheObject);
        }

        public virtual void PutToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.contextCache)
                return;

            this.contextCache.PutToCache<T>(cacheKey, cacheObject);
        }

        public virtual T UpdateToCache<T>(string cacheKey, object cacheObject) where T : class
        {
            if (null == this.contextCache)
                return default(T);

            object o = this.contextCache.UpdateToCache<T>(cacheKey, cacheObject);
            if (null == o)
                return default(T);

            return (T)o;
        }

        public virtual bool RemoveFromCache<T>(string cacheKey) where T : class
        {
            if (null == this.contextCache)
                return true;

            return this.contextCache.RemoveFromCache<T>(cacheKey);
        }

        public virtual bool ExistInCache<T>(string cacheKey) where T : class
        {
            if (null == this.contextCache)
                return false;

            return this.contextCache.ExistInCache<T>(cacheKey);
        }

        public virtual DbSet<T> GetSet<T>() where T : class
        {
            return this.dbContext.Set<T>();
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        public virtual int BulkSaveChanges(bool validateOnSaveEnabled = true, int bulkSize = 64)
        {
            // 发送消息
            Messager.SendMessage(dbContext, CurrentMessager);
            if (MessagePlugin != null)
            {
                MessagePlugin.ApplyChange();
            }
            return dbContext.BulkSaveChanges(validateOnSaveEnabled, bulkSize);
        }

        private DateTime sysDate = DateTime.MinValue;
        public virtual DateTime GetSYSTime()
        {
            if (sysDate == DateTime.MinValue)
            {
                sysDate = this.dbContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault();
            }
            return sysDate;
        }

        /// <summary>
        /// 实时取数的方法
        /// </summary>
        /// <param name="shiShi"></param>
        /// <returns></returns>
        public virtual DateTime GetSYSTime(bool shiShi)
        {
            if (sysDate == DateTime.MinValue || shiShi)
            {
                sysDate = this.dbContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault();
            }
            return sysDate;
        }

        public virtual List<string> GetOrder(string XuHaoMC, string QianZhui = null, int Count = 1)
        {
            return new List<string>();
        }

        #endregion

        #region 增删改查

        public virtual TEntity RegisterAdd<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            // 应用默认值
            entity.SetDefaultValue();

            this.dbContext.Set<TEntity>().Add(entity);

            if (autoSaveChanges)
                this.BulkSaveChanges();

            return entity;
        }

        public virtual IEnumerable<TEntity> RegisterAdd<TEntity>(IEnumerable<TEntity> entitys, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            foreach (var entity in entitys)
            {
                // 应用默认值
                entity.SetDefaultValue();
            }

            this.dbContext.Set<TEntity>().AddRange(entitys);

            if (autoSaveChanges)
            {
                this.BulkSaveChanges();
            }

            return entitys;
        }

        public virtual TEntity RegisterUpdate<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            var state = this.dbContext.Entry(entity).State;
            // if (this.dbContext.Entry(entity).State == EntityState.Detached)
            //   this.dbContext.Entry(entity).State = EntityState.Modified;
            if (this.dbContext.Entry(entity).State != EntityState.Modified)
            {
                this.dbContext.Entry(entity).State = EntityState.Modified;
            }

            if (autoSaveChanges)
            {
                this.BulkSaveChanges();
            }

            return entity;
        }

        public virtual IEnumerable<TEntity> RegisterUpdate<TEntity>(IEnumerable<TEntity> entitys, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            bool autoDetectChanges = this.dbContext.Configuration.AutoDetectChangesEnabled;

            if (autoDetectChanges)
                this.dbContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var item in entitys)
            {
                this.RegisterUpdate<TEntity>(item, false);
            }

            if (autoDetectChanges)
                this.dbContext.Configuration.AutoDetectChangesEnabled = true;

            if (autoSaveChanges)
            {

                this.BulkSaveChanges();
            }

            return entitys;
        }

        public virtual TEntity RegisterDelete<TEntity>(TEntity entity, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            // if (this.dbContext.Entry(entity).State == EntityState.Detached)
            //this.dbContext.Entry(entity).State = EntityState.Deleted;

            var state = this.dbContext.Entry(entity).State;

            this.dbContext.Entry(entity).State = EntityState.Deleted;


            if (autoSaveChanges)
            {
                this.BulkSaveChanges();
            }

            return entity;
        }

        public virtual IEnumerable<TEntity> RegisterDelete<TEntity>(IEnumerable<TEntity> entitys, bool autoSaveChanges = false) where TEntity : EntityBase
        {
            bool autoDetectChanges = this.dbContext.Configuration.AutoDetectChangesEnabled;

            if (autoDetectChanges)
                this.dbContext.Configuration.AutoDetectChangesEnabled = false;

            foreach (var item in entitys)
            {
                this.RegisterDelete<TEntity>(item, false);
            }

            if (autoDetectChanges)
                this.dbContext.Configuration.AutoDetectChangesEnabled = true;

            if (autoSaveChanges)
            {
                this.BulkSaveChanges();
            }

            return entitys;
        }

        #endregion

        #region Raw Sql

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql</param>
        /// <param name="sort">排序字段（可为空）</param>
        /// <param name="pageSize">页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public virtual IPagedResult<TEntity> PagedQuery<TEntity>(string sql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "") where TEntity : class
        {
            return this.dbContext.PagedQuery<TEntity>(sql, parameters, pageIndex, pageSize, sort);
        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters) where TEntity : class
        {
            return this.dbContext.Database.SqlQuery<TEntity>(sql, parameters);
        }

        public IEnumerable<TEntity> SqlQuery<TEntity>(string sql, Dictionary<string, object> parameters) where TEntity : class
        {
            List<DbParameter> paraList = new List<DbParameter>();

            var cmd = this.dbContext.Database.Connection.CreateCommand();

            parameters.ToList().ForEach(o =>
            {
                var parm = cmd.CreateParameter();
                parm.ParameterName = o.Key;
                parm.Value = o.Value;

                if (o.Value.GetType() == typeof(DateTime) || o.Value.GetType() == typeof(DateTime?))
                {
                    parm.DbType = DbType.Date;
                }

                paraList.Add(parm);
            });

            // paraList.Clear();

            var result = this.dbContext.Database.SqlQuery<TEntity>(sql, paraList.ToArray()).ToList();
            paraList.Clear();
            return result;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return this.dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        #endregion

        /// <summary>
        /// 锁表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public virtual List<T> LockTable<T>(string where, int waitTime) where T : EntityBase
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            if (this.dbContext.Database.Connection.State != ConnectionState.Open)
                this.dbContext.Database.Connection.Open();
            var cmd = this.dbContext.Database.Connection.CreateCommand();
            cmd.CommandText = sql;

            DataTable table = new DataTable();
            var read = cmd.ExecuteReader();
            for (int i = 0; i < read.FieldCount; i++)
            {
                // 创建table的第一列
                DataColumn Column = new DataColumn();
                // 该列的数据类型
                Column.DataType = read.GetFieldType(i);
                // 该列得名称
                Column.ColumnName = read.GetName(i);
                table.Columns.Add(Column);
            }

            while (read.Read())
            {
                var row = table.NewRow();
                for (int i = 0; i < read.FieldCount; i++)
                {
                    if (read[i] != DBNull.Value)
                    {
                        row[i] = read[i];
                    }
                }
                table.Rows.Add(row);
            }

            return table;
        }

        public IPagedTableResult GetPagedDataTable(string sql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "")
        {
            return this.dbContext.PagedTableQuery(sql, parameters, pageIndex, pageSize, sort);
        }

        public void ExecuteProc(string procName, params DbParameter[] dbParameter)
        {
            this.dbContext.ExecuteProc(procName, dbParameter);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="yingyongId"></param>
        /// <param name="canShuId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetCanShu(string yingyongId, string canShuId, string defaultValue)
        {
            return defaultValue;

            //if (string.IsNullOrWhiteSpace(canShuId))
            //    throw new DomainException("参数ID为空！");

            //if (string.IsNullOrWhiteSpace(yingyongId))
            //    yingyongId = "00";

            ////先从缓存里面取
            //string canShuZhi = "";

            ////if (HISCacheManager.GetCanShu(yingYongId, canShuId, ref canShuZhi))
            ////    return canShuZhi;

            ////取所有应用的参数
            //var canShuList = this.dbContext.Set<GY_CANSHU>()
            //                    .Where(c => c.CANSHUID == canShuId)
            //                    .Select(c => (new
            //                    {
            //                        c.CANSHUID,
            //                        c.CANSHUZHI,
            //                        c.YINGYONGID
            //                    })).ToList();

            //var canShu = canShuList.Where(c => c.YINGYONGID == yingYongId).FirstOrDefault();

            //if (null != canShu)
            //{
            //    return canShu.CANSHUZHI;
            //}
            //else if (yingYongId == "00") //如果应用ID为空，则表示应用ID没有传入
            //{
            //    return defaultValue;
            //}

            //canShu = canShuList.Where(c => c.YINGYONGID == yingYongId.Substring(0, 2)).FirstOrDefault();

            //if (null != canShu)
            //{
            //    return canShu.CANSHUZHI;
            //}

            //canShu = canShuList.Where(c => c.YINGYONGID == "00").FirstOrDefault();

            //if (null != canShu)
            //{
            //    return canShu.CANSHUZHI;
            //}

            //return defaultValue;

            // throw new NotImplementedException();
        }

        #region 批量操作

        // 缓存模型中的字段排序
        private Dictionary<string, Dictionary<string, int>> EntityOrdinal = new Dictionary<string, Dictionary<string, int>>();
        private List<ColumnInfo> GetEntityInfo(ObjectStateEntry objectEntry)
        {
            var type = objectEntry.Entity.GetType();

            ColumnInfo columnInfo = null;
            List<ColumnInfo> columnList = new List<ColumnInfo>();

            // 获取变化的列
            List<string> changeColumns = objectEntry.GetModifiedProperties().ToList();

            // 历史值列表
            object[] originalValues = null;
            // 当前值列表
            object[] currentValues = null;
            foreach (var item in type.GetProperties())
            {
                if (item.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                    continue;

                columnInfo = new ColumnInfo();

                // 物理列名
                var colatts = (ColumnAttribute[])item.GetCustomAttributes(typeof(ColumnAttribute), false);
                if (colatts.Count() > 0 && !string.IsNullOrWhiteSpace(colatts[0].Name))
                    columnInfo.DbName = colatts[0].Name;
                else
                    columnInfo.DbName = item.Name;

                // 主键
                var keyatts = (KeyAttribute[])item.GetCustomAttributes(typeof(KeyAttribute), false);
                if (keyatts.Count() > 0)
                    columnInfo.IsKey = true;
                else
                    columnInfo.IsKey = false;

                // 列名
                columnInfo.ColumnName = item.Name;

                // 数据类型
                columnInfo.DbType = GetOracleDbType(item.PropertyType);

                // 当前值
                if (objectEntry.State != EntityState.Deleted)
                {
                    var i = 0;
                    var name = objectEntry.EntitySet.ElementType.FullName;
                    if (EntityOrdinal.ContainsKey(name))
                    {
                        if (!EntityOrdinal[name].ContainsKey(item.Name))
                        {
                            i = objectEntry.CurrentValues.GetOrdinal(item.Name);

                            EntityOrdinal[name].Add(item.Name, i);
                        }
                        else
                        {
                            i = EntityOrdinal[name][item.Name];
                        }

                    }
                    else
                    {
                        i = objectEntry.CurrentValues.GetOrdinal(item.Name);

                        var ko = new Dictionary<string, int>();
                        ko.Add(item.Name, i);

                        EntityOrdinal.Add(name, ko);
                    }

                    if (currentValues == null)
                    {
                        currentValues = new object[objectEntry.CurrentValues.FieldCount];
                        objectEntry.CurrentValues.GetValues(currentValues);
                    }

                    columnInfo.CurrentValue = currentValues[i];
                }

                // 原始值
                if (objectEntry.State != EntityState.Added)
                {
                    var i = 0;
                    var name = objectEntry.EntitySet.ElementType.FullName;
                    if (EntityOrdinal.ContainsKey(name))
                    {
                        if (!EntityOrdinal[name].ContainsKey(item.Name))
                        {
                            i = objectEntry.OriginalValues.GetOrdinal(item.Name);
                            EntityOrdinal[name].Add(item.Name, i);
                        }
                        else
                        {
                            i = EntityOrdinal[name][item.Name];
                        }

                    }
                    else
                    {
                        i = objectEntry.OriginalValues.GetOrdinal(item.Name);

                        var ko = new Dictionary<string, int>();
                        ko.Add(item.Name, i);

                        EntityOrdinal.Add(name, ko);
                    }

                    if (originalValues == null)
                    {
                        originalValues = new object[objectEntry.OriginalValues.FieldCount];
                        objectEntry.OriginalValues.GetValues(originalValues);
                    }

                    columnInfo.OriginalValue = originalValues[i];

                }

                // 值是否被修改
                if (objectEntry.State == EntityState.Modified)
                {
                    if (changeColumns.Contains(columnInfo.ColumnName))
                        columnInfo.IsChanged = true;
                    else
                        columnInfo.IsChanged = false;
                }

                columnList.Add(columnInfo);
            }
            return columnList;
        }

        private OracleDbType GetOracleDbType(Type type)
        {
            OracleDbType dataType = OracleDbType.Varchar2;

            if (type == typeof(string))
            {
                dataType = OracleDbType.Varchar2;
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                dataType = OracleDbType.Date;
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                dataType = OracleDbType.Int32;
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                dataType = OracleDbType.Int32;
            }
            else if (type == typeof(long) || type == typeof(long?) || type == typeof(Int64) || type == typeof(Int64?))
            {
                dataType = OracleDbType.Int64;
            }
            else if (type == typeof(decimal) || type == typeof(decimal?) || type == typeof(double) || type == typeof(double?))
            {
                dataType = OracleDbType.Decimal;
            }
            else if (type == typeof(Guid))
            {
                dataType = OracleDbType.Varchar2;
            }
            else if (type == typeof(bool) || type == typeof(bool?) || type == typeof(Boolean) || type == typeof(Boolean?))
            {
                dataType = OracleDbType.Byte;
            }
            else if (type == typeof(byte[]))
            {
                dataType = OracleDbType.Blob;
            }
            else if (type == typeof(char))
            {
                dataType = OracleDbType.Char;
            }
            return dataType;
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="cmd"></param>
        private void WriteSqlLog(DBContextBase dBContextBase, DbCommand cmd)
        {
            if (dBContextBase.Database.Log == null) return;

            StringBuilder builder = new StringBuilder(256);
            builder.AppendLine(cmd.CommandText);

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                var parameter = cmd.Parameters[i];

                builder.Append("-- ")
                    .Append(parameter.ParameterName);

                if (null != parameter.Value && parameter.Value.GetType() == typeof(object[]))
                {
                    object[] values = (object[])parameter.Value;

                    builder.Append(": ");
                    foreach (var item in values)
                    {
                        builder.Append("'")
                            .Append((item == null || item == DBNull.Value) ? "null" : item)
                            .Append(",");
                    }

                    if (values.Length > 0)
                        builder.Remove(builder.Length - 1, 1);

                    builder.Append("' (Type = ")
                            .Append(parameter.DbType);
                }
                else
                {
                    builder.Append(": '")
                            .Append((parameter.Value == null || parameter.Value == DBNull.Value) ? "null" : parameter.Value)
                            .Append("' (Type = ")
                            .Append(parameter.DbType);
                }

                if (parameter.Direction != ParameterDirection.Input)
                {
                    builder.Append(", Direction = ").Append(parameter.Direction);
                }

                if (!parameter.IsNullable)
                {
                    builder.Append(", IsNullable = false");
                }

                if (parameter.Size != 0)
                {
                    builder.Append(", Size = ").Append(parameter.Size);
                }

                if (((IDbDataParameter)parameter).Precision != 0)
                {
                    builder.Append(", Precision = ").Append(((IDbDataParameter)parameter).Precision);
                }

                if (((IDbDataParameter)parameter).Scale != 0)
                {
                    builder.Append(", Scale = ").Append(((IDbDataParameter)parameter).Scale);
                }

                builder.Append(")").Append(Environment.NewLine);
            }

            dBContextBase.Database.Log(builder.ToString());
        }

        /// <summary>
        /// 获取实体的物理表表名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetTableName(object entity)
        {
            var type = entity.GetType();

            TableAttribute[] table = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), false);

            if (table.Length > 0)
                return table[0].Name;
            else
                return string.Empty;
        }

        /// <summary>
        /// 获取数据库的事务信息
        /// </summary>
        /// <param name="objectContext"></param>
        /// <returns></returns>
        private Tuple<DbConnection, DbTransaction> GetStore(ObjectContext objectContext)
        {
            DbConnection dbConnection = objectContext.Connection;
            var entityConnection = dbConnection as EntityConnection;

            if (entityConnection == null)
                return new Tuple<DbConnection, DbTransaction>(dbConnection, null);

            DbConnection connection = entityConnection.StoreConnection;

            // 动态获取当前的数据库事务
            dynamic connectionProxy = new DynamicProxy(entityConnection);
            dynamic entityTransaction = connectionProxy.CurrentTransaction;
            if (entityTransaction == null)
                return new Tuple<DbConnection, DbTransaction>(connection, null);

            DbTransaction transaction = entityTransaction.StoreTransaction;
            return new Tuple<DbConnection, DbTransaction>(connection, transaction);
        }

        #endregion
    }

    #region 数据库批量操作辅助类

    /// <summary>
    /// 列信息
    /// </summary>
    internal class ColumnInfo
    {
        public ColumnInfo()
        {
            IsChanged = false;
            IsKey = false;
        }

        public string ColumnName { get; set; }

        public string DbName { get; set; }

        public OracleDbType DbType { get; set; }
        public object CurrentValue { get; set; }
        public object OriginalValue { get; set; }

        public bool IsKey { get; set; }

        public bool IsChanged { get; set; }

        public bool ConcurrencyCheck { get; set; }

        //public string WhereSql(bool ori)
        //{
        //    return string.Format("{0}{1}{2}",DbName,()
        //}
    }

    /// <summary>
    /// 实体信息
    /// </summary>
    internal class EntityInfo
    {
        public string TableName { get; set; }

        public ObjectStateEntry StateEntity { get; set; }

        public List<ColumnInfo> ColumnList { get; set; }
    }

    #endregion
}