using Mediinfo.Utility.Extensions;
using Mediinfo.Utility.Util;
using System;

namespace Mediinfo.Infrastructure.Core.Job
{
    /// <summary>
    /// 任务
    /// </summary>
    public class QuartZJob : IJob
    {
        public string DeleteJob(string JOB_ID, string JOB_NAME)
        {
            WebClientEx wc = new WebClientEx(36000);
            return wc.DownloadString(new Uri(@"http://127.0.0.1:9500/Job/DeleteJobRequest/" 
                + Base64Util.Base64Encode(JOB_ID) +
                "/" + Base64Util.Base64Encode(JOB_NAME)));
        }

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
        public string GenerateJob(string JOB_ID, string JOB_NAME,
            string MoKuaiMc, string YeWuMc, string CaoZuoMc,
            DateTime START_TIME,
            string CanShu = "{ }", int PRIORITY = 5)
        {
            WebClientEx wc = new WebClientEx(36000);
            return wc.DownloadString(new Uri(@"http://127.0.0.1:9500/Job/GenerateJob/"+ Base64Util.Base64Encode(JOB_ID) +
                "/"+ Base64Util.Base64Encode(JOB_NAME) + 
                "/"+ START_TIME.ToInvariantString("yyyy-MM-dd HH:mm:ss") +
                "/"+PRIORITY.ToString() +"/"+MoKuaiMc +
                "/"+YeWuMc +"/"+CaoZuoMc +
                "/"+ Base64Util.Base64Encode(CanShu)));
        }
    }
}
