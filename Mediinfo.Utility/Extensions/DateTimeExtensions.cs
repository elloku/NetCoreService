using System;
using System.Collections.Generic;
using System.Text;

namespace Mediinfo.Utility.Extensions
{
    /// <summary>
    /// 时间的扩展方法
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// （扩展方法）将当前时间转换为所在天的最开始时间（00：00：00）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToBeginDateTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// （扩展方法）将当前时间转换为所在天的最开始时间（00：00：00）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime? ToBeginDateTime(this DateTime? dt)
        {
            if (dt == null)
                return null;

            return new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day);
        }

        /// <summary>
        /// （扩展方法）将当前时间转换为所在天的结束时间（23：59：59）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToEndDateTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 998);
        }

        /// <summary>
        /// （扩展方法）将当前时间转换为所在天的结束时间（23：59：59）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime? ToEndDateTime(this DateTime? dt)
        {
            if (dt == null)
                return null;

            return new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day, 23, 59, 59, 998);
        }

        /// <summary>
        /// datetime?时间转化datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime NullToDateTime(this DateTime? datetime)
        {
            try
            {
                if (datetime == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return Convert.ToDateTime(datetime);
                }
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// datetime 转化为 datetime?
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime? DateTimeToNullDateTime(this DateTime datetime)
        {
            try
            {
                return (DateTime?)datetime;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// datetime格式化
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this DateTime? dateTime, string format)
        {
            try
            {
                if (dateTime == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToDateTime(dateTime).ToString(format);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 格式化日期，不依赖本机的语言和区域设置（不对格式字符进行转义）
        /// </summary>
        /// <param name="dateTime">需要格式化的日期</param>
        /// <param name="format">格式字符</param>
        /// <returns></returns>
        public static string ToInvariantString(this DateTime dateTime, string format)
        {
            try
            {
                return dateTime.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 格式化日期，不依赖本机的语言和区域设置（不对格式字符进行转义）
        /// </summary>
        /// <param name="dateTime">需要格式化的日期</param>
        /// <param name="format">格式字符</param>
        /// <returns></returns>
        public static string ToInvariantString(this DateTime? dateTime, string format)
        {
            try
            {
                if (dateTime == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Convert.ToDateTime(dateTime).ToString(format, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得某月的第一天 
        /// </summary> 
        /// <param name="datetime">要取得月份第一天的时间</param> 
        /// <returns></returns> 
        public static DateTime FirstDayOfMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /// <summary>
        /// 取得某月的最后一天 
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 取得上个月的最后一天 
        /// </summary>
        /// <param name="datetime">要取得上个月最后一天的当前时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfPrdviousMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }
    }
}
