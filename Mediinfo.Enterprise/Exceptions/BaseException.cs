using Newtonsoft.Json;
using System;

namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// 系统异常基类
    /// </summary>
    public class BaseException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="parms">自定义参数</param>
        public BaseException(ReturnCode errorCode, string errorMessage, object[] parms = null):base(errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Parms = parms;
        }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public object[] Parms { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public ReturnCode ErrorCode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonProperty]
        public string ErrorMessage { get; set; }
    }
}
