using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Mediinfo.Utility.Extensions
{
    /// <summary>
    /// 字符串的扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// （扩展方法）判断字符串是否可以转化为double类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDouble(this string source)
        {
            if (source == null)
                return false;

            return double.TryParse(source, out _);
        }

        /// <summary>
        /// （扩展方法）判断字符串是否可以转化为int类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInt(this string source)
        {
            if (source == null)
                return false;
            return int.TryParse(source, out _);
        }

        /// <summary>
        /// （扩展方法）字符串转Int
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt(this string source, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(source))
                return defaultValue;

            if (int.TryParse(source, out var result))
                return result;
            return 0;
        }

        /// <summary>
        /// （扩展方法）判断字符串是否可以转化为DateTime类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string source)
        {
            if (source == null)
                return false;
            DateTime result;
            return DateTime.TryParse(source, out result);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string sValue)
        {
            return (T)Enum.Parse(typeof(T), sValue, true);
        }

        /// <summary>
        /// （扩展方法）如果字符串为空则返回""
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToStringEx(this object s)
        {
            if (s == null)
                return "";
            else
                return s.ToString();
        }

        /// <summary>
        /// （扩展方法)判断字符串是否是由数字组成
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumber(this string s)
        {
            if (s == null)
                return false;
            else
                return Regex.IsMatch(s, "^[0-9]*$");
        }

        /// <summary>
        /// （扩展方法）判断字符串是否可以转化为decimal类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string source)
        {
            if (source == null)
                return false;
            decimal result;
            return decimal.TryParse(source, out result);
        }

        /// <summary>
        /// （扩展方法）将字符串转换成Decimal类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string source, decimal defaultValue = 0m)
        {
            if (source == null)
                return defaultValue;
            if (decimal.TryParse(source, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// （扩展方法）判断字符串为空转化为0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NullToZero(this object s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.ToStringEx()))
                return "0";
            return s.ToString();
        }
        /// <summary>
        /// (扩展方法)替换字符,下标从1开始
        /// </summary>
        /// <param name="source">需替换的字符串</param>
        /// <param name="weiShu">需替换的起始位</param>
        /// <param name="value">替换的值</param>
        /// <param name="changDu">需替换的长度</param>
        /// <returns></returns>
        public static string Replace(this string source, int weiShu, char value, int changDu = 1)
        {
            var sources = source.ToCharArray();
            weiShu--;
            for (int i = weiShu; i < weiShu + changDu; i++)
            {
                sources[i] = value;
            }
            return new string(sources);
        }

        /// <summary>
        /// (扩展方法)计算字符长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(this string str)
        {
            if (str.Length == 0 || string.IsNullOrWhiteSpace(str))
                return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            foreach (var t in s)
            {
                if (t == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }

        /// <summary>
        /// HTMLEntitiesEncode（HTMLEntities编码）
        /// add by zhukunpin
        /// </summary>
        /// xml在日志平台上记录的时候会丢失一些字符，需要编码下再记录日志信息
        /// <param name="text">需要转换的html文本</param>
        /// <returns>HTMLEntities编码后的文本</returns>
        public static string HtmlEntitiesEncode(this string text)
        {
            // 获取文本字符数组
            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();

            // 初始化输出结果
            StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                // 将指定的 Unicode 字符的值转换为等效的 32 位有符号整数
                int value = Convert.ToInt32(c);

                // 内码为127以下的字符为标准ASCII编码，不需要转换，否则做 &#{数字}; 方式转换
                if (value > 127)
                {
                    result.AppendFormat("&#{0};", value);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 中文的编码格式转换
        /// </summary>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEncoding(this string text,string name= "utf-8")
        {
            string temp = string.Empty;
            byte[] encodedBytes = Encoding.GetEncoding(name).GetBytes(text);
            return encodedBytes.Aggregate(temp, (current, b) => current + ("%" + b.ToString("X")));
        }
    }
}
