using System.Linq;
using System.Reflection;

namespace Mediinfo.APIGateway.Core.Services
{
    /// <summary>
    /// 获取程序集中的服务信息
    /// </summary>
    public class ServiceInfo
    {
        private Assembly assembly = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assembly"></param>
        public ServiceInfo(Assembly assembly)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// 获取服务名
        /// </summary>
        /// <returns></returns>
        public string GetServiceName()
        {
            //var assemblyTitle = (AssemblyTitleAttribute)assembly.GetCustomAttributes(true).Where(m => m.GetType() == typeof(AssemblyTitleAttribute)).FirstOrDefault();
            var assemblyTitle = assembly.GetName().Name;
            string serviceName = assemblyTitle.Substring("Mediinfo.Service.".Length).Replace('.', '-');
            return serviceName;
        }

        /// <summary>
        /// 获取服务版本
        /// </summary>
        /// <returns></returns>
        public string GetServiceVersion()
        {
            var assemblyVersion = (AssemblyVersionAttribute)assembly.GetCustomAttributes(true).Where(m => m.GetType() == typeof(AssemblyVersionAttribute)).FirstOrDefault();
            //return assemblyVersion.Version;
            return "V1";
        }
    }
}
