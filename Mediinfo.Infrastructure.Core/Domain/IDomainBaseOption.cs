using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediinfo.Infrastructure.Core.Domain
{
    public interface IDomainBaseOption
    {
        DateTime? GetSYSTime();
        /// <summary>
        /// 返回新的主键ID
        /// </summary>
        /// <param name="XuHaoMC">序号名称</param>
        /// <param name="Count">返回的主键数量</param>
        /// <returns></returns>
        List<string> GetOrder(string XuHaoMC, string QianZhui, int Count = 1);
    }
}
