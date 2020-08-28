using Mediinfo.Enterprise.PagedResult;
using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.DBContext
{
    public interface IPagedQuery
    {
        IPagedResult<T> Query<T>(string strSql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "") where T : class;
    }
}
