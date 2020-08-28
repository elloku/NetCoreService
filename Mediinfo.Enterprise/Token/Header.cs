namespace Mediinfo.Enterprise.Token
{
    /// <summary>
    /// 头部信息
    /// </summary>
    public class Header
    {
        /// <summary>
        /// token类型
        /// </summary>
        public string typ { get; set; }

        /// <summary>
        /// 加密算法
        /// </summary>
        public string alg { get; set; }
    }
}
