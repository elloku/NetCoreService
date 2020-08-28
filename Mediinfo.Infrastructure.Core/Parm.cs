namespace Mediinfo.Infrastructure.Core
{
    /// <summary>
    /// 参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Parm<T> : BaseParm
    {
        /// <summary>
        /// 实体
        /// </summary>
        public T Entity { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity"></param>
        public Parm(T entity)
        {
            Entity = entity;
        }
    }
}
