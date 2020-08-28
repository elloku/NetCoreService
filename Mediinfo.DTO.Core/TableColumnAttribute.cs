using System;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// Table列特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        /// <summary>
        /// 特性构造函数
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        public TableColumnAttribute(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}
