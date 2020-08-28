namespace Mediinfo.Enterprise.Exceptions
{
    public class CloudException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public CloudException(string errorMessage, ReturnCode errorCode = ReturnCode.CLOUDERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
