using System;

namespace Mediinfo.Infrastructure.Core.Job
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// 生成任务调度
        /// </summary>
        /// <param name="JOB_ID">任务唯一主键（TJ-121322842348234235）</param>
        /// <param name="JOB_NAME">任务名称（示例；药品调价）</param>
        /// <param name="START_TIME">开始时间(示例：2018-12-12 16:29:51)</param>
        /// <param name="PRIORITY">优先级（1-10）</param>
        /// <param name="MoKuaiMc">模块名称（示例：HIS-YKF-PanCun）</param>
        /// <param name="YeWuMc">业务名称（示例：YKPanCun）</param>
        /// <param name="CaoZuoMc">操作名称（示例：GetTiJiaoPCD）</param>
        /// <param name="CanShu">参数（示例：{ "panCunDID":"1" }）</param>
        /// <returns>返回值为【ok】代表正常，其他代表异常</returns>
        string GenerateJob(string JOB_ID, string JOB_NAME,
            string MoKuaiMc, string YeWuMc, string CaoZuoMc,
            DateTime START_TIME,
            string CanShu = "{ }", int PRIORITY = 5);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="JOB_ID">任务ID</param>
        /// <param name="JOB_NAME">任务名称</param>
        /// <returns></returns>
        string DeleteJob(string JOB_ID, string JOB_NAME);
    }
}
