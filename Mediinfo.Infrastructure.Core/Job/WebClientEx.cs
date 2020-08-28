using System;
using System.IO;
using System.Net;
using System.Text;

namespace Mediinfo.Infrastructure.Core.Job
{
    /// <summary>
    /// WebClient扩展
    /// </summary>
    public class WebClientEx : WebClient
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 请求是否出错
        /// </summary>
        public bool IsError = false;

        /// <summary>
        /// 错误详细消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timeout"></param>
        public WebClientEx(int timeout)
        {
            Timeout = timeout;
        }

        protected HttpWebRequest httpWebRequest;

        /// <summary>  
        /// 重写GetWebRequest,添加WebRequest对象超时时间  
        /// </summary>  
        /// <param name="address"></param>  
        /// <returns></returns>  
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;

            httpWebRequest = request;
            return request;
        }

        /// <summary>
        /// 重写GetWebResponse，用于处理错误消息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)base.GetWebResponse(request);
            }
            catch (WebException ex)
            {
                IsError = true;

                res = (HttpWebResponse)ex.Response;
                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                string strHtml = sr.ReadToEnd();
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                ErrorMessage = strHtml;
            }

            return res;
        }
    }
}
