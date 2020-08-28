using System;
using System.Diagnostics;

namespace Mediinfo.Utility.Extensions
{
    /// <summary>
    /// 性能调试时间的扩展方法
    /// </summary>
    public static class StopWatchExtensions
    {
        public static string ElapsedTimeString(this Stopwatch stopWatch)
        {
            return
                $"{stopWatch.Elapsed.Hours} Hours:{stopWatch.Elapsed.Minutes:00} Minutes:{stopWatch.Elapsed.Seconds:00} Seconds.{stopWatch.Elapsed.Milliseconds:00} Milliseconds";
        }
    }
}
