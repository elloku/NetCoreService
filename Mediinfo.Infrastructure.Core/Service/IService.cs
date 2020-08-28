using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.DBContext;

namespace Mediinfo.Infrastructure.Core.Service
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// ef上下文
        /// </summary>
        DBContextBase DBContext { get; set; }

        /// <summary>
        /// 服务上下文
        /// </summary>
        ServiceContext ServiceContext { get; set; }
    }
}
