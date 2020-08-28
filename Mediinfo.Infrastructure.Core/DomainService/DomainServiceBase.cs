using Autofac.Core;

using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.EventBus;
using Mediinfo.Infrastructure.Core.Repository;
using Mediinfo.Infrastructure.Core.UnitOfWork;

using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.DomainService
{
    /// <summary>
    /// 领域服务基类
    /// </summary>
    public abstract class DomainServiceBase:ICanShu
    {
        /// <summary>
        /// 当前工作单元（Unit Of Work）
        /// </summary>
        protected IUnitOfWork UnitOfWork = null;

        /// <summary>
        /// 服务上下文
        /// </summary>
        public ServiceContext ServiceContext { get; set; }

        /// <summary>
        /// 校验DomianService层并抛出异常
        /// </summary>
        public ICheck Check => new DomainServiceCheck();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DomainServiceBase()
        {
            this.UnitOfWork = null;
            this.ServiceContext = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uow">工作单元（Unit Of Work）</param>
        /// <param name="serviceContext">服务上下文</param>
        public DomainServiceBase(IUnitOfWork uow,ServiceContext serviceContext)
        {
            this.UnitOfWork = uow;
            this.ServiceContext = serviceContext;
        }

        protected virtual T GetDomainService<T>() where T : IDomainService
        {
            return DomainServiceLocator.Instance.GetService<T>(UnitOfWork, ServiceContext);
        }

        protected virtual object DomainServiceInvoke(string domainServiceName, string methodName, params object[] parameters)
        {
            return DomainServicePlugin.Instance.Invoke(UnitOfWork, ServiceContext, domainServiceName, methodName, parameters);
        }

        /// <summary>
        /// 获取仓储接口
        /// </summary>
        /// <typeparam name="T">仓储类型</typeparam>
        /// <param name="uow">工作单元（Unit Of Work）</param>
        /// <returns></returns>
        protected virtual T GetRepository<T>( )
        {
            var list = new List<ResolvedParameter>();

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IRepositoryContext),
                                           (pi, ctx) => (IRepositoryContext)UnitOfWork));

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ServiceContext),
                                                     (pi, ctx) => (ServiceContext)ServiceContext));

            return ServiceLocator.Instance.GetService<T>(list);
            //return ServiceLocator.Instance.GetServiceByParm<T>(new { repositoryContext = (IRepositoryContext)UnitOfWork, serviceContext = ServiceContext });
        }

        /// <summary>
        /// 获取仓储接口
        /// </summary>
        /// <typeparam name="T">仓储类型</typeparam>
        /// <param name="uow">工作单元（Unit Of Work）</param>
        /// <returns></returns>
        protected virtual T GetRepository<T>(IUnitOfWork unitOfWork)
        {
            var list = new List<ResolvedParameter>();

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IRepositoryContext),
                                           (pi, ctx) => (IRepositoryContext)unitOfWork));

            list.Add(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ServiceContext),
                                                     (pi, ctx) => (ServiceContext)ServiceContext));

            return ServiceLocator.Instance.GetService<T>(list);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        protected virtual void SendMessage<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.Add<T>(args);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="yingyongId">应用id</param>
        /// <param name="canShuId">参数ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public string GetCanShu(string yingyongId, string canShuId, string defaultValue)
        {
            return ((IRepositoryContext)UnitOfWork).GetCanShu(yingyongId, canShuId, defaultValue);
        }

        /// <summary>
        /// 获取本应用参数
        /// </summary>
        /// <param name="canShuId">参数ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public string GetCanShu(string canShuId,string defaultValue)
        {
            return ((IRepositoryContext)UnitOfWork).GetCanShu(this.ServiceContext.YINGYONGID, canShuId, defaultValue);
        }
    }
}
