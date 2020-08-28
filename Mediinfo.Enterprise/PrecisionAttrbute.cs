using System;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 自定义Decimal的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DecimalPrecisionAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="precision">精度</param>
        /// <param name="scale">位数</param>
        public DecimalPrecisionAttribute(byte precision, byte scale)
        {
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// 精度
        /// </summary>
        public byte Precision { get; set; }

        /// <summary>
        /// 位数
        /// </summary>
        public byte Scale { get; set; }
    }
}
