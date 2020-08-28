namespace Mediinfo.Enterprise.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public UnauthorizedException(string errorMessage, ReturnCode errorCode = ReturnCode.UNAUTHORIZED) : base(errorCode, errorMessage)
        {

        }
    }
}
