using Mediinfo.Enterprise;

using System.Linq;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 服务上下文创建者
    /// </summary>
    public class HISServiceContextCreater : IServiceContextCreater
    {
        /// <summary>
        /// 获取服务上下文
        /// </summary>
        /// <returns></returns>
        public ServiceContext GetServiceContext()
        {
            var context = new ServiceContext();

            // 通过HISClientHelper生成ServiceContext
            HISClientHelper hisClient = new HISClientHelper();
            var pros = typeof(HISClientHelper).GetProperties().ToList();
            typeof(ServiceContext).GetProperties().ToList().ForEach(o =>
            {
                var pro = pros.FirstOrDefault(p => p.Name.ToUpper() == o.Name.ToUpper());
                if (pro != null)
                {
                    var value = pro.GetValue(hisClient, null);
                    if (value != null)
                        o.SetValue(context, value, null);
                }
            });
            return context;
        }
    }
}
