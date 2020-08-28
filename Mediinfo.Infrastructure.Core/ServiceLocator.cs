using Autofac;
using Autofac.Core;
using Autofac.Features.OwnedInstances;

using Mediinfo.Infrastructure.Core.Entity;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Mediinfo.Infrastructure.Core
{
    /// <summary>
    /// 服务定位器
    /// </summary>
    [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
    public class ServiceLocator : IServiceProvider
    {
        private readonly Autofac.IContainer _container;
        private static readonly ServiceLocator _instance = new ServiceLocator();

        /// <summary>
        /// 单例构造函数
        /// </summary>
        private ServiceLocator()
        {
            try
            {
                var builder = new ContainerBuilder();
                var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

                if (HttpContext.Current != null)
                {
                    assemblyPath += "bin";
                }

                List<Assembly> assemblys = new List<Assembly>();

                DirectoryInfo TheFolder = new DirectoryInfo(assemblyPath);
                var mediDlls = TheFolder.GetFiles().Where(c => c.Extension.Contains("dll") && (c.Name.Contains("Mediinfo.Infrastructure") || c.Name.Contains("Mediinfo.Domain")));

                // 遍历文件
                foreach (FileInfo NextDll in mediDlls)
                {
                    var ass = Assembly.LoadFrom(NextDll.FullName);
                    assemblys.Add(ass);
                }

                var types = assemblys.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDependency)))).ToArray();

                var impls = types.Where(t => !t.IsInterface).ToList();

                foreach (var impl in impls)
                {
                    var services = impl.GetInterfaces().Where(m => m != typeof(IDependency));
                    foreach (var service in services)
                    {
                        builder.RegisterType(impl).As(service).InstancePerDependency();
                    }
                }

                _container = builder.Build();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    Console.WriteLine(exSub.Message);
                    Console.WriteLine(exSub.ToString());
                    sb.AppendLine(exSub.ToString());
                }
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ServiceLocator Instance
        {
            get { return _instance; }
        }

        #region Public Methods

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            var ownedT = _container.Resolve<Owned<T>>();
            return ownedT.Value;
        }

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public T GetService<T>(List<ResolvedParameter> parameterList)
        {
            if (parameterList == null || parameterList.Count <= 0)
            {
                return _container.Resolve<Owned<T>>().Value;
            }
            else
            {
                return _container.Resolve<Owned<T>>(parameterList.ToArray()).Value;
            }
        }

        //public T GetServiceByParm<T>(object overridedArguments)
        //{
        //    if (overridedArguments == null)
        //    {
        //        return _container.Resolve<Owned<T>>().Value;
        //    }
        //    else
        //    {
        //        var overrides = new List<ResolvedParameter>();
        //        var argumentsType = overridedArguments.GetType();
        //        argumentsType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //            .ToList()
        //            .ForEach(property =>
        //            {

        //                overrides.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == property.GetType(),
        //                                   (pi, ctx) => property.GetValue(overridedArguments, null)));
        //            });
        //        return _container.Resolve<Owned<T>>(overrides).Value;
        //    }
        //}

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T GetService<T>(params object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
            {
                return _container.Resolve<Owned<T>>().Value;
            }
            else
            {
                List<ResolvedParameter> paramList = new List<ResolvedParameter>();
                foreach (var item in parameters)
                {
                    paramList.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == item.GetType(), (pi, ctx) => item));
                }
                return _container.Resolve<Owned<T>>(paramList.ToArray()).Value;
            }
        }

        /// <summary>
        /// 注入所有服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetAllService<T>()
        {
            return _container.Resolve<IEnumerable<T>>().ToArray();
        }

        /// <summary>
        /// 注入所有服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object[] GetAllService(Type type)
        {
            Type enumerableOfType = typeof(IEnumerable<>).MakeGenericType(type);
            return (object[])_container.ResolveService(new TypedService(enumerableOfType));
        }

        /// <summary>
        /// 注入所有服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        #endregion
    }
}