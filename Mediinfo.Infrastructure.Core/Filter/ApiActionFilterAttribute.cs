using Mediinfo.Enterprise;
using Mediinfo.Enterprise.Log;
using Mediinfo.Enterprise.Token;
using Mediinfo.Infrastructure.Core.Cache;
using Mediinfo.Infrastructure.Core.MessageQueue;
using Mediinfo.Infrastructure.Core.UnitOfWork;
using Mediinfo.Utility.Compress;
using Mediinfo.Utility.Extensions;
using Mediinfo.Utility.Util;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Mediinfo.Infrastructure.Core.Controller;
using Mediinfo.Utility;

namespace Mediinfo.Infrastructure.Core.Filter
{
    /// <summary>
    /// 服务请求拦截器
    /// </summary>
    public class ApiActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 服务执行前
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var tokens = filterContext.Request.Headers.Where(n => n.Key == "token");
            string token = null;
            if (tokens.Count() > 0)
            {
                token = tokens.FirstOrDefault().Value.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(token))
            {
                var payload = MediToken.GetTokenPayLoad(token);

                var userInfo = payload.AuthInfo;
                var controller = filterContext.ControllerContext.Controller;
                var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                // 用payload中的userid设置ServiceContext
                var serviceContextObj = serviceContextProperty.GetValue(controller);
                if (serviceContextObj != null)
                {
                    ServiceContext serviceContext = (ServiceContext)serviceContextObj;
                    serviceContext.USERID = userInfo.UserID;
                    serviceContextProperty.SetValue(controller, serviceContext);
                }
            }

