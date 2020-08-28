namespace Mediinfo.Enterprise.Exceptions
{
    public class PluginsException : BaseException
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="errorCode">错误代码,默认为ReturnCode.CLIENTERROR</param>
        public PluginsException(string errorMessage, ReturnCode errorCode = ReturnCode.PLUGINSERROR) : base(errorCode, errorMessage)
        {

        }
    }
}
