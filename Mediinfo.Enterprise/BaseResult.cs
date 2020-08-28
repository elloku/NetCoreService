namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 服务端返回对象
    /// </summary>
    public abstract class BaseResult
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BaseResult()
        {
            // 默认成功
            ReturnCode = ReturnCode.SUCCESS;
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// 返回值的附加信息
        /// </summary>
        public string ReturnMessage { get; set; }

        /// <summary>
        /// 返回的详细错误信息
        /// </summary>
        public string ExceptionContent { get; set; }

        /// <summary>
        /// 错误信息的位置
        /// </summary>
        public string ExceptionPosition { get; set; }

        ///// <summary>
        ///// 主键值
        ///// </summary>
        //public string ID { get; set; }

        ///// <summary>
        ///// 新的主键值
        ///// </summary>
        //public List<string> NewOrders { get; set; }
    }
}
