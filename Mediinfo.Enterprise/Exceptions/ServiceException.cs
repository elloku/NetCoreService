namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// 服务异常
    /// </summary>
    public class ServiceException : BaseException
    {
        /// <summary>
        /// 服务异常构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码</param>
        public ServiceException(string errorMessage, ReturnCode errorCode = ReturnCode.SERVICEERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
