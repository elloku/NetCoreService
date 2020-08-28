using Mediinfo.Enterprise.Log;

using System.Threading.Tasks;

namespace Mediinfo.Enterprise.Exceptions
{
    /// <summary>
    /// https://stackoverflow.com/questions/7883052/a-tasks-exceptions-were-not-observed-either-by-waiting-on-the-task-or-accessi
    /// task 异常捕获并记录
    /// create by songxl on 2019-12-24
    /// </summary>
    public static class TaskException
    {
        public static void LogExceptions(this Task task)
        {
            task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        var aggException = t.Exception.Flatten();
                        foreach (var exception in aggException.InnerExceptions)
                        {
                            LogHelper.Intance.Error("线程异常", "线程异常", exception.Message);
                        }
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
