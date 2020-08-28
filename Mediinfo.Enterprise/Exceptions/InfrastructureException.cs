namespace Mediinfo.Enterprise.Exceptions
{
    public class InfrastructureException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public InfrastructureException(string errorMessage, ReturnCode errorCode = ReturnCode.INFRASTRUCTUREERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
