using System;
using System.Threading;

namespace Mediinfo.APIGateway.Core
{
    /// <summary>
    /// 超时函数
    /// </summary>
    public class FuncTimeout
    {
        /// <summary> 
        /// 信号量 
        /// </summary> 
        public ManualResetEvent manu = new ManualResetEvent(false);

        /// <summary> 
        /// 是否接受到信号 
        /// </summary> 
        public bool isGetSignal;

        /// <summary> 
        /// 设置超时时间 
        /// </summary> 
        public int timeout;

        /// <summary> 
        /// 要调用的方法的一个委托 
        /// </summary> 
        public Action FunctionNeedRun;

        /// <summary> 
        /// 构造函数，传入超时的时间以及运行的方法 
        /// </summary> 
        /// <param name="_action"></param> 
        /// <param name="_timeout"></param> 
        public FuncTimeout(Action _action, int _timeout)
        {
            FunctionNeedRun = _action;
            timeout = _timeout;
        }

        /// <summary> 
        /// 回调函数 
        /// </summary> 
        /// <param name="ar"></param> 
        public void MyAsyncCallback(IAsyncResult ar)
        {
            //isGetSignal为false,表示异步方法其实已经超出设置的时间，此时不再需要执行回调方法。 
            if (isGetSignal == false)
            {
                Console.WriteLine("放弃执行回调函数");
                Thread.CurrentThread.Abort();
            }
            else
            {
                
                Console.WriteLine("调用回调函数");
            }
        }

        /// <summary> 
        /// 调用函数 
        /// </summary> 
        /// <param name="param1"></param> 
        public void doAction()
        {
            Action WhatTodo = CombineActionAndManuset;
            // 通过BeginInvoke方法，在线程池上异步的执行方法。 
            var r = WhatTodo.BeginInvoke(MyAsyncCallback, null);
            // 设置阻塞,如果上述的BeginInvoke方法在timeout之前运行完毕，则manu会收到信号。此时isGetSignal为true。 
            // 如果timeout时间内，还未收到信号，即异步方法还未运行完毕，则isGetSignal为false。 
            isGetSignal = manu.WaitOne(timeout);

            if (isGetSignal == true)
            {
                Console.WriteLine("函数运行完毕，收到设置信号,异步执行未超时");
            }
            else
            {
                Console.WriteLine("没有收到设置信号,异步执行超时");
            }
        }

        /// <summary> 
        /// 把要传进来的方法，和 manu.Set()的方法合并到一个方法体。 
        /// action方法运行完毕后，设置信号量，以取消阻塞。 
        /// </summary> 
        /// <param name="obj"></param> 
        public void CombineActionAndManuset()
        {
            FunctionNeedRun();
            manu.Set();
        }
    }
}
