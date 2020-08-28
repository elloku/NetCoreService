using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Utility;
using Mediinfo.Utility.Util;

using System;

namespace Mediinfo.Enterprise.Token
{
    /// <summary>
    /// mediinfo token
    /// </summary>
    public class MediToken
    {
        /// <summary>
        /// MediToken的构造函数
        /// </summary>
        /// <param name="iss">签发者</param>
        /// <param name="aud">接收者</param>
        /// <param name="sub">面向的用户</param>
        /// <param name="authInfo">自定义用户信息</param>
        public MediToken(string iss, string aud, string sub, AuthInfo authInfo)
        {
            this.Header = new Header() { alg = "HS256", typ = "JWT" };
            this.Payload = new Payload()
            {
                exp = DateTime.MaxValue,
                nbf = DateTime.Now,
                iat = DateTime.Now,
                AuthInfo = authInfo,
                jti = Guid.NewGuid().ToString(),
                iss = iss,
                aud = aud,
                sub = sub
            };
        }

        /// <summary>
        /// 创建一个带有过期时间的token
        /// </summary>
        /// <param name="iss">签发者</param>
        /// <param name="aud">接收者</param>
        /// <param name="sub">面向的用户</param>
        /// <param name="exp"></param>
        /// <param name="authInfo">自定义用户信息</param>
        public MediToken(string iss, string aud, string sub, DateTime exp, AuthInfo authInfo)
        {
            this.Header = new Header() { alg = "HS256", typ = "JWT" };
            this.Payload = new Payload()
            {
                exp = exp,
                nbf = DateTime.Now,
                iat = DateTime.Now,
                AuthInfo = authInfo,
                jti = Guid.NewGuid().ToString(),
                iss = iss,
                aud = aud,
                sub = sub
            };
        }

        /// <summary>
        /// 头部信息
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// 载荷
        /// </summary>
        public Payload Payload { get; set; }

        /// <summary>
        /// 签证
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 加密盐
        /// </summary>
        public static string secret = "MediHis6";

        /// <summary>
        /// 生成并返回jwt token
        /// </summary>
        /// <returns></returns>
        public string CreateToken()
        {
            string header = Base64Util.Base64Encode(JsonUtil.SerializeObject(Header));
            string payload = Base64Util.Base64Encode(JsonUtil.SerializeObject(Payload));
            string signature = SHA256.Encrypt(SHA256.Encrypt(header + "." + payload) + secret);
            return header + "." + payload + "." + signature;
        }

        /// <summary>
        /// 验证token是否合法
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        private static bool VerifyToken(string jwtToken)
        {
            string[] ts = jwtToken.Split('.');
            if (ts.Length == 3)
            {
                if (ts[2] == SHA256.Encrypt(SHA256.Encrypt(ts[0] + "." + ts[1]) + secret))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据token获取PayLoad
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public static Payload GetTokenPayLoad(string jwtToken)
        {
            if (!VerifyToken(jwtToken))
            {
                throw new ServiceException("非法的Token!");
            }

            string payload = Base64Util.Base64Decode(jwtToken.Split('.')[1]);
            return JsonUtil.DeserializeToObject<Payload>(payload);
        }
    }
}
