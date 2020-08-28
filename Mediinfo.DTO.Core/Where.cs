using System;
using System.Collections.Generic;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// Where
    /// </summary>
    public class Where
    {
        public List<KeyValuePair<string, Tuple<Property, string, string>>> wheres;
        public Tuple<Property, string, string> where;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        internal Where()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="column"></param>
        /// <param name="operater"></param>
        /// <param name="value"></param>
        public Where(Property column, string operater, string value)
        {
            if (column.Value == null)
            {
                column.Value = new Value();
            }
            where = new Tuple<Property, string, string>(new Property() { ColumnName = column.ColumnName, Value = column.Value, Aggregates = column.Aggregates }, operater, value);
            column.Aggregates = new List<string>();
        }

        /// <summary>
        /// 重写&操作符
        /// </summary>
        /// <param name="lwhere"></param>
        /// <param name="rwhere"></param>
        /// <returns></returns>
        public static Where operator &(Where lwhere, Where rwhere)
        {
            var where = new Where();
            where.wheres = new List<KeyValuePair<string, Tuple<Property, string, string>>>();
            if (lwhere.where != null && rwhere.where != null)
            {
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, lwhere.where));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" and ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, rwhere.where));
            }
            else if (lwhere.where != null && rwhere.wheres != null)
            {
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, lwhere.where));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" and ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" (", null));
                where.wheres.AddRange(rwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(") ", null));
            }
            else if (lwhere.wheres != null && rwhere.where != null)
            {
                where.wheres.AddRange(lwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" and ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, rwhere.where));
            }
            else if (lwhere.wheres != null && rwhere.wheres != null)
            {
                where.wheres.AddRange(lwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" and ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" (", null));
                where.wheres.AddRange(rwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(") ", null));
            }
            return where;
        }

        /// <summary>
        /// 重写|操作符
        /// </summary>
        /// <param name="lwhere"></param>
        /// <param name="rwhere"></param>
        /// <returns></returns>
        public static Where operator |(Where lwhere, Where rwhere)
        {
            var where = new Where();
            where.wheres = new List<KeyValuePair<string, Tuple<Property, string, string>>>();
            if (lwhere.where != null && rwhere.where != null)
            {
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, lwhere.where));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" or ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, rwhere.where));
            }
            else if (lwhere.where != null && rwhere.wheres != null)
            {
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, lwhere.where));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" or ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" (", null));
                where.wheres.AddRange(rwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(") ", null));
            }
            else if (lwhere.wheres != null && rwhere.where != null)
            {
                where.wheres.AddRange(lwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" or ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(string.Empty, rwhere.where));
            }
            else if (lwhere.wheres != null && rwhere.wheres != null)
            {
                where.wheres.AddRange(lwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" or ", null));
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(" (", null));
                where.wheres.AddRange(rwhere.wheres);
                where.wheres.Add(new KeyValuePair<string, Tuple<Property, string, string>>(") ", null));
            }
            return where;
        }
    }
}