            base.OnActionExecuting(filterContext);
        }
        /// <summary>
        /// 服务执行后
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override async void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var controller = actionExecutedContext.ActionContext.ControllerContext.Controller;
                var unitOfWorkProperty = controller.GetType().BaseType.GetField("_UnitOfWorks", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                var unitOfWorkDict = (Dictionary<string, IUnitOfWork>)unitOfWorkProperty.GetValue(controller);

                var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                ServiceContext serviceContext = serviceContextProperty.GetValue(controller) as ServiceContext;

                string fuWuCsStr = await actionExecutedContext.ActionContext.Request.Content.ReadAsStringAsync();

                // 获取服务完整地址信息
                string serviceUri = actionExecutedContext.ActionContext.Request.RequestUri.ToString();

                // 获取服务信息
                string pathAndQuery = actionExecutedContext.ActionContext.Request.RequestUri.PathAndQuery;
				string server = actionExecutedContext.ActionContext.Request.RequestUri.Host;
				string port = actionExecutedContext.ActionContext.Request.RequestUri.Port.ToString();
				string moKuaiMc = pathAndQuery.Split('/')[1];
                string yeWuMc = pathAndQuery.Split('/')[2];
                string caoZuoMc = pathAndQuery.Split('/')[3];

                StringBuilder sqlLogText = new StringBuilder();

                //ESLog eSLog = new ESLog();
                StringBuilder stringBuilder = new StringBuilder();

                #region 记录调用日志

                // 记录日志=====================================================================
                try
                {
                    SysLogEntity logEntity = new SysLogEntity();
                    logEntity.RiZhiID = Guid.NewGuid().ToString();
                    logEntity.ChuangJianSj = DateTime.Now.ToInvariantString("yyyy/MM/dd HH:mm:ss");
                    logEntity.RiZhiBt = serviceContext.USERNAME + "[" + serviceContext.USERID + "]成功调用了[" + moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc + "]服务。";

                    stringBuilder.AppendLine(serviceContext.USERNAME + "[" + serviceContext.USERID + "]成功调用了[" + server + ":" + port + "端口上的" + serviceUri + "]服务。");

					stringBuilder.AppendLine();
                    stringBuilder.AppendLine("参数列表：");
                    // 服务参数
                    var fuWuCsList = QueryUrl.GetData(fuWuCsStr);
                    foreach (var item in fuWuCsList)
                    {
                        stringBuilder.AppendLine(item.Key + "=" + Uri.UnescapeDataString(item.Value).DecompressString());
                    }

                    try
                    {
                        var res = await actionExecutedContext.Response.Content.ReadAsStringAsync();
                        ServiceResult serviceResult = JsonUtil.DeserializeToObject<ServiceResult>(res);
                        serviceResult.ReturnCode = serviceResult.ReturnCode.DecompressString();
                        serviceResult.ReturnMessage = serviceResult.ReturnMessage.DecompressString();
                        serviceResult.ExceptionContent = serviceResult.ExceptionContent.DecompressString();
                        serviceResult.Content = serviceResult.Content.DecompressString();

                        stringBuilder.AppendLine("返回内容：");
                        stringBuilder.AppendLine(JsonUtil.SerializeObject(serviceResult));
                    }
                    catch (Exception ex)
                    {
                        stringBuilder.AppendLine("返回内容：无，信息：" + ex.ToString());
                    }



                    logEntity.RiZhiNr = stringBuilder.ToString();
                    logEntity.FuWuMc = moKuaiMc + "/" + yeWuMc + "/" + caoZuoMc;
                    logEntity.QingQiuLy = serviceContext.DANGQIANCKMC;
                    // 日志类型：1.菜单打开，2.客户端异常，3.服务调用，4服务端异常，5.SQL日志，6.性能日志
                    logEntity.RiZhiLx = 3;
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
                    LogHelper.Intance.PutSysInfoLog(logEntity);
                    // 记录日志=====================================================================
                }
                catch (Exception e)
                {
                    LocalLog.WriteLog(this.GetType(), e);
                }

                #endregion

                List<string> unitOfWorkKeys = new List<string>(unitOfWorkDict.Keys);
                for (int i = 0; i < unitOfWorkDict.Values.Count; i++)
                {
                    IUnitOfWork unitOfWork = unitOfWorkDict[unitOfWorkKeys[i]];

                    #region 记录SQL日志

                    try
                    {
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
                    }
                    catch (Exception e)
                    {
                        LocalLog.WriteLog(this.GetType(), e);
                    }
                    // 记录日志=====================================================================

                    #endregion

                    //try
                    //{
                    //    unitOfWork.MessagePlugin?.Handler();
                    //}
                    //catch (Exception e)
                    //{
                    //    Enterprise.Log.LogHelper.Intance.Error("业务插件",e.Message, JsonUtil.SerializeObject(e));
                    //}

                    try
                    {


                        // 是否发送                  
                        if (unitOfWork != null && unitOfWork.CurrentMessager.IsPublish)
                        {
                            unitOfWork.CurrentMessager.Context = serviceContext;

                            unitOfWork.CurrentMessager.MoKuaiMc = moKuaiMc;
                            unitOfWork.CurrentMessager.YeWuMc = yeWuMc;
                            unitOfWork.CurrentMessager.CaoZuoMc = caoZuoMc;

                            // 发送消息
                            using (var client = MessageQueueClientFactory.CreateDbClient())
                            {
                                client.Publish(moKuaiMc, yeWuMc, caoZuoMc, unitOfWork.CurrentMessager);
                            }

                            //是否需要推送消息处理日志
                            bool tuiSong = unitOfWork.CurrentMessager.EntityNameList.ToList().Exists(d => Messager.DaiJianCeSTList.Contains(d));

                            // 将发送的消息异步记录到ES
                            // 需要先将消息内容序列化，否则异步序列化会因部分对象已释放而报错
                            string riZhiNr = JsonConvert.SerializeObject(unitOfWork.CurrentMessager);
                            string id = unitOfWork.CurrentMessager.ID;
                            await Task.Factory.StartNew(() =>
                             {
                                 LogHelper.Intance.Info("消息发送", "消息发送成功", riZhiNr, id);

                                 if (tuiSong)
                                 {
                                     dynamic obj = new System.Dynamic.ExpandoObject(); //动态类型字段 可读可写
                                     obj.ID = id;
                                     obj.Status = 0;//0表示未处理
                                     obj.ChuLiSJ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                     LogHelper.Intance.Info("消息处理", "消息处理完毕", JsonConvert.SerializeObject(obj), id);
                                 }
                             });
                        }
                    }
                    catch (Exception ex)
                    {
                        // 发送消息队列失败
                        throw ex;
                    }
                    finally
                    {
                        if (unitOfWork != null)
                        {
                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                        unitOfWork = null;
                    }
                }
                unitOfWorkDict.Clear();

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
            }
            catch (Exception ex)
            {
                var controller = actionExecutedContext.ActionContext.ControllerContext.Controller;
                var serviceContextProperty = controller.GetType().BaseType.GetProperty("ServiceContext", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                ServiceContext serviceContext = serviceContextProperty.GetValue(controller) as ServiceContext;
                // 本地日志
                LogHelper.Intance.Error("系统日志", "ApiActionFilterAttribute处理OnActionExecuted报错", "ApiActionFilterAttribute处理OnActionExecuted报错:" + ex.ToString() + "\r\n" + JsonUtil.SerializeObject(serviceContext));
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
