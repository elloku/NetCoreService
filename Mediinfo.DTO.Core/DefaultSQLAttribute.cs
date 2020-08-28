using System;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 默认SQL特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultSQLAttribute : Attribute
    {
        public string SQL { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sql">sql</param>
        public DefaultSQLAttribute(string sql)
        {
            SQL = sql;
        }
    }
}