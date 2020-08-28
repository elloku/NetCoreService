namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 返回单个实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : BaseResult
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Result() : base()
        {

        }

        /// <summary>
        /// 返回成功的构造函数
        /// </summary>
        /// <param name="_Return">返回内容</param>
        public Result(T _Return)
        {
            ReturnCode = ReturnCode.SUCCESS;
            Return = _Return;
        }

        /// <summary>
        /// 返回错误消息的构造函数
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        public Result(ReturnCode returnCode, string returnMessage)
        {
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
        }

        /// <summary>
        /// 返回错误消息的构造函数附加结果
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误信息</param>
        /// <param name="_Return">结果</param>
        public Result(ReturnCode returnCode, string returnMessage, T _Return)
        {
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
            Return = _Return;
        }

        /// <summary>
        /// 返回详细错误信息的构造函数
        /// </summary>
        /// <param name="returnCode">错误码</param>
        /// <param name="returnMessage">错误消息</param>
        /// <param name="exceptionContent">详细错误消息</param>
        /// <param name="_Return">结果</param>
        public Result(ReturnCode returnCode, string returnMessage,string exceptionContent, T _Return)
        {
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
            Return = _Return;
            ExceptionContent = exceptionContent;
        }
        
        /// <summary>
        /// 存储结果
        /// </summary>
        public T Return { get; set; }
    }
}
