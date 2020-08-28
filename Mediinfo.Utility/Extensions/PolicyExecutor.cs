using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mediinfo.Utility.Extensions
{
    /// <summary>
    /// 调用方法重试 
    /// </summary>
    public class PolicyExecutor
    {
        /// <summary>
        /// 参数带无返回值
        /// </summary>
        /// <typeparam name="T1">参数类型1</typeparam>
        /// <param name="action">执行的方法</param>
        /// <param name="arg1">参数1</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        public static void Execute<T1>(Action<T1> action, T1 arg1, TimeSpan retryInterval, int retryCount = 3)
        {
            TimeSpan[] timeSpans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                timeSpans[i] = retryInterval;
            }
            var policy = Policy.Handle<Exception>().WaitAndRetry(
               timeSpans, (ex, time) =>
               {
                   /*LogHelper.Default.Error("请求超时", "请求超时，正在重试...",
                          "请求超时，正在重试...失败原因：" + ex.ToString() + ",请求持续了(" + time.Seconds + ")秒发生错误或无响应后，进行了重试");*/
               });
            // 执行请求
            policy.Execute(() => action(arg1));
        }
        /// <summary>
        /// 一个参数带返回值
        /// </summary>
        /// <typeparam name="T1">>参数类型1</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">执行的方法</param>
        /// <param name="arg1">参数1</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        /// <returns>返回类型</returns>
        public static T Execute<T1, T>(Func<T1, T> func, T1 arg1, TimeSpan retryInterval, int retryCount = 3)
        {
            TimeSpan[] timeSpans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                timeSpans[i] = retryInterval;
            }
            var policy = Policy.Handle<Exception>().WaitAndRetry(
               timeSpans, (ex, time) =>
               {
                   /*LogHelper.Default.Error("请求超时", "请求超时，正在重试...",
                         "请求超时，正在重试...失败原因：" + ex.ToString() + ",请求持续了(" + time.Seconds + ")秒发生错误或无响应后，进行了重试");*/
               });
            // 执行请求
            return policy.Execute<T>(() => func(arg1));
        }
        /// <summary>
        /// 二个参数带返回值
        /// </summary>
        /// <typeparam name="T1">>参数类型1</typeparam>
        /// <typeparam name="T2">参数类型2</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">执行的方法</param>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        public static T Execute<T1, T2, T>(Func<T1, T2, T> func, T1 arg1, T2 arg2, TimeSpan retryInterval, int retryCount = 3)
        {
            TimeSpan[] timeSpans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                timeSpans[i] = retryInterval;
            }
            var policy = Policy.Handle<Exception>().WaitAndRetry(
               timeSpans, (ex, time) =>
               {
                  /* LogHelper.Default.Error("请求超时", "请求超时，正在重试...",
                        "请求超时，正在重试...失败原因：" + ex.ToString() + ",请求持续了(" + time.Seconds + ")秒发生错误或无响应后，进行了重试");*/
               });
            // 执行请求
            return policy.Execute<T>(() => func(arg1, arg2));
        }
        /// <summary>
        /// 三个参数带返回值
        /// </summary>
        /// <typeparam name="T1">>参数类型1</typeparam>
        /// <typeparam name="T2">参数类型2</typeparam>
        /// <typeparam name="T3">参数类型2</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">执行的方法</param>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3">参数2</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        public static T Execute<T1, T2, T3, T>(Func<T1, T2, T3, T> func, T1 arg1, T2 arg2, T3 arg3, TimeSpan retryInterval, int retryCount = 3)
        {
            TimeSpan[] timeSpans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                timeSpans[i] = retryInterval;
            }
            var policy = Policy.Handle<Exception>().WaitAndRetry(
               timeSpans, (ex, time) =>
               {
                  /* LogHelper.Default.Error("请求超时", "请求超时，正在重试...",
                        "请求超时，正在重试...失败原因：" + ex.ToString() + ",请求持续了(" + time.Seconds + ")秒发生错误或无响应后，进行了重试");*/
               });
            // 执行请求
            return policy.Execute<T>(() => func(arg1, arg2, arg3));
        }
        /// <summary>
        /// 四个参数带返回值
        /// </summary>
        /// <typeparam name="T1">>参数类型1</typeparam>
        /// <typeparam name="T2">参数类型2</typeparam>
        /// <typeparam name="T3">参数类型2</typeparam>
        /// <typeparam name="T4">参数类型2</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">执行的方法</param>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3">参数2</param>
        /// <param name="arg4">参数2</param>
        /// <param name="retryInterval">重试间隔</param>
        /// <param name="retryCount">重试次数</param>
        public static T Execute<T1, T2, T3, T4, T>(Func<T1, T2, T3, T4, T> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan retryInterval, int retryCount = 3)
        {
            TimeSpan[] timeSpans = new TimeSpan[retryCount];
            for (int i = 0; i < retryCount; i++)
            {
                timeSpans[i] = retryInterval;
            }
            var policy = Policy.Handle<Exception>().WaitAndRetry(
               timeSpans, (ex, time) =>
               {
                   /*LogHelper.Default.Error("请求超时", "请求超时，正在重试...",
                       "请求超时，正在重试...失败原因：" + ex.ToString() + ",请求持续了(" + time.Seconds + ")秒发生错误或无响应后，进行了重试");*/
               });
            // 执行请求
            return policy.Execute<T>(() => func(arg1, arg2, arg3, arg4));
        }
        /// <summary>
        /// 方法超时判断  
        /// </summary>
        /// <author>xieyz 2020 3.18 13:56</author>
        /// <typeparam name="T">出参类型</typeparam>
        /// <typeparam name="T1">入参类型</typeparam>
        /// <param name="ms">超时时间(毫秒)</param>
        /// <param name="func">委托方法</param>
        /// <param name="t">入参</param>
        /// <returns></returns>
        public static T TimeoutCheck<T, T1>(Func<T1, T> func, T1 t, int ms)
        {
            var wait = new ManualResetEvent(false);
            bool RunOK = false;
            var task = Task.Factory.StartNew<T>(() =>
            {
                var result = func.Invoke(t);
                RunOK = true;
                wait.Set();
                return result;
            });
            wait.WaitOne(ms);
            if (RunOK)
            {
                return task.Result;
            }
            else
            {
                return default(T);
            }
        }
    }
}
