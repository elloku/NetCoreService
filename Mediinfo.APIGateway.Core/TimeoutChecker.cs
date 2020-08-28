using System;
using System.Threading;

namespace Mediinfo.APIGateway.Core
{
    /// <summary>
    /// 超时监测
    /// </summary>
    public class TimeoutChecker : IDisposable
    {
        long _timeout;              // 超时时间  
        Action<Delegate> _proc;               // 会超时的代码  
        Action<Delegate> _procHandle;         // 处理超时  
        Action<Delegate> _timeoutHandle;      // 超时后处理事件  
        ManualResetEvent _event = new ManualResetEvent(false);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="timeoutHandle"></param>
        public TimeoutChecker(System.Action<Delegate> proc, System.Action<Delegate> timeoutHandle)
        {
            this._proc = proc;
            this._timeoutHandle = timeoutHandle;
            this._procHandle = delegate
            {
                // 计算代码执行的时间  
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                if (this._proc != null)
                    this._proc(null);
                sw.Stop();
                // 如果执行时间小于超时时间则通知用户线程  
                if (sw.ElapsedMilliseconds < this._timeout && this._event != null)
                {
                    this._event.Set();
                }
            };
        }

        /// <summary>
        /// 等待时间
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Wait(long timeout)
        {
            this._timeout = timeout;
            // 异步执行  
            this._procHandle.BeginInvoke(null, null, null);
            // 如果在规定时间内没等到通知则为 false  
            bool flag = this._event.WaitOne((int)timeout, false);
            if (!flag)
            {
                // 触发超时时间  
                this._timeoutHandle?.Invoke(null);
            }
            this.Dispose();

            return flag;
        }

        /// <summary>
        /// 析构
        /// </summary>
        private void Dispose()
        {
            if (this._event != null)
                this._event.Close();
            this._event = null;
            this._proc = null;
            this._procHandle = null;
            this._timeoutHandle = null;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
