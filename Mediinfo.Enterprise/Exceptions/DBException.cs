namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// 数据库异常
    /// </summary>
    public class DBException : BaseException
    {
        /// <summary>
        /// 数据库异常
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码，默认值：ReturnCode.DBERROR</param>
        public DBException(string errorMessage, ReturnCode errorCode = ReturnCode.DBERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
