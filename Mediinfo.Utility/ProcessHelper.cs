using System;
using System.Diagnostics;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 进程帮助类
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 根据进程ID关闭进程
        /// </summary>
        /// <param name="id">进程ID</param>
        public static void KillProcess(int id)
        {
            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {
                if (p.Id == id)
                {
                    for (int i = 0; i < p.Threads.Count; i++)
                        p.Threads[i].Dispose();
                    p.Kill();

                    break;
                }
            }
        }

        /// <summary>
        /// 根据进程名称关闭进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        public static void KillProcess(string processName)
        {
            foreach (Process thisproc in Process.GetProcessesByName(processName))
            {
                if (!thisproc.CloseMainWindow())
                {
                    thisproc.Kill();
                    GC.Collect();
                }
                Process[] prcs = Process.GetProcesses();
                foreach (Process p in prcs)
                {
                    if (p.ProcessName.Equals(processName))
                    {
                        p.Kill();
                    }
                }
            }
        }
    }
}
