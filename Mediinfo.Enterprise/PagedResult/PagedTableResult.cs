using System.Data;

namespace Mediinfo.Enterprise.PagedResult
{
    public class PagedTableResult : IPagedTableResult
    {
        #region Public Properties

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalRecords { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// 每页的记录数
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public long PageIndex { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public DataTable PageData { get; set; }

        #endregion 

        #region Ctor

        public PagedTableResult(long totalRecords, long totalPages, long pageSize, long pageIndex, DataTable data)
        {
            this.TotalPages = totalPages;
            this.TotalRecords = totalRecords;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.PageData = data;
        }

        #endregion
    }
}
