using System.Data;

namespace Mediinfo.Enterprise.PagedResult
{
    /// <summary>
    /// 分页结果接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPagedTableResult
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        long TotalRecords { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        long TotalPages { get; set; }

        /// <summary>
        /// 每页的记录数
        /// </summary>
        long PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        long PageIndex { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        DataTable PageData { get; set; }
    }
}
