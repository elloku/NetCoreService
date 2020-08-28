namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// Domain层异常
    /// </summary>
    public class DomainException : BaseException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码，默认值：ReturnCode.DOMAINERROR</param>
        public DomainException(string errorMessage, ReturnCode errorCode = ReturnCode.DOMAINERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
