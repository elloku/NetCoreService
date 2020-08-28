using Mediinfo.Enterprise;

namespace Mediinfo.Infrastructure.Core.DbMessage
{
    public class ServiceInfo
    {
        /// <summary>
        /// 模块名称（DLL）
        /// </summary>
        public string MoKuaiMc { get; set; }

        /// <summary>
        /// 业务名称(Controller)
        /// </summary>
        public string YeWuMc { get; set; }

        /// <summary>
        /// 操作名称(Controller中方法名)
        /// </summary>
        public string CaoZuoMc { get; set; }

        /// <summary>
        /// 环境上下文
        /// </summary>
        public ServiceContext Context { get; set; }
    }
}
