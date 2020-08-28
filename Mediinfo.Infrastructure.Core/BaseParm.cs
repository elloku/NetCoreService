using Mediinfo.Enterprise;

namespace Mediinfo.Infrastructure.Core
{
    /// <summary>
    /// 参数基类
    /// </summary>
    public class BaseParm
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseParm()
        {

        }

        /// <summary>
        /// 服务上下文
        /// </summary>
        public ServiceContext ServiceContext { get; set; }
    }

}
