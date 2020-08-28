using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Enterprise.Log;
using Mediinfo.Infrastructure.Core.Cache;
using Mediinfo.Infrastructure.Core.UnitOfWork;
using Mediinfo.Utility.Compress;
using Mediinfo.Utility.Extensions;
using Mediinfo.Utility.Util;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http.Filters;
using Mediinfo.Infrastructure.Core.Controller;

namespace Mediinfo.Infrastructure.Core.Filter
{
    /// <summary>
    /// 服务异常拦截器
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override async void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            StringBuilder sqlLogText = new StringBuilder();
            try
            {
                var controller = actionExecutedContext.ActionContext.ControllerContext.Controller;
                var unitOfWorkProperty = controller.GetType().BaseType.GetField("_UnitOfWorks", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                var unitOfWorkDict = (Dictionary<string, IUnitOfWork>)unitOfWorkProperty.GetValue(controller);

                var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                ServiceContext serviceContext = serviceContextProperty.GetValue(controller) as ServiceContext;

                string fuWuCsStr = await actionExecutedContext.ActionContext.Request.Content.ReadAsStringAsync();

                // 获取服务信息
                string pathAndQuery = actionExecutedContext.ActionContext.Request.RequestUri.PathAndQuery;
                string moKuaiMc = pathAndQuery.Split('/')[1];
                string yeWuMc = pathAndQuery.Split('/')[2];
                string caoZuoMc = pathAndQuery.Split('/')[3];


                //ESLog eSLog = new ESLog();


                List<string> unitOfWorkKeys = new List<string>(unitOfWorkDict.Keys);
                for (int i = 0; i < unitOfWorkDict.Values.Count; i++)
                {
                    IUnitOfWork unitOfWork = unitOfWorkDict[unitOfWorkKeys[i]];
                    try
                    {
                        #region 记录SQL日志

                        // 记录日志=====================================================================
                        if (!string.IsNullOrWhiteSpace(unitOfWork.SqlLog.ToString()))
                        {

                            sqlLogText.AppendLine(unitOfWork.SqlLog.ToString());

                            SysLogEntity sqlLogEntity = new SysLogEntity();
                            sqlLogEntity.RiZhiID = Guid.NewGuid().ToString();
                            sqlLogEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                            sqlLogEntity.RiZhiBt = "[" + moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc + "]服务运行期间的SQL记录。";
                            sqlLogEntity.RiZhiNr = unitOfWork.SqlLog.ToString();

                            sqlLogEntity.FuWuMc = moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc;
                            sqlLogEntity.QingQiuLy = serviceContext.DANGQIANCKMC;
                            // 日志类型：1.菜单打开，2.客户端异常，3.服务调用，4服务端异常，5.SQL日志，6.性能日志
                            sqlLogEntity.RiZhiLx = 5;

                            sqlLogEntity.YINGYONGID = serviceContext.YINGYONGID;
                            sqlLogEntity.XITONGID = serviceContext.XITONGID;
                            sqlLogEntity.YINGYONGMC = serviceContext.YINGYONGMC;
                            sqlLogEntity.YINGYONGJC = serviceContext.YINGYONGJC;
                            sqlLogEntity.VERSION = serviceContext.VERSION;
                            sqlLogEntity.IP = serviceContext.IP;
                            sqlLogEntity.MAC = serviceContext.MAC;
                            sqlLogEntity.COMPUTERNAME = serviceContext.COMPUTERNAME;
                            sqlLogEntity.USERNAME = serviceContext.USERNAME;
                            sqlLogEntity.USERID = serviceContext.USERID;
                            sqlLogEntity.KESHIID = serviceContext.KESHIID;
                            sqlLogEntity.KESHIMC = serviceContext.KESHIMC;
                            sqlLogEntity.BINGQUID = serviceContext.BINGQUID;
                            sqlLogEntity.BINGQUMC = serviceContext.BINGQUMC;
                            sqlLogEntity.JIUZHENKSID = serviceContext.JIUZHENKSID;
                            sqlLogEntity.JIUZHENKSMC = serviceContext.JiuZhenKSMC;
                            sqlLogEntity.YUANQUID = serviceContext.YUANQUID;
                            sqlLogEntity.GONGZUOZID = serviceContext.GONGZUOZID;
                            //eSLog.PutLog(sqlLogEntity);
                            LogHelper.Intance.PutSysInfoLog(sqlLogEntity);
                        }
                        // 记录日志=====================================================================

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                unitOfWorkKeys.Clear();

                #region 记录日志

                // 记录日志=====================================================================

                SysLogEntity logEntity = new SysLogEntity();
                logEntity.RiZhiID = Guid.NewGuid().ToString();
                logEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                logEntity.RiZhiBt = serviceContext.USERNAME + "[" + serviceContext.USERID + "]调用[" + moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc + "]服务发生错误。";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(serviceContext.USERNAME + "[" + serviceContext.USERID + "]调用[" + moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc + "]服务发生错误。");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("参数列表：");
                // 服务参数
                var fuWuCsList = QueryUrl.GetData(fuWuCsStr);
                foreach (var item in fuWuCsList)
                {
                    stringBuilder.AppendLine(item.Key + "=" + Uri.UnescapeDataString(item.Value).DecompressString());
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("异常信息：");

                if (actionExecutedContext.Exception is BaseException)
                {
                    var exct = (BaseException)actionExecutedContext.Exception;
                    stringBuilder.Append(exct.ErrorMessage);
                }

                stringBuilder.AppendLine(actionExecutedContext.Exception.ToString());
                stringBuilder.AppendLine("异常信息Json：");
                try
                {
                    stringBuilder.AppendLine(JsonUtil.SerializeObject(actionExecutedContext.Exception));
                }
                catch (Exception ex)
                {
                    stringBuilder.Append("转换json日志时发生错误：" + ex.ToString());
                }

                stringBuilder.AppendLine("SQL日志：");
                stringBuilder.AppendLine(sqlLogText.ToString());

                logEntity.RiZhiNr = stringBuilder.ToString();
                logEntity.FuWuMc = moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc;
                logEntity.QingQiuLy = serviceContext.DANGQIANCKMC;
                // 日志类型：1.菜单打开，2.客户端异常，3.服务调用，4服务端异常，5.SQL日志，6.性能日志
                logEntity.RiZhiLx = 4;
                logEntity.YINGYONGID = serviceContext.YINGYONGID;
                logEntity.XITONGID = serviceContext.XITONGID;
                logEntity.YINGYONGMC = serviceContext.YINGYONGMC;
                logEntity.YINGYONGJC = serviceContext.YINGYONGJC;
                logEntity.VERSION = serviceContext.VERSION;
                logEntity.IP = serviceContext.IP;
                logEntity.MAC = serviceContext.MAC;
                logEntity.COMPUTERNAME = serviceContext.COMPUTERNAME;
                logEntity.USERNAME = serviceContext.USERNAME;
                logEntity.USERID = serviceContext.USERID;
                logEntity.KESHIID = serviceContext.KESHIID;
                logEntity.KESHIMC = serviceContext.KESHIMC;
                logEntity.BINGQUID = serviceContext.BINGQUID;
                logEntity.BINGQUMC = serviceContext.BINGQUMC;
                logEntity.JIUZHENKSID = serviceContext.JIUZHENKSID;
                logEntity.JIUZHENKSMC = serviceContext.JiuZhenKSMC;
                logEntity.YUANQUID = serviceContext.YUANQUID;
                logEntity.GONGZUOZID = serviceContext.GONGZUOZID;
                //eSLog.PutLog(logEntity);
                LogHelper.Intance.PutSysErrorLog(logEntity);
                // 记录日志=====================================================================

                #endregion

                var requestContextCacheProperty = controller.GetType().BaseType.GetField("_RequestContextCache", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (requestContextCacheProperty != null)
                {
                    var requestContextCacheObj = requestContextCacheProperty.GetValue(controller);
                    if (requestContextCacheObj != null)
                    {
                        var requestContextCache = (ContextCache)requestContextCacheObj;
                        requestContextCache.Clear();
                        requestContextCache.Dispose();
                        requestContextCache = null;
                    }
                }

                // 如果截获异常为我们自定义，可以处理的异常则通过我们自己的规则处理
                ServiceResult serviceResult = new ServiceResult("-1", "服务器发生未知错误！", "服务器发生未知错误！");
                if (actionExecutedContext.Exception is ApplicationException)
                {
                    serviceResult = new ServiceResult("-1", actionExecutedContext.Exception.Message, actionExecutedContext.Exception.ToString() + "\n" + sqlLogText);
                }
                if (actionExecutedContext.Exception is UnauthorizedException)
                {
                    serviceResult = new ServiceResult("-5", actionExecutedContext.Exception.Message, actionExecutedContext.Exception.ToString() + "\n" + sqlLogText);
                }
                else if (actionExecutedContext.Exception is ServiceException)
                {
                    var exception = (ServiceException)actionExecutedContext.Exception;
                    serviceResult = new ServiceResult(exception.ErrorCode.ToString(), exception.ErrorMessage, JsonUtil.SerializeObject(actionExecutedContext.Exception) + "\n" + sqlLogText);
                }
                else if (actionExecutedContext.Exception is BaseException)
                {
                    var exception = (BaseException)actionExecutedContext.Exception;
                    serviceResult = new ServiceResult(exception.ErrorCode.ToString(), exception.ErrorMessage, JsonUtil.SerializeObject(actionExecutedContext.Exception) + "\n" + sqlLogText);
                }

                else
                {
                    serviceResult = new ServiceResult("-1", actionExecutedContext.Exception.Message, JsonUtil.SerializeObject(actionExecutedContext.Exception) + "\n" + sqlLogText);
                }

                // 记录服务调用耗时日志
                var ServiceStartTimeProperty = controller.GetType().BaseType.GetField("_ServiceStartTime", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (ServiceStartTimeProperty != null)
                {
                    var ServiceStartTimeObj = ServiceStartTimeProperty.GetValue(controller);
                    if (ServiceStartTimeObj != null)
                    {
                        var serviceStartTime = (long)ServiceStartTimeObj;
                        long nowTicks = DateTime.Now.Ticks;
                        DateTime dateTime = new DateTime(nowTicks - serviceStartTime);
                        float haoShi = (float)Math.Round((decimal)((nowTicks - serviceStartTime) / 10000000d), 4);

                        SysLogEntity logTimeEntity = new SysLogEntity
                        {
                            RiZhiID = Guid.NewGuid().ToString(),
                            ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss"),
                            RiZhiBt = serviceContext.USERNAME + "[" + serviceContext.USERID + "]调用[" + moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc + "]服务，总耗时：" + haoShi + "秒。",
                            RiZhiNr = "[服务调用耗时：" + haoShi + "秒] " + stringBuilder.ToString() + sqlLogText.ToString(),
                            FuWuHs = haoShi,  // 添加耗时
                            FuWuMc = moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc,
                            QingQiuLy = serviceContext.DANGQIANCKMC,
                            // 日志类型：1.菜单打开，2.客户端异常，3.服务调用，4服务端异常，5.SQL日志，6.性能日志
                            RiZhiLx = 6,
                            YINGYONGID = serviceContext.YINGYONGID,
                            XITONGID = serviceContext.XITONGID,
                            YINGYONGMC = serviceContext.YINGYONGMC,
                            YINGYONGJC = serviceContext.YINGYONGJC,
                            VERSION = serviceContext.VERSION,
                            IP = serviceContext.IP,
                            MAC = serviceContext.MAC,
                            COMPUTERNAME = serviceContext.COMPUTERNAME,
                            USERNAME = serviceContext.USERNAME,
                            USERID = serviceContext.USERID,
                            KESHIID = serviceContext.KESHIID,
                            KESHIMC = serviceContext.KESHIMC,
                            BINGQUID = serviceContext.BINGQUID,
                            BINGQUMC = serviceContext.BINGQUMC,
                            JIUZHENKSID = serviceContext.JIUZHENKSID,
                            JIUZHENKSMC = serviceContext.JiuZhenKSMC,
                            YUANQUID = serviceContext.YUANQUID,
                            GONGZUOZID = serviceContext.GONGZUOZID
                        };
                        //eSLog.PutLog(logTimeEntity);
                        LogHelper.Intance.PutSysInfoLog(logTimeEntity);
                    }
                }

                actionExecutedContext.Response =
                            actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK,
                                serviceResult);
            }
            catch (Exception ex)
            {
                try
                {
                    // 返回错误信息，并向客户端传递错误
                    var controller = actionExecutedContext.ActionContext.ControllerContext.Controller;
                    var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    ServiceContext serviceContext = serviceContextProperty.GetValue(controller) as ServiceContext;

                    var serviceResult = new ServiceResult("-1", actionExecutedContext.Exception.Message + "ApiExceptionFilterAttribute处理OnException报错：" + ex.Message, JsonUtil.SerializeObject(actionExecutedContext.Exception) + "\n" + JsonUtil.SerializeObject(ex) + "\n" + sqlLogText);

                    actionExecutedContext.Response =
                                actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK,
                                    serviceResult);
                    // 本地日志
                    LogHelper.Intance.Error("系统日志", "ApiExceptionFilterAttribute处理OnException报错", "ApiActionFilterAttribute处理OnActionExecuted报错:" + ex.ToString() + "\r\n" + JsonUtil.SerializeObject(serviceContext));

                }
                catch (Exception iex)
                {
                    // 增加捕获异常的异常的处理
                    LogHelper.Intance.Error("ApiExceptionFilterAttribute", "ApiExceptionFilterAttribute处理OnException报错，在异常处理中再次报错", "直接异常：" + ex.ToString() + ";捕获处理异常：" + iex.ToString());
                }
            }
        }
    }
}
