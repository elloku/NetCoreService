using Newtonsoft.Json;
using System;

namespace Mediinfo.Enterprise.Token
{
    /// <summary>
    /// 载荷的实现
    /// </summary>
    public class Payload
    {
        /// <summary>
        /// jwt签发者
        /// </summary>
        public string iss { get; set; }

        /// <summary>
        /// jwt所面向的用户
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// 接收jwt的一方
        /// </summary>
        public string aud { get; set; }

        /// <summary>
        ///  jwt的过期时间，这个过期时间必须要大于签发时间
        /// </summary>
        public DateTime exp { get; set; }

        /// <summary>
        /// 定义在什么时间之前，该jwt都是不可用的.
        /// </summary>
        public DateTime nbf { get; set; }

        /// <summary>
        /// jwt的签发时间
        /// </summary>
        public DateTime iat { get; set; }

        /// <summary>
        /// jwt的唯一身份标识，主要用来作为一次性token,从而回避重放攻击
        /// </summary>
        public string jti { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonProperty]
        public AuthInfo AuthInfo { get; set; }
    }
}
