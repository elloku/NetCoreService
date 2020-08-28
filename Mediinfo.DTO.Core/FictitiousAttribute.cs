using System;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 虚拟列属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FictitiousAttribute : Attribute
    {
        public string DescribColmun { get; set; }
        /// <summary>
        /// 特性构造函数
        /// </summary>
        /// <param name="describColmun"></param>
        public FictitiousAttribute(string describColmun)
        {
            DescribColmun = describColmun;
        }
    }
}
