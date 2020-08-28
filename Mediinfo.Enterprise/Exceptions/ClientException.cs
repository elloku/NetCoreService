namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// 客户端异常
    /// </summary>
    public class ClientException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public ClientException(string errorMessage, ReturnCode errorCode = ReturnCode.CLIENTERROR) : base(errorCode, errorMessage)
        {

        }
    }
}