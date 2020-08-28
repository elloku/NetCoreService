using System.Collections.Generic;

namespace Mediinfo.Enterprise.PagedResult
{
    /// <summary>
    /// 分页结果构造器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResultBuilder<T>
    {
        private long _totalRecords;
        private long _totalPages;
        private long _pageSize;
        private long _pageIndex;
        private List<T> _data;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalRecords"></param>
        /// <param name="totalPages"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="data"></param>
        public PagedResultBuilder(long totalRecords, long totalPages, long pageSize, long pageIndex, List<T> data)
        {
            _totalRecords = totalRecords;
            _totalPages = totalPages;
            _pageSize = pageSize;
            _pageIndex = pageIndex;
            _data = data;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public PagedResult<T> Build()
        {
            return new PagedResult<T>(_totalRecords, _totalPages, _pageSize, _pageIndex, _data);
        }
    }
}
