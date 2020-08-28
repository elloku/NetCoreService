namespace Mediinfo.APIGateway.Core.Services
{
    /// <summary>
    /// Consul字典配置
    /// </summary>
    public class ConsulKVConfig
    {
        #region 配置中心路径获取

        /// <summary>
        /// 获取字典前缀
        /// </summary>
        /// <param name="serviceKVPath"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static string GetServiceKVPath(ServiceKVConfigPath serviceKVPath, string serviceName, string serviceVersion, string serviceId)
        {
            string result = "";
            switch (serviceKVPath)
            {
                case ServiceKVConfigPath.RegisterInfo:
                    result = "service/registerInfo/" + serviceName + "/" + serviceVersion + "/" + serviceId;
                    break;
                case ServiceKVConfigPath.ServiceStatus:
                    result = "service/abtesting/" + serviceName + "/" + serviceVersion + "/" + serviceId;
                    break;
                case ServiceKVConfigPath.ABTestingIp:
                    result = "service/abtesting/" + serviceName + "/" + serviceVersion + "/" + serviceId + "/testip";
                    break;
                case ServiceKVConfigPath.RoundRobin:
                    result = "service/roundRobin/" + serviceName + "/" + serviceVersion;
                    break;
                default:
                    break;
            }
            return result;
        }

        #endregion
    }
}
