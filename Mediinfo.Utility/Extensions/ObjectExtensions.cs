using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Mediinfo.Utility.Extensions
{
    /// <summary>
    /// Object的扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 对象为Null或者是空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this object obj)
        {
            if (obj == null)
                return true;

            return string.IsNullOrWhiteSpace(obj.ToString());
        }

        /// <summary>
        /// object转换decimal方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static decimal GetDecimal(this object obj, decimal DefaultValue = 0)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString())) return DefaultValue;
            if (decimal.TryParse(obj.ToString(), out var result))
                return result;
            return DefaultValue;
        }

        /// <summary>
        /// 是否为null或者dbnull
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDBNull(this object obj)
        {
            if (obj == null || obj.Equals(DBNull.Value))
                return true;

            return false;
        }

        /// <summary>
        /// 剪切字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SubString(this string str, int startIndex)
        {
            return !str.IsNullOrWhiteSpace() ? str.Substring(startIndex, str.Length - startIndex) : "";
        }

        /// <summary>
        /// 剪切字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubString(this string str, int startIndex, int length)
        {
            return !str.IsNullOrWhiteSpace() ? str.Substring(startIndex, length) : "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this object input, int defaultValue = 0)
        {
            return input.IsNullOrWhiteSpace() ? defaultValue : Convert.ToInt32(input);
        }

        /// <summary>
        /// convert to datetime
        /// </summary>
        /// <param name="aim"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string aim, DateTime defaultValue = default(DateTime))
        {
            if (DateTime.TryParse(aim, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// convert to bool
        /// </summary>
        /// <param name="aim"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string aim, bool defaultValue = false)
        {
            if (bool.TryParse(aim, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// convert to double
        /// </summary>
        /// <param name="aim"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string aim, double defaultValue = 0)
        {
            if (double.TryParse(aim, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// convert to specified number of double
        /// </summary>
        /// <param name="aim"></param>
        /// <param name="digits"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string aim, int digits, double defaultValue = 0)
        {
            return Math.Round(ToDouble(aim), digits);
        }

        /// <summary>
        /// convert to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aim"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable aim) where T : class, new()
        {
            var list = new List<T>();
            var type = typeof(T);
            var plist = new List<PropertyInfo>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                if (aim.Columns.IndexOf(item.Name) != -1)
                    plist.Add(item);
            }

            foreach (DataRow item in aim.Rows)
            {
                T t = new T();
                foreach (var proper in plist)
                {
                    if (item[proper.Name] != DBNull.Value)
                        proper.SetValue(t, item[proper.Name], null);
                }
                list.Add(t);
            }
            return list;
        }
    }
}
