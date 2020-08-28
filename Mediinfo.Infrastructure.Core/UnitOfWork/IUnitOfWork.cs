using Mediinfo.Infrastructure.Core.DBContext;
using Mediinfo.Infrastructure.Core.MessageQueue;
using Mediinfo.Infrastructure.Core.Repository;

using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Mediinfo.Infrastructure.Core.UnitOfWork
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : ISaveChanges, IDisposable
    {
        Guid UnitOfWorkID { get; set; }

        StringBuilder SqlLog { get; set; }

        Messager CurrentMessager { get; set; }

        IMessagePlugin MessagePlugin { get; set; }

        /// <summary>
        /// 当前的事务
        /// </summary>
        DbTransaction CurrentDbTransaction { get; }

        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 使用已有事务
        /// </summary>
        /// <param name="dbTransaction"></param>
        void UseTransaction(DbTransaction dbTransaction);

        /// <summary>
        /// 提交
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();

        DataTable GetDataTable(string sql);

        void ExecuteProc(string procName, params DbParameter[] dbParameter);

        //int SaveChanges();

        //int BulkSaveChanges(int bulkSize = 64);
    }
}
