namespace Mediinfo.Infrastructure.Core.Job
{
    /// <summary>
    /// 任务工厂
    /// </summary>
    public class JobFactory
    {
        /// <summary>
        /// 任务创建
        /// </summary>
        /// <returns></returns>
        public static IJob Builder()
        {
            return new QuartZJob();
        }
    }
}
