namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// DTO异常
    /// </summary>
    public class DTOException : BaseException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码</param>
        public DTOException(string errorMessage, ReturnCode errorCode = ReturnCode.DTOERROR) : base(errorCode, errorMessage)
        {

        }
    }
}