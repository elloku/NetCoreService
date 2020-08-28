using Consul;
using Mediinfo.APIGateway.Core.Cache;
using Mediinfo.APIGateway.Core.Consul;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediinfo.APIGateway.Core.Services
{
    /// <summary>
    /// 服务管理
    /// </summary>
    public class ServiceManager : IDisposable
    {
        private string prefixRoundRobin = "service/roundRobin";
        private ConsulClient _consul;
        private static readonly ServiceManager _instance = new ServiceManager();

        /// <summary>
        /// 单例
        /// </summary>
        public static ServiceManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        private ServiceManager(string host = "127.0.0.1", int port = 8500)
        {
            try
            {
                _consul = new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{host}:{port}");
                });
            }
            catch (Exception ex)
            {
                _consul.Dispose();
                _consul = null;

                _consul = new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{host}:{port}");
                });
            }
        }

        ///获取服务
        /// <summary>
        /// 根据服务名称获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> GetService(string serviceName, System.Threading.CancellationToken ct)
        {
            var services = await this.GetServices(serviceName,ct);
            if (services.Length <= 0)
            {
                return string.Empty;
            }
            return ClusterStrategy.GetServiceByRandom(services);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="ct"></param>
        /// <param name="passingOnly"></param>
        /// <returns></returns>
        public async Task<string> GetService(string serviceName, string version, System.Threading.CancellationToken ct, bool passingOnly = true)
        {
            var services = await this.GetServices(serviceName, version,ct, passingOnly);
            if (services.Response.Length <= 0)
            {
                return string.Empty;
            }

            var serviceEntrys = services.Response;
            List<ServiceEntry> passingServices = new List<ServiceEntry>();
            foreach (var item in serviceEntrys)
            {
                // 如果服务检查中有不健康的服务，就剔除掉(排除consul挂掉的情况)
                if (item.Checks.Where(m => m.Status.Status != "passing").Count() > 0)
                {
                    continue;
                }

                passingServices.Add(item);
            }

            var lastIndex = await GetServiceRoundRobin(serviceName, version,ct);
            return ClusterStrategy.GetServiceByRoundRobin(passingServices, lastIndex);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="httpRequest"></param>
        /// <param name="ct"></param>
        /// <param name="passingOnly"></param>
        /// <returns></returns>
        public async Task<string> GetService(string serviceName, string version, System.Web.HttpRequestBase httpRequest, System.Threading.CancellationToken ct, bool passingOnly = true)
        {
            var services = await this.GetServices(serviceName, version, ct, passingOnly);
            if (services.Response.Length <= 0)
            {
                return string.Empty;
            }

            var serviceEntrys = services.Response;
            List<ServiceEntry> passingServices = new List<ServiceEntry>();
            // 客户ip
            string userIp = httpRequest.UserHostAddress;

            List<ServiceEntry> allTestServices = new List<ServiceEntry>();

            List<ServiceEntry> offLineServices = new List<ServiceEntry>();

            foreach (var item in serviceEntrys)
            {
                // 如果服务检查中有不健康的服务，就剔除掉(排除consul挂掉的情况)
                if (item.Checks.Where(m => m.Status.Status != "passing").Count() > 0)
                {
                    continue;
                }
                passingServices.Add(item);
                // 查询出测试服务
                if ((await ABTestingCache.Instance.GetServiceStatus(item.Service.Service, item.Service.Tags.FirstOrDefault(), item.Service.ID)) == "测试")
                {
                    allTestServices.Add(item);
                }

                // 查询出测试服务
                if ((await ABTestingCache.Instance.GetServiceStatus(item.Service.Service, item.Service.Tags.FirstOrDefault(), item.Service.ID)) == "下线")
                {
                    offLineServices.Add(item);
                }
            }

            List<ServiceEntry> resultTestServices = new List<ServiceEntry>();

            // 当且仅当部分服务是灰度服务的时候
            if (allTestServices.Count < passingServices.Count)
            {
                foreach (var item in allTestServices)
                {
                    var testips = await ABTestingCache.Instance.GetServiceTestIp(item.Service.Service, item.Service.Tags.FirstOrDefault(), item.Service.ID);
                    foreach (var ip in testips)
                    {
                        if(!string.IsNullOrEmpty(ip))
                        {
                            long start = IP2Long(ip.Split('-')[0]);
                            long end = IP2Long(ip.Split('-')[1]);
                            long ipAddress = IP2Long(userIp);
                            bool inRange = (ipAddress >= start && ipAddress <= end);
                            if (inRange)
                            {
                                resultTestServices.Add(item);
                            }
                        }
                        
                    }

                }
            }

            var lastIndex = await GetServiceRoundRobin(serviceName, version, ct);

            // 如果ip地址匹配灰度发布，优先返回灰度发布的地址
            if(resultTestServices.Count > 0)
            {
                passingServices = resultTestServices;
            }
            else
            {
                //不匹配则优先返回稳定版本
                foreach (var item in allTestServices)
                {
                    passingServices.Remove(item);
                }
            }

            // 移除下线的服务
            foreach (var item in offLineServices)
            {
                passingServices.Remove(item);
            }

            return ClusterStrategy.GetServiceByRoundRobin(passingServices, lastIndex);
        }

        /// <summary>
        /// 把IP地址转成数字
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="ipHash"></param>
        /// <param name="ct"></param>
        /// <param name="passingOnly"></param>
        /// <returns></returns>
        public async Task<string> GetService(string serviceName, string version,int ipHash, System.Threading.CancellationToken ct, bool passingOnly = true)
        {
            var services = await this.GetServices(serviceName, version,ct, passingOnly);
            if (services.Response.Length <= 0)
            {
                return string.Empty;
            }
            return ClusterStrategy.GetServiceByIpHash(services.Response, ipHash);
        }

        /// <summary>
        /// 获取轮询服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceVersion"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ulong> GetServiceRoundRobin(string serviceName, string serviceVersion, System.Threading.CancellationToken ct)
        {
            // 默认为随机
            ulong lastIndex = (ulong)DateTime.Now.Millisecond;
            try
            {
                KVPair kVPair;
                // 查找kv是否存在
                string kv = await ConsulKVManager.Instance.GetValue(
                    ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.RoundRobin,
                    serviceName, serviceVersion, ""), ct);
                if(string.IsNullOrEmpty(kv))
                {
                    lastIndex = 0;
                }
                else
                {
                    lastIndex = Convert.ToUInt64(kv) + 1;
                    
                }
                await ConsulKVManager.Instance.SetValue(
                    ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.RoundRobin,
                    serviceName, serviceVersion, ""), lastIndex.ToString(), ct);
                
                //var kv = await _consul.KV.Get(prefixRoundRobin + "/" + serviceName + "/" + serviceVersion, ct);
                //if (kv != null)
                //{
                //    if (kv.Response == null)
                //    {
                //        kVPair = new KVPair(prefixRoundRobin + "/" + serviceName + "/" + serviceVersion);
                //        kVPair.Value = BitConverter.GetBytes((ulong)0);
                //    }
                //    else
                //    {
                //        kVPair = kv.Response;
                //        kVPair.Value = BitConverter.GetBytes(BitConverter.ToUInt64(kVPair.Value, 0) + 1);
                //    }

                //    var resultKV = await _consul.KV.Put(kVPair, WriteOptions.Default, ct);
                //    if (resultKV != null)
                //    {
                //        var result = resultKV.Response;
                //        if (result)
                //        {
                //            lastIndex = BitConverter.ToUInt64(kVPair.Value, 0);
                //        }
                //    }
                //}
            }
            catch(Exception ex)
            {
                //LogHelper.WriteLog(typeof(ServiceManager), ex.ToString());
            }

            return lastIndex;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceID"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ServiceEntry> TakeService(string serviceID, System.Threading.CancellationToken ct)
        {
            var services = await this.GetServices(serviceID.Split('@')[0],ct);
            if (services.Length <= 0)
            {
                return null;
            }
            return services.Where(m => m.Service.ID == serviceID).FirstOrDefault();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected async Task<ServiceEntry[]> GetServices(string serviceName,System.Threading.CancellationToken ct)
        {
            QueryResult<ServiceEntry[]> serviceEntrys = await _consul.Health.Service(serviceName,ct);
            return serviceEntrys.Response;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="ct"></param>
        /// <param name="passingOnly"></param>
        /// <returns></returns>
        protected async Task<QueryResult<ServiceEntry[]>> GetServices(string serviceName, string version, System.Threading.CancellationToken ct, bool passingOnly = true)
        {
            QueryResult<ServiceEntry[]> queryServiceEntrys = await _consul.Health.Service(serviceName, version, passingOnly,ct);
            return queryServiceEntrys;
        }

        /// <summary>
        /// 析构
        /// </summary>
        public void Dispose()
        {
            _consul.Dispose();
        }
    }
}
