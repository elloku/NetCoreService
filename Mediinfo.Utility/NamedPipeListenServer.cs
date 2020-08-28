using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Mediinfo.Utility
{
    public class NamedPipeListenServer
    {
        public Func<string, string> OutPut = null;
        public Action<string> AfterDo = null;
        List<NamedPipeServerStream> _serverPool = new List<NamedPipeServerStream>();
        string _pipName = "test";
        public NamedPipeListenServer(string pipName)
        {
            _pipName = pipName;
        }

        /// <summary>
        /// 创建一个NamedPipeServerStream
        /// </summary>
        /// <returns></returns>
        protected NamedPipeServerStream CreateNamedPipeServerStream()
        {
            NamedPipeServerStream npss = new NamedPipeServerStream(_pipName, PipeDirection.InOut, 10);
            _serverPool.Add(npss);
            Debug.WriteLine("启动了一个NamedPipeServerStream " + npss.GetHashCode());
            return npss;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="npss"></param>
        protected void DistroyObject(NamedPipeServerStream npss)
        {
            npss.Close();
            if (_serverPool.Contains(npss))
            {
                _serverPool.Remove(npss);
            }
            Debug.WriteLine("销毁一个NamedPipeServerStream " + npss.GetHashCode());
        }

        public void Run() 
        {
            using (NamedPipeServerStream pipeServer = CreateNamedPipeServerStream())
            {
                pipeServer.WaitForConnection();
                Debug.WriteLine("建立一个连接 " + pipeServer.GetHashCode());

                Action act = new Action(Run);
                act.BeginInvoke(null, null);

                try
                {
                    bool isRun = true;
                    while (isRun)
                    {
                        string str = null;
                        StreamReader sr = new StreamReader(pipeServer);
                        while (pipeServer.CanRead && (null != (str = sr.ReadLine())))
                        {
                            ProcessMessage(str, pipeServer);

                            if (!pipeServer.IsConnected)
                            {
                                isRun = false;
                                break;
                            }
                        }

                        Thread.Sleep(50);
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
                finally
                {
                    DistroyObject(pipeServer);
                }
            }
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pipeServer"></param>
        protected virtual void ProcessMessage(string str, NamedPipeServerStream pipeServer)
        {
            // Read user input and send that to the client process.
            using (StreamWriter sw = new StreamWriter(pipeServer))
            {
                sw.AutoFlush = true;
                string outstring = string.Empty;
                if (OutPut != null)
                {
                    outstring = OutPut(str);
                }
                sw.WriteLine(outstring);
                if (AfterDo != null)
                {
                    AfterDo(outstring);
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            for (int i = 0; i < _serverPool.Count; i++)
            {
                var item = _serverPool[i];

                DistroyObject(item);
            }
        }
    }
}
