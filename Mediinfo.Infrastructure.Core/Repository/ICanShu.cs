namespace Mediinfo.Infrastructure.Core.Repository
{
    /// <summary>
    /// 参数接口
    /// </summary>
    public interface ICanShu
    {
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="yingyongId">应用Id</param>
        /// <param name="canShuId">参数Id</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        string GetCanShu(string yingyongId, string canShuId, string defaultValue);
    }
}
