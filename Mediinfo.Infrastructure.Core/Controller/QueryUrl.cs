using System;
using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.Controller
{
    /// <summary>
    /// 查询的url
    /// </summary>
    public class QueryUrl
    {
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetData(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var splits = url.Split('&');
                foreach (var s in splits)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;
                    // 没有用String.Split防止某些少见Query String中出现多个=，要把后面的无法处理的=全部显示出来
                    var idx = s.IndexOf('=');
                    result.Add(Uri.UnescapeDataString(s.Substring(0, idx)), Uri.UnescapeDataString(s.Substring(idx + 1)));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new FormatException("URL格式错误", ex);
            }
        }
    }
}
