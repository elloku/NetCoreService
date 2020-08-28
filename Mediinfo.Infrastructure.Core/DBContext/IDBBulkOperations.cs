namespace Mediinfo.Infrastructure.Core.DBContext
{
    /// <summary>
    /// 批量操作
    /// </summary>
    public interface IDBBulkOperations
    {
        int BulkSaveChanges(bool validateOnSaveEnabled = true,int bulkSize = 64);
    }
}
