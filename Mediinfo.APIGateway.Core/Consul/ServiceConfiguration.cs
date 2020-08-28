namespace Mediinfo.APIGateway.Core.Consul
{
    /// <summary>
    /// 服务配置
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int ServicePort { get; set; }
    }
}
