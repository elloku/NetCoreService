namespace Mediinfo.Enterprise.Exceptions
{
    public class RepositoryException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public RepositoryException(string errorMessage, ReturnCode errorCode = ReturnCode.REPOSITORYERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
