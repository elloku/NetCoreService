using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Consul
{
    public class ClusterClient : IDisposable
    {
        private ConsulClient _consul;
        private static readonly ClusterClient _instance = new ClusterClient();

        public static ClusterClient Instance
        {
            get { return _instance; }
        }

        public ClusterClient(string host = "127.0.0.1", int port = 8500)
        {
            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{host}:{port}");

            });
        }

        public async Task<List<ServiceEntry>> GetSevices()
        {
            List<ServiceEntry> result = new List<ServiceEntry>();
            var nodes = await  _consul.Catalog.Services();

            foreach (var node in nodes.Response)
            {
                 var services = await _consul.Health.Service(node.Key);
                 result.AddRange(services.Response);
            }
            return result;
        }

        public async Task<bool> IsServiceExists(string serviceAddress, int servicePort = 80)
        {
            List<ServiceEntry> services = await this.GetSevices();
            return services.Any(m => m.Service.Address == serviceAddress && m.Service.Port == servicePort);
        }
        
        public async Task<string> ReRegister(string serviceId, string serviceName, string version, int servicePort, bool checking = true)
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
                ID = serviceId,
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

            var result = await _consul.Agent.ServiceRegister(registration);
            return result.StatusCode.ToString();
        }

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

        public void Dispose()
        {
            _consul.Dispose();
            _consul = null;
        }
    }
}
