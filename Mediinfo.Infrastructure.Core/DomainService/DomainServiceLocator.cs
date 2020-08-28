using Autofac;
using Autofac.Core;
using Autofac.Features.OwnedInstances;

using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.UnitOfWork;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mediinfo.Infrastructure.Core.DomainService
{
    /// <summary>
    /// 窗口外部调用定位器
    /// </summary>
    public class DomainServiceLocator
    {
        private readonly Autofac.IContainer _container;
        private static readonly DomainServiceLocator _instance = new DomainServiceLocator();
        
        /// <summary>
        /// 单例构造函数
        /// </summary>
        private DomainServiceLocator()
        {
            var builder = new ContainerBuilder();
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            List<Assembly> assemblys = new List<Assembly>();

            DirectoryInfo TheFolder = new DirectoryInfo(assemblyPath);
            var mediDlls = TheFolder.GetFiles().Where(c => c.Extension.Contains("dll") && c.Name.Contains("Mediinfo.DomainService"));

            var path = AppDomain.CurrentDomain.BaseDirectory;
            // 遍历文件
            foreach (FileInfo NextDll in mediDlls)
            {
                var ass = Assembly.LoadFrom(NextDll.FullName);
                assemblys.Add(ass);
            }

            var types = assemblys.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDomainService)))).ToArray();

            var impls = types.Where(t => !t.IsInterface).ToList();

            foreach (var impl in impls)
            {
                var services = impl.GetInterfaces().Where(m => m != typeof(IDomainService));
                foreach (var service in services)
                {
                    builder.RegisterType(impl).As(service).InstancePerDependency();
                }
            }

            _container = builder.Build();
        }

        public static DomainServiceLocator Instance
        {
            get { return _instance; }
        }

        #region Public Methods

        /// <summary>
        /// 获取IDomainService
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public T GetService<T>(IUnitOfWork unitOfWork, ServiceContext serviceContext) where T: IDomainService
        {
            var list = new List<ResolvedParameter>();

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IUnitOfWork),
                                           (pi, ctx) => unitOfWork));

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ServiceContext),
                                                     (pi, ctx) => serviceContext));

            var ownedT = _container.Resolve<Owned<T>>(list);
            return ownedT.Value;
        }

        #endregion
    }
}