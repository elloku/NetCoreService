using System;
using System.IO;
using System.IO.Pipes;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 命名管道客户端
    /// </summary>
    public class NamedPipeClient : IDisposable
    {
        string _serverName;
        string _pipName;
        NamedPipeClientStream _pipeClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverName">服务器地址</param>
        /// <param name="pipName">管道名称</param>
        public NamedPipeClient(string serverName, string pipName)
        {
            _serverName = serverName;
            _pipName = pipName;

            _pipeClient = new NamedPipeClientStream(serverName, pipName, PipeDirection.InOut);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string Query(string request)
        {
            if (!_pipeClient.IsConnected)
            {
                _pipeClient.Connect(10000);
            }

            StreamWriter sw = new StreamWriter(_pipeClient);
            sw.WriteLine(request);
            sw.Flush();

            StreamReader sr = new StreamReader(_pipeClient);
            string temp;
            string returnVal = string.Empty;
            while ((temp = sr.ReadLine()) != null)
            {
                returnVal += temp;
            }
            
            return returnVal;
        }

        #region IDisposable 成员

        bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed && _pipeClient != null)
            {
                _pipeClient.Dispose();
                _disposed = true;
            }
        }
        
        #endregion
    }
}
