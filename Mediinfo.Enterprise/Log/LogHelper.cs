using Mediinfo.Utility;
using Mediinfo.Utility.Extensions;

using System;

namespace Mediinfo.Enterprise.Log
{
    /// <summary>
    /// 日志信息分级别操作
    /// </summary>
    public class LogHelper
    {
        private ESLog eSLog;

        private static LogHelper _LogHelper = new LogHelper();

        // 自定义日志级别 默认所有自定义日志都写
        private static int zdyRiZhi = 0;
        // 系统日志级别 默认系统日志都写
        private static int xtRiZhi = 0;
        public static void InitialRiZhiKZ(string zdyRiZhiKz, string xtRiZhiKz, string localip)
        {
            int index1 = zdyRiZhiKz.IndexOf("：");
            string[] zdyips = zdyRiZhiKz.SubString(0, index1).Split('-');
            bool isZdyIp = CheckIpInRange(localip, zdyips[0], zdyips[1]);
            if (isZdyIp)
            {
                zdyRiZhi = int.Parse(zdyRiZhiKz.SubString(index1 + 1));
            }

            int index2 = xtRiZhiKz.IndexOf("：");
            string[] xtips = xtRiZhiKz.SubString(0, index2).Split('-');
            bool isXtIp = CheckIpInRange(localip, xtips[0], xtips[1]);
            if (isXtIp)
            {
                xtRiZhi = int.Parse(xtRiZhiKz.SubString(index2 + 1));
            }
        }

        /// <summary>
        /// 判断IP是否在区间内
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="startIp"></param>
        /// <param name="endIp"></param>
        /// <returns></returns>
        public static bool CheckIpInRange(string ip, string startIp, string endIp)
        {
            long start = IP2Long(startIp);
            long end = IP2Long(endIp);
            long ipAddress = IP2Long(ip);
            bool inRange = (ipAddress >= start && ipAddress <= end);
            return inRange;
        }
        /// <summary>
        /// 把IP地址转成数字
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long IP2Long(string ip)
        {
            try
            {
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                string[] ipBytes;
                double num = 0;
                if (!string.IsNullOrEmpty(ip))
                {
                    ipBytes = ip.Split('.');
                    for (int i = ipBytes.Length - 1; i >= 0; i--)
                    {
                        num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                    }
                }
                return (long)num;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("[" + ip + "]IP地址转换数字报错：" + ex.ToString());
            }
        }
        private LogHelper()
        {
            eSLog = new ESLog();            
        }

        /// <summary>
        /// 实现单例模式
        /// </summary>
        public static LogHelper Intance
        {
            get
            {
                return _LogHelper;
            }
        }

        /// <summary>
        /// 提交警告日志信息
        /// </summary>
        /// <param name="suoYin">索引</param>
        /// <param name="riZhiBt">日志标题</param>
        /// <param name="riZhiNr">日志内容</param>
        public void Warn(string suoYin, string riZhiBt, string riZhiNr)
        {
            if (zdyRiZhi < 1) return;

            LocalLog.WriteLog(this.GetType(), riZhiNr);
        
            try
            {
                LogEntity logEntity = new LogEntity();
                logEntity.RiZhiID = Guid.NewGuid().ToString();
                logEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                logEntity.SuoYin = suoYin;
                logEntity.RiZhiBt = riZhiBt;
                logEntity.RiZhiNr = riZhiNr;
                logEntity.RiZhiJb = "WARN";
                eSLog.PutLog(logEntity);
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 提交日志信息
        /// </summary>
        /// <param name="suoYin">索引</param>
        /// <param name="riZhiBt">日志标题</param>
        /// <param name="riZhiNr">日志内容</param>
        /// <param name="id">ID</param>
        public void Info(string suoYin, string riZhiBt, string riZhiNr, string id = "")
        {
            if (zdyRiZhi < 2) return;

            LocalLog.WriteLog(this.GetType(), riZhiNr);
           
            try
            {
                LogEntity logEntity = new LogEntity();
                logEntity.RiZhiID = Guid.NewGuid().ToString();
                logEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                logEntity.SuoYin = suoYin;
                logEntity.RiZhiBt = riZhiBt;
                logEntity.RiZhiNr = riZhiNr.HtmlEntitiesEncode();
                logEntity.RiZhiJb = "INFO";
                eSLog.PutLog(logEntity, id);
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 提交错误日志信息
        /// </summary>
        /// <param name="suoYin">索引</param>
        /// <param name="riZhiBt">日志标题</param>
        /// <param name="riZhiNr">日志内容</param>
        public void Error(string suoYin, string riZhiBt, string riZhiNr)
        {
            if (zdyRiZhi < 0) return;

            LocalLog.WriteLog(this.GetType(), riZhiNr);
          
            try
            {
                LogEntity logEntity = new LogEntity();
                logEntity.RiZhiID = Guid.NewGuid().ToString();
                logEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                logEntity.SuoYin = suoYin;
                logEntity.RiZhiBt = riZhiBt;
                logEntity.RiZhiNr = riZhiNr;
                logEntity.RiZhiJb = "ERROR";
                eSLog.PutLog(logEntity);
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 写入系统标准日志
        /// </summary>
        public void PutSysErrorLog(SysLogEntity logEntity)
        {
            if (xtRiZhi < 0) return;
            try
            {
                eSLog.PutLog(logEntity);
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog(this.GetType(), ex);
            }

        }
        public void PutSysInfoLog(SysLogEntity logEntity)
        {
            if (xtRiZhi < 1) return;
            try
            {
                eSLog.PutLog(logEntity);
            }
            catch (Exception ex)
            {
                LocalLog.WriteLog(this.GetType(), ex);
            }
        }

    }
}
