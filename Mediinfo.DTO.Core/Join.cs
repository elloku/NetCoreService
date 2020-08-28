using Mediinfo.Enterprise.Exceptions;

using System.Collections.Generic;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 构造join
    /// </summary>
    public class Join
    {
        /// <summary>
        /// join列表
        /// </summary>
        public List<KeyValuePair<Property, Property>> joins = new List<KeyValuePair<Property, Property>>();
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        internal Join()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        internal Join(Property left, Property right)
        {
            joins.Add(new KeyValuePair<Property, Property>(left, right));
        }

        /// <summary>
        /// 重写&操作符
        /// </summary>
        /// <param name="lwhere"></param>
        /// <param name="rwhere"></param>
        /// <returns></returns>
        public static Join operator &(Join lwhere, Join rwhere)
        {
            lwhere.joins.AddRange(rwhere.joins);
            return lwhere;
        }

        /// <summary>
        /// 重写|操作符
        /// </summary>
        /// <param name="lwhere"></param>
        /// <param name="rwhere"></param>
        /// <returns></returns>
        public static Join operator |(Join lwhere, Join rwhere)
        {
            throw new DTOException("该方法不能执行");
        }
    }
}