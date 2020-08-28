using Mediinfo.Enterprise.PagedResult;
using Mediinfo.Infrastructure.Core.EventBus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Mediinfo.Infrastructure.Core.DBContext
{
    /// <summary>
    /// EF DBContext基类
    /// </summary>
    public class DBContextBase : DbContext
    {
        public Action<ConnectionState> ConnectClose;
        
        protected string DbSchemaName { get; set; }

        protected IDBBulkOperations dBBulkOperations = null;

        protected IPagedQuery pagedQuery = null;
        protected IPagedTableQuery pagedTableQuery = null;

        #region 构造函数

        static DBContextBase()
        {
            // 关闭数据库初始化操作，防止自动更新数据
            Database.SetInitializer<DBContextBase>(null);
        }

        public DBContextBase(string nameOrConnectionString, string schemaName = "") : base(nameOrConnectionString)
        {
            Database.Connection.StateChange += (x, y) => {
                if (y.CurrentState == ConnectionState.Closed)
                {
                    if (ConnectClose != null)
                    {
                        ConnectClose(y.CurrentState);
                    }
                }
                else if (y.CurrentState == ConnectionState.Open)
                {
                    MessageEventBus.ArgsWait.Clear();
                }
            };
            DbSchemaName = schemaName;
        }

        public DBContextBase(DbConnection existingConnection, bool contextOwnsConnection = true, string schemaName = "") : base(existingConnection, contextOwnsConnection)
        {
            Database.Connection.StateChange += (x, y) => 
            {
                if (y.CurrentState == ConnectionState.Closed)
                {
                    if (ConnectClose != null)
                    {
                        ConnectClose(y.CurrentState);
                    }
                }
                else if (y.CurrentState == ConnectionState.Open)
                {
                    MessageEventBus.ArgsWait.Clear();
                }
            };
            DbSchemaName = schemaName;
        }
        
        #endregion 

        /// <summary>
        /// 加载modelBuilder
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void loadEntity(ModelBuilder modelBuilder)
        {
           
        }

        /// <summary>
        /// 重写OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                //base.Configuration.AutoDetectChangesEnabled = false;
                base.Configuration.ValidateOnSaveEnabled = true;

                // 禁用一对多级联删除
                modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

                // 禁用多对多级联删除
                modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

                // 禁用默认表名复数形式
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

                // 设置默认的SCHEMA
                if (!String.IsNullOrWhiteSpace(DbSchemaName))
                    modelBuilder.HasDefaultSchema(DbSchemaName);

                // 设置默认的decimal精度
                modelBuilder.Properties<decimal>().Configure(prop => prop.HasPrecision(22, 6));

                loadEntity(modelBuilder);

                base.OnModelCreating(modelBuilder);
        }
        
        /// <summary>
        /// 批量保存（新增，修改，删除），需要在具体的DBContext里面实现
        /// </summary>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        public virtual int BulkSaveChanges(bool validateOnSaveEnabled = true,int bulkSize = 64)
        {
            if (dBBulkOperations == null)
                return this.SaveChanges();

            return dBBulkOperations.BulkSaveChanges(validateOnSaveEnabled,bulkSize);
        }

        public virtual IPagedResult<T> PagedQuery<T>(string strSql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "") where T : class
        {
            if (pagedQuery == null)
                throw new NotImplementedException("不支持的方法，PagedQuery未实现！");

            return pagedQuery.Query<T>(strSql, parameters, pageIndex, pageSize, sort);
        }

        public virtual IPagedTableResult PagedTableQuery(string sql, Dictionary<string, object> parameters, int pageIndex, int pageSize, string sort)
        {
            if (pagedTableQuery == null)
                throw new NotImplementedException("不支持的方法，PagedTableQuery未实现！");

            return pagedTableQuery.Query(sql, parameters, pageIndex, pageSize, sort);
        }

        public void ExecuteProc(string procName, params DbParameter[] dbParameter)
        {
            if (this.Database.Connection.State != ConnectionState.Open)
                this.Database.Connection.Open();
            var cmd = this.Database.Connection.CreateCommand(); 
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;
            if (dbParameter.Length > 0)
                cmd.Parameters.AddRange(dbParameter);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <summary>
        /// 获取datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            if (this.Database.GetDbConnection().State != ConnectionState.Open)
                this.Database.GetDbConnection().Open();
            var cmd = this.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;

            DataTable table = new DataTable();
            var read = cmd.ExecuteReader();
            for (int i = 0; i < read.FieldCount; i++)
            {
                //创建table的第一列
                DataColumn Column = new DataColumn();
                //该列的数据类型
                Column.DataType = read.GetFieldType(i);
                //该列得名称
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
    }
}
