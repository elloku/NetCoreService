using Consul;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mediinfo.APIGateway.Core.Consul
{
    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public class ClusterStrategy
    {
        /// <summary>
        /// 随机
        /// </summary>
        /// <param name="serviceEntrys"></param>
        /// <returns></returns>
        public static string GetServiceByRandom(ServiceEntry[] serviceEntrys)
        {
            List<ServiceEntry> passingServices = new List<ServiceEntry>();
            foreach (var item in serviceEntrys)
            {
                // 如果服务检查中有不健康的服务，就剔除掉
                if (item.Checks.Any(m => m.Status.Status != "passing"))
                {
                    continue;
                }

                passingServices.Add(item);
            }

            // 随机获取其中一个健康的服务
            Random rd = new Random(DateTime.Now.Millisecond);
            int rdIndex = rd.Next(0, passingServices.Count);

            if (rdIndex >= passingServices.Count)
            {
                rdIndex = 0;
            }

            ServiceEntry service = passingServices[rdIndex];

            return service.Node.Address + ":" + service.Service.Port + "/" + service.Service.Address;
        }

        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="passingServices"></param>
        /// <param name="lastIndex"></param>
        /// <returns></returns>
        public static string GetServiceByRoundRobin(List<ServiceEntry> passingServices, ulong lastIndex)
        {
            // 轮询算法
            ulong rdIndex = lastIndex % (ulong)passingServices.Count;

            ServiceEntry service = passingServices[(int)rdIndex];

            return service.Node.Address + ":" + service.Service.Port + "/" + service.Service.Address;
        }
        
        /// <summary>
        /// IP哈希
        /// </summary>
        /// <param name="serviceEntrys"></param>
        /// <param name="ipHash"></param>
        /// <returns></returns>
        public static string GetServiceByIpHash(ServiceEntry[] serviceEntrys, int ipHash)
        {
            List<ServiceEntry> passingServices = new List<ServiceEntry>();
            foreach (var item in serviceEntrys)
            {
                // 如果服务检查中有不健康的服务，就剔除掉
                if (item.Checks.Any(m => m.Status.Status != "passing"))
                {
                    continue;
                }

                passingServices.Add(item);
            }

            // 轮询算法
            int rdIndex = ipHash % passingServices.Count;

            ServiceEntry service = passingServices[rdIndex];

            return service.Node.Address + ":" + service.Service.Port + "/" + service.Service.Address;
        }
    }
}
