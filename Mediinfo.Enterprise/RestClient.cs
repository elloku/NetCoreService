using Mediinfo.Utility;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// Rest客户端
    /// </summary>
    public class RestClient
    {
        #region Delete方式

        private static string Delete(string uri, string data = "", string username = "", string pwd = "")
        {
            return CommonHttpRequest(uri, "DELETE", data, username, pwd);
        }

        #endregion

        #region Put方式

        private static string Put(string uri, string data, string username = "", string pwd = "")
        {
            return CommonHttpRequest(uri, "PUT", data, username, pwd);
        }

        #endregion

        #region POST方式实现
        private static string Post(string uri, string data, string username = "", string pwd = "")
        {
            return CommonHttpRequest(uri, "Post", data, username, pwd);
        }

        #endregion

        #region GET方式实现

        private static string Get(string uri, string username = "", string pwd = "")
        {
            return CommonHttpRequest(uri, "GET", username: username, pwd: pwd);
        }

        #endregion

        #region Delete方式

        private static void DeleteNoResult(string uri, string data = "", string username = "", string pwd = "")
        {
            Task.Factory.StartNew(() =>
            {
                CommonHttpRequest(uri, "DELETE", data, username, pwd);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        #endregion

        #region Put方式

        private static void PutNoResult(string uri, string data, string username = "", string pwd = "")
        {
            Task.Factory.StartNew(() =>
            {
                CommonHttpRequest(uri, "PUT", data, username, pwd);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        #endregion

        #region POST方式实现

        private static void PostNoResult(string uri, string data, string username = "", string pwd = "")
        {
            Task.Factory.StartNew(() =>
            {
                CommonHttpRequest(uri, "POST", data, username, pwd);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        #endregion

        #region GET方式实现

        private static void GetNoResult(string uri, string username = "", string pwd = "")
        {
            Task.Factory.StartNew(() =>
            {
                CommonHttpRequest(uri, "GET", username: username, pwd: pwd);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        #endregion

        #region  私有方法

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="type">请求类型</param>
        /// <param name="data">数据</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        private static string CommonHttpRequest(string url, string type, string data = "", string username = "", string pwd = "")
        {
            HttpWebRequest myRequest = null;
            Stream outstream = null;
            HttpWebResponse myResponse = null;
            StreamReader reader = null;
            try
            {
                // 构造http请求的对象
                myRequest = (HttpWebRequest)WebRequest.Create(url);


                // 设置
                myRequest.ProtocolVersion = HttpVersion.Version10;
                myRequest.Method = type;
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(pwd))
                    myRequest.Credentials = new NetworkCredential(username, pwd);
                if (data.Trim() != "")
                {
                    myRequest.ContentType = "application/json";
                    //myRequest.ContentLength = data.Length;
                    //myRequest.Headers.Add("data", data);

                    // 转成网络流
                    byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

                    outstream = myRequest.GetRequestStream();
                    outstream.Flush();
                    outstream.Write(buf, 0, buf.Length);
                    outstream.Flush();
                    outstream.Close();
                }
                // 获得接口返回值
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string ReturnXml = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
                myRequest.Abort();
                return ReturnXml;
            }
            catch (WebException ex)
            {
                var res = (HttpWebResponse)ex.Response;
                StreamReader sr;
                string strHtml = ex.Message;
                if (res != null)
                {
                    sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    strHtml = sr.ReadToEnd();
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                }

                if (outstream != null) outstream.Close();
                if (reader != null) reader.Close();
                if (myResponse != null) myResponse.Close();
                if (myRequest != null) myRequest.Abort();
                string msg = strHtml + Environment.NewLine + "请求地址:" + url;
                LocalLog.WriteLog(typeof(RestClient), msg);
                return "";
            }
        }

        #endregion

        #region 通用请求

        /// <summary>
        /// Http通用请求(同步方式)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="inputData"></param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public static string HttpRequest(string url, HttpType type, string inputData = "", string username = "", string pwd = "")
        {
            switch (type)
            {
                case HttpType.PUT:
                    return Put(url, inputData, username, pwd);
                case HttpType.GET:
                    return Get(url, username, pwd);
                case HttpType.POST:
                    return Post(url, inputData, username, pwd);
                case HttpType.DELETE:
                    return Delete(url, inputData, username, pwd);
                default:
                    break;
            }
            return "";
        }
        /// <summary>
        /// 异步方式请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="inputData"></param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        public static void HttpRequestNoResult(string url, HttpType type, string inputData = "", string username = "", string pwd = "")
        {
            switch (type)
            {
                case HttpType.PUT:
                    PutNoResult(url, inputData, username, pwd);
                    break;
                case HttpType.GET:
                    GetNoResult(url, username, pwd);
                    break;
                case HttpType.POST:
                    PostNoResult(url, inputData, username, pwd);
                    break;
                case HttpType.DELETE:
                    DeleteNoResult(url, inputData, username, pwd);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Http通用请求
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uri"></param>
        /// <param name="type"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string HttpRequest(string ip, string port, string uri, HttpType type, string inputData = "")
        {
            string url = "http://" + ip + ":" + port + uri;
            return HttpRequest(url, type, inputData);
        }

        #endregion
    }

    public enum HttpType
    {
        PUT = 0,
        GET = 1,
        POST = 2,
        DELETE = 3
    }
}
