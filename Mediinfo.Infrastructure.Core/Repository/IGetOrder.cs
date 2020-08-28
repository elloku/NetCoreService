using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 获取序号接口
    /// </summary>
    public interface IGetOrder
    {
        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="xuHaoMing">序号名称</param>
        /// <param name="qianZhui">前缀</param>
        /// <param name="count">获取的序号数量</param>
        /// <returns>序号列表</returns>
        List<string> GetOrder(string xuHaoMing, string qianZhui = null, int count = 1);
    }
}
