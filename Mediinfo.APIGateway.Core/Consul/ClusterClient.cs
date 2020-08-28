using Consul;
using Mediinfo.APIGateway.Core.Log;
using Mediinfo.APIGateway.Core.Services;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Mediinfo.APIGateway.Core.Consul
{
    /// <summary>
    /// consul集群客户端
    /// </summary>
    public class ClusterClient : IDisposable
    {
        private ConsulClient _consul;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public ClusterClient(string host = "127.0.0.1", int port = 8500)
        {
            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{host}:{port}");
            });
        }

        /// <summary>
        /// 获取所有服务
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceEntry>> GetSevices()
        {
            List<ServiceEntry> result = new List<ServiceEntry>();
            var nodes = await _consul.Catalog.Services();

            foreach (var node in nodes.Response)
            {
                var services = await _consul.Health.Service(node.Key);
                result.AddRange(services.Response);
            }
            return result;
        }

        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceAddress"></param>
        /// <param name="servicePort"></param>
        /// <returns></returns>
        public async Task<bool> IsServiceExists(string serviceAddress, int servicePort = 80)
        {
            List<ServiceEntry> services = await this.GetSevices();
            return services.Any(m => m.Service.Address == serviceAddress && m.Service.Port == servicePort);
        }

       /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="servicePort"></param>
        /// <returns></returns>
        public async Task<string> Deregister(int servicePort)
        {
            var allServices = await _consul.Agent.Services();
            var services = allServices.Response.Where(m => m.Value.Port == servicePort);
            foreach (var service in services)
            {

                await ConsulKVManager.Instance.DeletePath(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ServiceStatus, service.Value.Service, service.Value.Tags.FirstOrDefault(), service.Value.ID));
                await ConsulKVManager.Instance.DeletePath(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.RegisterInfo, service.Value.Service, service.Value.Tags.FirstOrDefault(), service.Value.ID));
                await ConsulKVManager.Instance.DeletePath(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ABTestingIp, service.Value.Service, service.Value.Tags.FirstOrDefault(), service.Value.ID));

                LogHelper.WriteLog(this.GetType(), "服务被注销了：" + service.Value.ID);
                await _consul.Agent.ServiceDeregister(service.Value.ID);
            }

            return "200";
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="node"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> Deregister(string node, string id)
        {
            try
            {
                LogHelper.WriteLog(this.GetType(), "服务被注销了：" + id);
                await _consul.Agent.ServiceDeregister(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "200";
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public ServiceConfiguration GetConfiguration()
        {
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            serviceConfiguration.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
            serviceConfiguration.ServiceVersion = ConfigurationManager.AppSettings["ServiceVersion"];
            return serviceConfiguration;
        }

        /// <summary>
        /// 重新注册TCP服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="serviceAddress"></param>
        /// <param name="servicePort"></param>
        /// <param name="checking"></param>
        /// <returns></returns>
        public async Task<string> ReRegisterTcp(string serviceName, string version, string serviceAddress, int servicePort, bool checking = true)
        {
            if (await IsServiceExists(serviceAddress, servicePort))
            {
                List<ServiceEntry> allServices = await this.GetSevices();
                var services = allServices.Where(m => m.Service.Address == serviceAddress && m.Service.Port == servicePort);
                foreach (var service in services)
                {
                    await _consul.Agent.ServiceDeregister(service.Service.ID);
                }
            }

            var registration = new AgentServiceRegistration()
            {
                ID = serviceName + "@" + Guid.NewGuid(),
                Address = serviceAddress,
                Name = serviceName,
                Tags = new[] { version },
                Port = servicePort,
            };

            if (checking)
            {
                registration.Check = new AgentServiceCheck
                {
                    TCP = serviceAddress + ":" + servicePort,
                    Interval = new TimeSpan(50000000)
                };
            }

            var result = await _consul.Agent.ServiceRegister(registration);
            return result.StatusCode.ToString();
        }

        /// <summary>
        /// 重新注册HTTP服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="servicePort"></param>
        /// <param name="checking"></param>
        /// <returns></returns>
        public async Task<string> ReRegister(string serviceName, string version, int servicePort, bool checking = true)
        {
            string serviceAddress = serviceName;
            // 先注销
            var allServices = await _consul.Agent.Services();
            var services = allServices.Response.Where(m => m.Value.Service == serviceName && m.Value.Port == servicePort && m.Value.Tags.Contains(version));
            foreach (var service in services)
            {
                await _consul.Agent.ServiceDeregister(service.Value.ID);
            }

            // 再重新注册
            var registration = new AgentServiceRegistration()
            {
                ID = serviceName + "@" + Guid.NewGuid(),
                Address = serviceAddress,
                Name = serviceName,
                Tags = new[] { version },
                Port = servicePort,
            };

            if (checking)
            {
                registration.Check = new AgentServiceCheck
                {
                    HTTP = "http://127.0.0.1:" + servicePort + "/Health",
                    Interval = new TimeSpan(50000000)
                };
            }

            string status = await ConsulKVManager.Instance.GetValue("service/abtesting/servicedefaultstatus");
            if (!string.IsNullOrEmpty(status) && status == "1")
            {
                await ConsulKVManager.Instance.SetValue(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ServiceStatus, serviceName, version, registration.ID), "测试");
                await ConsulKVManager.Instance.SetValue(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.ABTestingIp, serviceName, version, registration.ID), "");
            }

            await ConsulKVManager.Instance.SetValue(ConsulKVConfig.GetServiceKVPath(ServiceKVConfigPath.RegisterInfo, serviceName, version, registration.ID), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var result = await _consul.Agent.ServiceRegister(registration);
            return result.StatusCode.ToString();
        }

        /// <summary>
        /// 重新注册TCP服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="serviceAddress"></param>
        /// <param name="servicePort"></param>
        /// <param name="checking"></param>
        /// <returns></returns>
        public async Task<string> RegisterTcp(string serviceName, string version, string serviceAddress, int servicePort, bool checking = true)
        {
            if (await IsServiceExists(serviceAddress, servicePort))
            {
                return "403";
            }

            var registration = new AgentServiceRegistration()
            {
                ID = serviceName + "@" + Guid.NewGuid(),
                Address = serviceAddress,
                Name = serviceName,
                Tags = new[] { version },
                Port = servicePort,
            };

            if (checking)
            {
                registration.Check = new AgentServiceCheck
                {
                    TCP = serviceAddress + ":" + servicePort,
                    Interval = new TimeSpan(50000000)
                };
            }

            var result = await _consul.Agent.ServiceRegister(registration);
            return result.StatusCode.ToString();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="version"></param>
        /// <param name="servicePort"></param>
        /// <param name="checking"></param>
        /// <returns></returns>
        public async Task<string> Register(string serviceName, string version, int servicePort, bool checking = true)
        {
            string serviceAddress = serviceName;

            var registration = new AgentServiceRegistration()
            {
                ID = serviceName + "@" + Guid.NewGuid(),
                Address = serviceAddress,
                Name = serviceName,
                Tags = new[] { version },
                Port = servicePort,
            };

            if (checking)
            {
                registration.Check = new AgentServiceCheck
                {
                    HTTP = "http://localhost:" + servicePort + "/Health",
                    Interval = new TimeSpan(30000000)
                };
            }

            var result = await _consul.Agent.ServiceRegister(registration);
            return result.StatusCode.ToString();
        }

        /// <summary>
        /// 析构
        /// </summary>
        public void Dispose()
        {
            _consul.Dispose();
            _consul = null;
        }
    }
}
