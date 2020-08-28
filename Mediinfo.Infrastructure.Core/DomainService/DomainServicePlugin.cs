using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.UnitOfWork;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mediinfo.Infrastructure.Core.DomainService
{
    public class DomainServicePlugin
    {
        private static Dictionary<Type, List<MethodInfo>> pluginTypes = new Dictionary<Type, List<MethodInfo>>();
        private static Dictionary<Type, List<MethodInfo>> defaultTypes = new Dictionary<Type, List<MethodInfo>>();
        private static readonly DomainServicePlugin _instance = new DomainServicePlugin();

        /// <summary>
        /// 单例构造函数
        /// </summary>
        private DomainServicePlugin()
        {
            var pluginDll = Enterprise.Config.MediinfoConfig.GetValue("SystemConfig.xml", "DomainServicePlugin");
            if (!string.IsNullOrEmpty(pluginDll))
            {
                var ptypes = Assembly.Load(pluginDll).GetTypes().Where(p => p.IsSubclassOf(typeof(DomainServiceBase)));
                foreach (var item in ptypes)
                {
                    var methods = item.GetMethods().ToList();
                    pluginTypes.Add(item, methods);
                }
            }

            // 默认Types
            List<Assembly> assemblys = new List<Assembly>();
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo TheFolder = new DirectoryInfo(assemblyPath);
            var mediDlls = TheFolder.GetFiles().Where(c => c.Extension.Contains("dll") && c.Name.Contains("Mediinfo.DomainService"));
            var path = AppDomain.CurrentDomain.BaseDirectory;
            // 遍历文件
            foreach (FileInfo NextDll in mediDlls)
            {
                var ass = Assembly.LoadFrom(NextDll.FullName);
                assemblys.Add(ass);
            }
            var dtypes = assemblys.SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(typeof(DomainServiceBase))));
            foreach (var item in dtypes)
            {
                var methods = item.GetMethods().ToList();
                defaultTypes.Add(item, methods);
            }
        }

        public static DomainServicePlugin Instance
        {
            get { return _instance; }
        }

        public object Invoke(IUnitOfWork unitOfWork, ServiceContext serviceContext, string domainServiceName, string methodName, params object[] parameters)
        {
            var types = pluginTypes.Where(m => m.Key.FullName.EndsWith(domainServiceName));
            if (types.Any())
            {
                var methods = types.FirstOrDefault().Value.Where(m => m.Name == methodName);
                if (methods.Any())
                {
                    var type = types.FirstOrDefault().Key;
                    var method = methods.FirstOrDefault();

                    try
                    {
                        object obj = Activator.CreateInstance(type, unitOfWork, serviceContext);
                        object result = method.Invoke(obj, parameters);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            types = defaultTypes.Where(m => m.Key.FullName.EndsWith(domainServiceName));
            if (types.Any())
            {
                var methods = types.FirstOrDefault().Value.Where(m => m.Name == methodName);
                if (methods.Any())
                {
                    var type = types.FirstOrDefault().Key;
                    var method = methods.FirstOrDefault();

                    try
                    {
                        object obj = Activator.CreateInstance(type, unitOfWork, serviceContext);
                        object result = method.Invoke(obj, parameters);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            return null;
        }
    }
}
