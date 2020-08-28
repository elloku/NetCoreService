namespace Mediinfo.Enterprise.Token
{
    public interface IAccessToken
    {
        /// <summary>
        /// 设置token
        /// </summary>
        /// <param name="token"></param>
        void SetToken(string token);

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        string GetToken();
    }
}
