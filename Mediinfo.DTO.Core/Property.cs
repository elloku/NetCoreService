using Mediinfo.Enterprise.Exceptions;

using System;
using System.Collections.Generic;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Property
    {
        public List<string> Aggregates { get; protected internal set; }
        public string Out { get; protected internal set; }
        public object Value { get; set; }
        public string ColumnName { get; protected internal set; }
        public string Null { get; protected internal set; }
        public string Descing { get; protected internal set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Property()
        {
            Aggregates = new List<string>();
        }

        /// <summary>
        /// 重写==操作符
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator ==(Property column, object value)
        {
            return CreateOperator(column, " = ", value);
        }

        /// <summary>
        /// 重写==操作符
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Join operator ==(Property column, Property value)
        {
            try
            {
                var join = new Join(new Property() { ColumnName = column.ColumnName, Out = column.Out }, new Property() { ColumnName = value.ColumnName, Out = value.Out });
                column.Out = null;
                value.Out = null;
                return join;
            }
            catch (Exception)
            {
                throw new DTOException(column.ColumnName + "==后的参数未赋值");
            }
            
        }

        /// <summary>
        /// 改方法将不能执行
        /// </summary>
        public static Join operator !=(Property column, Property value)
        {
            throw new DTOException("该方法不能执行");
        }

        /// <summary>
        /// 重写!=
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator !=(Property column, object value)
        {
            return CreateOperator(column, " != ", value);
        }

        /// <summary>
        /// 重写<
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator <(Property column, object value)
        {
            return CreateOperator(column, " < ", value);
        }

        /// <summary>
        /// 重写>
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator >(Property column, object value)
        {
            return CreateOperator(column, " > ", value);
        }

        /// <summary>
        /// 重写<=
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator <=(Property column, object value)
        {
            return CreateOperator(column, " <= ", value);
        }

        /// <summary>
        /// 重写>=
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Where operator >=(Property column, object value)
        {
            return CreateOperator(column, " >= ", value);
        }

        /// <summary>
        /// 创建操作符
        /// </summary>
        /// <param name="column"></param>
        /// <param name="operater"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Where CreateOperator(Property column, string operater, object value)
        {
            if (value != null)
            {
                if (value.GetType().IsValueType)
                {
                    return new Where(column, operater, value.ToString());
                }
                else if (value is string)
                {
                    if (value.ToString() == "sysdate")
                    {
                        return new Where(column, operater, "sysdate");
                    }
                    else
                    {
                        return new Where(column, operater, "'" + value + "'");
                    }
                }
                else if (value is DateTime)
                {
                    return new Where(column, operater, "TO_DATE('" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-mm-dd hh24:mi:ss')");
                }
                else if (value is Null)
                {
                    if (operater == " = ")
                    {
                        return new Where(column, " is ", "null");
                    }
                    else if (operater == " != ")
                    {
                        return new Where(column, " is not ", "null");
                    }
                    else
                    {
                        throw new DTOException("Null不支持除==或者!=意外的操作符");
                    }
                }
                else if (value is Value)
                {
                    return new Where(column, operater, null);
                }
                else
                {
                    throw new DTOException(column.ColumnName + "的数据类型不被支持");
                }
            }
            else
            {
                return new Where(column, operater, null);
            }
        }

        /// <summary>
        /// 构造like头部
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where StartLike(object value)
        {
            return new Where(this, " like ", "'%" + value + "'");
        }

        /// <summary>
        /// 构造like尾部
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where EndLike(object value)
        {
            return new Where(this, " like ", "'" + value + "%'");
        }

        /// <summary>
        /// 构造like
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where Like(object value)
        {
            return new Where(this, " like ", "'%" + value +"%'");
        }

        /// <summary>
        /// 构造in
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where In(string value)
        {
            return new Where(this, " in ", value);
        }

        /// <summary>
        /// 构造notin
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Where NotIn(string value)
        {
            return new Where(this, " not in ", value);
        }

        /// <summary>
        /// 构造Between
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Where Between(object start, object end)
        {
            if (start != null && start != null)
            {
                if (start.GetType() != end.GetType())
                {
                    throw new DTOException("输入参数");
                }

                if (start.GetType().IsValueType)
                {
                    return new Where(this, " between ", start + " and " + end);
                }
                else if (start is string)
                {
                    return new Where(this, " between ", "'" + start + "' and '" + end + "'");
                }
                else if (start is DateTime)
                {
                    return new Where(this, " between ", "TO_DATE('" + ((DateTime)start).ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-mm-dd hh24:mi:ss') and TO_DATE('" + ((DateTime)start).ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-mm-dd hh24:mi:ss')");
                }
                else
                {
                    throw new DTOException("不支持此数据类型");
                }
            }
            else
            {
                throw new DTOException("Between的参数不允许为空");
            }
        }

        /// <summary>
        /// 构造1=1
        /// </summary>
        /// <returns></returns>
        public static Where OneEqualOne()
        {
            return new Where(new Property() { ColumnName = "1" } , "=",  1.ToString());
        }

        /// <summary>
        /// 构造OutJoin
        /// </summary>
        /// <returns></returns>
        public Property OutJoin()
        {
            Out = "(+)";
            return this;
        }

        /// <summary>
        /// 构造Desc
        /// </summary>
        /// <returns></returns>
        public Property Desc()
        {
            Descing = "desc";
            return this;
        }

        /// <summary>
        /// 构造Sum
        /// </summary>
        /// <returns></returns>
        public Property Sum()
        {
            Aggregates.Add("Sum({0})");
            return this;
        }

        /// <summary>
        /// 构造Count
        /// </summary>
        /// <returns></returns>
        public Property Count()
        {
            Aggregates.Add("Count({0})");
            return this;
        }

        /// <summary>
        /// 构造Max
        /// </summary>
        /// <returns></returns>
        public Property Max()
        {
            Aggregates.Add("Max({0})");
            return this;
        }

        /// <summary>
        /// 构造Mix
        /// </summary>
        /// <returns></returns>
        public Property Mix()
        {
            Aggregates.Add("Mix({0})");
            return this;
        }

        /// <summary>
        /// 构造nvl
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Property nvl(object defaultValue)
        {
            if (defaultValue.GetType().IsValueType)
            {
                Aggregates.Add("nvl({0}, " + defaultValue + ")");
            }
            else
            {
                Aggregates.Add("nvl({0}, '" + defaultValue + "')");
            }
            return this;
        }

        /// <summary>
        /// 构造Substr
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Property Substr(int start, int length)
        {
            Aggregates.Add("substr({0}, " + start  + ", " + length + ")");
            return this;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return null;
            }
            return Value.ToString();
        }
    }
}
