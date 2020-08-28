using Mediinfo.Utility.Compress;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 服务结果泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResult<T> : ServiceResult
    {
        /// <summary>
        /// 服务结果
        /// </summary>
        /// <param name="content">数据内容</param>
        public ServiceResult(T content) : base(content)
        {

        }

        /// <summary>
        /// 服务结果
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        public ServiceResult(string returnCode, string returnMessage)
            : base(returnCode, returnMessage)
        {

        }

        /// <summary>
        /// 服务结果
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        /// <param name="exceptionContent">详细错误内容</param>
        public ServiceResult(string returnCode, string returnMessage, string exceptionContent)
            : base(returnCode, returnMessage, exceptionContent)
        {

        }
    }

    /// <summary>
    /// 服务结果
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public string ReturnCode { get; set; }

        /// <summary>
        /// 返回值的附加信息
        /// </summary>
        public string ReturnMessage { get; set; }

        /// <summary>
        /// 详细错误消息
        /// </summary>
        public string ExceptionContent { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ServiceResult()
        {

        }

        /// <summary>
        /// 正常情况下的构造函数
        /// </summary>
        /// <param name="content">内容</param>
        public ServiceResult(object content)
        {
            ReturnCode = "0".ToCompress();
            ReturnMessage = "".ToCompress();
            Content = content.ToCompress();
            ExceptionContent = "".ToCompress();
        }

        /// <summary>
        /// 错误调用的构造函数
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        public ServiceResult(string returnCode, string returnMessage)
        {
            ReturnCode = returnCode.ToCompress();
            ReturnMessage = returnMessage.ToCompress();
            Content = "".ToCompress();
            ExceptionContent = "".ToCompress();
        }

        /// <summary>
        /// 错误调用的构造函数
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        /// <param name="exceptionContent">详细错误内容</param>
        public ServiceResult(string returnCode, string returnMessage, string exceptionContent)
        {
            ReturnCode = returnCode.ToCompress();
            ReturnMessage = returnMessage.ToCompress();
            Content = "".ToCompress();
            ExceptionContent = exceptionContent.ToCompress();
        }
    }
}
