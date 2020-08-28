using Mediinfo.Enterprise.Token;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// HIS访问令牌，本方法由不同系统实现不同的方式
    /// </summary>
    public class HISAccessToken : IAccessToken
    {
        private static string _Token = string.Empty;

        /// <summary>
        /// 设置token
        /// </summary>
        /// <param name="token"></param>
        public void SetToken(string token)
        {
            _Token = token;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            return _Token;
        }
    }
}
