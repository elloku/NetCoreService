using System;

namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 系统时间
    /// </summary>
    public interface ISYSTime
    {
        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        DateTime GetSYSTime();
    }
}
