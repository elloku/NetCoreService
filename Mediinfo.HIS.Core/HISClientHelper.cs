using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 客户端配置辅助类（只供四层，五层使用，三层的Service不要调用）
    /// </summary>
    public class HISClientHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static HISClientHelper()
        {
            GlobalSetting = HISGlobalSetting.Load();
            ClientSetting = HISClientSetting.Load();

            //Stopwatch watch = new Stopwatch();
            //watch.e
        }

        /// <summary>
        /// 院区ID
        /// </summary>
        [Description("院区ID")]
        public static string YUANQUID { get; set; }
        
        /// <summary>
        /// 院区名称
        /// </summary>
        [Description("院区名称")]
        public static string YUANQUMC { get; set; }

        /// <summary>
        /// 医院名称
        /// </summary>
        [Description("医院名称")]
        public static string YIYUANMC { get; set; }

        /// <summary>
        /// 医院简称
        /// </summary>
        [Description("医院简称")]
        public static string YIYUANJC { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [Description("应用ID")]
        public static string YINGYONGID { get; set; }

        /// <summary>
        /// 库存应用ID
        /// </summary>
        [Description("库存应用ID")]
        public static string KUCUNYYID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Description("应用名称")]
        public static string YINGYONGMC { get; set; }

        /// <summary>
        /// 应用简称
        /// </summary>
        [Description("应用简称")]
        public static string YINGYONGJC { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        [Description("系统ID")]
        public static string XITONGID { get; set; }

        /// <summary>
        /// 临床标志
        /// </summary>
        [Description("临床标志")]
        public static int? LINCHUANGBZ { get; set; }

        /// <summary>
        /// 门诊价格体系
        /// </summary>
        [Description("门诊价格体系")]
        public static int? MENZHENJGTX { get; set; }

        /// <summary>
        /// 住院价格体系
        /// </summary>
        [Description("住院价格体系")]
        public static int? ZHUYUANJGTX { get; set; }

        /// <summary>
        /// 急诊病区类型
        /// </summary>
        [Description("急诊病区类型")]
        public static int? JIZHENBQLX { get; set; }

        /// <summary>
        /// 计算机名
        /// </summary>
        [Description("计算机名")]
        public static string COMPUTERNAME { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [Description("IP地址")]
        public static string IP { get; set; }

        /// <summary>
        /// 网卡地址
        /// </summary>
        [Description("网卡地址")]
        public static string MAC { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public static string USERIDNEW { get; set; }

        /// <summary>
        /// 职工ID
        /// </summary>
        [Description("职工ID")]
        public static string USERID { get; set; }

        /// <summary>
        /// 职工密码
        /// </summary>
        [Description("职工密码")]
        public static string USERPWD { get; set; }

        /// <summary>
        /// 职工姓名
        /// </summary>
        [Description("职工姓名")]
        public static string USERNAME { get; set; }

        /// <summary>
        /// 职工工号
        /// </summary>
        [Description("职工工号")]
        public static string ZHIGONGGH { get; set; }
        
        /// <summary>
        /// 医生等级
        /// </summary>
        [Description("医生等级")]
        public static string YISHENGDJ { get; set; }

        /// <summary>
        /// 输入码类型
        /// </summary>
        [Description("输入码类型")]
        public static string SHURUMLX { get; set; }

        /// <summary>
        /// 职工表上的科室ID
        /// </summary>
        [Description("职工表上的科室ID")]
        public static string KESHIID { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        [Description("科室名称")]
        public static string KESHIMC { get; set; }

        /// <summary>
        /// 病区ID
        /// </summary>
        [Description("病区ID")]
        public static string BINGQUID { get; set; }

        /// <summary>
        /// 病区名称
        /// </summary>
        [Description("病区名称")]
        public static string BINGQUMC { get; set; }

        /// <summary>
        /// 就诊科室id
        /// </summary>
        [Description("就诊科室ID")]
        public static string JIUZHENKSID { get; set; }

        /// <summary>
        /// 就诊科室名称
        /// </summary>
        [Description("就诊科室名称")]
        public static string JIUZHENKSMC { get; set; }

        /// <summary>
        /// 就诊科室上级科室
        /// </summary>
        [Description("就诊科室上级科室")]
        public static string JIUZHENKSSJYWKS { get; set; }

        /// <summary>
        /// 就诊挂号类别
        /// </summary>
        [Description("就诊挂号类别")]
        public static string JIUZHENGHLB { get; set; }

        /// <summary>
        /// 坐诊类型
        /// </summary>
        [Description("坐诊类型")]
        public static string ZUOZHENLX { get; set; }
        
        /// <summary>
        /// 职工对应的核算科室
        /// </summary>
        [Description("职工对应的核算科室")]
        public static string HESUANKS { get; set; }

        /// <summary>
        /// 职工对应的核算科室名称
        /// </summary>
        [Description("职工对应的核算科室")]
        public static string HESUANKSMC { get; set; }

        /// <summary>
        /// 职工对应的人事科室
        /// </summary>
        [Description("职工对应的人事科室")]
        public static string RENSHIKS { get; set; }

        /// <summary>
        /// 公用应用中的科室ID
        /// </summary>
        [Description("公用应用中的科室ID")]
        public static string YINGYONGKSID { get; set; }

        /// <summary>
        /// 当前科室（对于需要选择科室的系统，为用户当前选择的登录科室）
        /// </summary>
        [Description("当前科室")]
        public static string DANGQIANKS { get; set; }

        /// <summary>
        /// 当前科室名称（对于需要选择科室的系统，为用户当前选择的登录科室）
        /// </summary>
        [Description("当前科室名称")]
        public static string DANGQIANKSMC { get; set; }

        /// <summary>
        /// 当前病区（对于需要选择病区的系统，为用户当前选择的登录病区）
        /// </summary>
        [Description("当前病区")]
        public static string DANGQIANBQ { get; set; }

        /// <summary>
        /// 当前病区名称（对于需要选择病区的系统，为用户当前选择的登录病区）
        /// </summary>
        [Description("当前病区名称")]
        public static string DANGQIANBQMC { get; set; }

        /// <summary>
        /// 电子病历科室
        /// </summary>
        [Description("电子病历科室")]
        public static string DIANZIBLKS { get; set; }

        /// <summary>
        /// 工作站ID
        /// </summary>
        [Description("工作站ID")]
        public static string GONGZUOZID { get; set; }

        /// <summary>
        /// 程序路径
        /// </summary>
        [Description("程序路径")]
        public static string EXEPATH { get; set; }

        /// <summary>
        /// 系统版本号
        /// </summary>
        [Description("系统版本号")]
        public static string VERSION { get; set; }

        /// <summary>
        /// 库房_库存管理类型
        /// </summary>
        [Description("库房_库存管理类型")]
        public static string KUCUNGLLX { get; set; }

        /// <summary>
        /// 库管理账簿类别
        /// </summary>
        [Description("库管理账簿类别")]
        public static string YK_GuanLiZBLB { get; set; }

        /// <summary>
        /// 药房窗口ID
        /// </summary>
        [Description("药房窗口ID")]
        public static string CHUANGKOUID { get; set; }

        /// <summary>
        /// 药房窗口ID
        /// </summary>
        [Description("药房窗口ID")]
        public static string CHUANGKOUMC { get; set; }

        /// <summary>
        /// 床位组ID(无相关数据则设为*)
        /// </summary>
        [Description("床位组ID")]
        public static string CHUANGWEIZID { get; set; }

        /// <summary>
        /// 床位组名称
        /// </summary>
        [Description("床位组名称")]
        public static string CHUANGWEIZMC { get; set; }

        /// <summary>
        /// 病人住院ID
        /// </summary>
        [Description("病人住院ID")]
        public static string BINGRENZYID { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        [Description("功能ID")]
        public static string GONGNENGID { get; set; }

        /// <summary>
        /// 自定义模糊查询
        /// </summary>
        [Description("自定义模糊查询")]
        public static string MoHuCX
        {
            get
            {
                if (ClientSetting != null && !string.IsNullOrWhiteSpace(USERID) && ClientSetting.UserConfigList != null && ClientSetting.UserConfigList.Count > 0)
                {
                    var find = ClientSetting.UserConfigList.FirstOrDefault(o => o.ZhiGongID == USERID);
                    if (find != null)
                        return find.MoHuCX;
                }
                return null;
            }
            set
            {
                if (ClientSetting != null && !string.IsNullOrWhiteSpace(USERID))
                {
                    if (ClientSetting.UserConfigList == null)
                        ClientSetting.UserConfigList = new List<UserConfig>();
                    var find = ClientSetting.UserConfigList.FirstOrDefault(o => o.ZhiGongID == USERID);
                    if (find != null)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            ClientSetting.UserConfigList.Remove(find);
                            ClientSetting.Save();
                        }
                        else
                        {
                            if (find.MoHuCX != value)
                            {
                                find.MoHuCX = value;
                                ClientSetting.Save();
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(value))
                    {
                        ClientSetting.UserConfigList.Add(new UserConfig() { ZhiGongID = USERID, MoHuCX = value });
                        ClientSetting.Save();
                    }
                }
            }
        }

        /// <summary>
        /// 我的床位ID
        /// </summary>
        public static List<string> MYCHUANGWEIID = new List<string>();

        /// <summary>
        /// 消息内容
        /// </summary>
        public static List<HISMessageBody> XIAOXINR = new List<HISMessageBody>();

        public static Dictionary<string, IList<HISMessageBody>> dictryMessBody = new Dictionary<string, IList<HISMessageBody>>();

        /// <summary>
        /// 消息总数
        /// </summary>
        public static int XIAOXIZS = 0;

        public static int xiaoxiCount = 0;

        /// <summary>
        /// 医嘱变动信息人数
        /// </summary>
        public static DataTable yzBianDongXXRSTable = new DataTable();

        /// <summary>
        /// 弹框时间设置
        /// </summary>
        public static int timeDate = 3000;

        /// <summary>
        /// 重新登录显示未处理医嘱变动信息条数
        /// </summary>
        public static string initYzTS = "0";

        /// <summary>
        /// 消息更新数量集合
        /// </summary>
        public static List<HISMessageBody> messBody = new List<HISMessageBody>();

        /// <summary>
        /// 消息内容
        /// </summary>
        public static List<HISMessageBody> XIAOXINR_LS = new List<HISMessageBody>();

        /// <summary>
        /// 消息内容
        /// </summary>
        public static List<HISMessageBody> XIAOXIYDNR = new List<HISMessageBody>();
        /// <summary>
        /// 已读消息总数
        /// </summary>
        public static int XIAOXIYDZS = 0;
        
        public static bool FrmType = false;

        /// <summary>
        /// 消息右边控件展示
        /// </summary>
        public static Dictionary<string, string> XiaoXiRightNR { get; set; }

        public static List<object> JiBingList { get; set; }

        /// <summary>
        /// 全局配置文件帮助类（HISGlobalSetting.xml)
        /// </summary>
        public static HISGlobalSetting GlobalSetting { get; set; }

        /// <summary>
        /// 本地配置文件帮助类（HISClientSetting.xml)
        /// </summary>
        public static HISClientSetting ClientSetting { get; set; }

        public static bool DLCKSZZT { get; set; }
        
        /// <summary>
        /// 框架base窗体，用于调用W_KUANGJIA_BASE中的方法
        /// </summary>
        public static dynamic KuangJiaBase { get; set; }

        /// <summary>
        /// 用来判断打开的main.exe是否是切换用户时打开的
        /// </summary>
        public static bool IsSwitchUser { get; set; } = false;
        /// <summary>
        /// 切换用户时记录上一次登陆的应用
        /// </summary>
        public static string PreviewApp { get; set; }


        public static WeakReference ower { get; set; }
        /// <summary>
        /// 当前窗口名称
        /// </summary>
        public static string DANGQIANCKMC { get; set; }

        /// <summary>
        /// 医疗组ID
        /// </summary>
        public static string YILIAOZID { get; set; }

        /// <summary>
        /// 在线状态ID
        /// </summary>
        public static string ZAIXIANZTID { get; set; }

        public static int ClearMemoryCounter = 0; //added by jzy for HR6-1920(533696) 用于清理内存的计数器

        public delegate void MsgEventHandler(object sender, BasicDeliverEventArgs e);
        public static event MsgEventHandler MsgEvent;

        public static void OnMsgEvent(object sender, BasicDeliverEventArgs e)
        {
            MsgEvent?.Invoke(sender, e);
        }

        public static IEnumerable<MsgEventHandler> GetMsgEventHandlers()
        {
            if (MsgEvent != null)
            {
                return from d in MsgEvent.GetInvocationList()
                       select (MsgEventHandler)d;
            }
            else
            {
                return new List<MsgEventHandler>();
            }
        }

        /// <summary>
        /// 内部消息订阅
        /// </summary>
        public static EventingBasicConsumer MsgConsumer { get; set; }
        public static string JiuZhenKSMC { get; set; }

        /// <summary>
        /// 系统登陆时间
        /// </summary>
        private static DateTime serverTime;

        private static Stopwatch stopwatch;

        public static Dictionary<string, Socket> dictryScoketList = new Dictionary<string, Socket>();
        public static Dictionary<string, Thread> dictryThreadList = new Dictionary<string, Thread>();

        /// <summary>
        /// 初始化本地服务器时间缓存
        /// </summary>
        /// <param name="dt"></param>
        public static void SetSysDate(DateTime dt)
        {
            serverTime = dt;

            if (stopwatch == null)
                stopwatch = new Stopwatch();

            stopwatch.Restart();
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSysDate()
        {
            if (serverTime == DateTime.MinValue || stopwatch == null)
            {
                SetSysDate(DateTime.Now);
            }
            return serverTime.AddMilliseconds(stopwatch.ElapsedMilliseconds);
            //return DateTime.Now;
        }

        public static int GetMenZhenZYBZ(string yingYongID)
        {
            if (string.IsNullOrWhiteSpace(yingYongID))//默认为门诊
            {
                return 0;
            }
            switch (yingYongID.Substring(0, 2))
            {
                case "11":
                    return 2;
                case "02":
                case "04":
                case "23":
                    return 0;
                default:
                    return 1;
            }

        }

        /// <summary>
        /// 客户端时间
        /// </summary>
        public static DateTime clientDateTime;

        /// <summary>
        /// 批处理命令
        /// </summary>
        public static void BatRunCmd(string batName, string batPath, out string errorException)
        {
            errorException = String.Empty;
            try
            {
                if (!String.IsNullOrWhiteSpace(batName) && !batName.Equals("hisstart.bat"))
                {
                    string hisstartpath = batPath + batName;
                    
                    if (File.Exists(hisstartpath))
                    {
                        Process hisstartprocess = new Process();

                        // 应用ID，职工ID，职工工号，职工姓名，密码
                        string argsment = HISClientHelper.YINGYONGID + " " + HISClientHelper.USERID + " " + HISClientHelper.ZHIGONGGH + " " + HISClientHelper.USERNAME + " " + HISClientHelper.USERPWD;
                        hisstartprocess.StartInfo = new ProcessStartInfo(hisstartpath, argsment);
                        hisstartprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        hisstartprocess.Start();
                    }
                    else
                    {
                        File.WriteAllText(hisstartpath, "%第一个参数--应用ID|第二个参数--职工ID|第三个参数--职工工号|第四个参数--职工姓名|第五个参数--密码%", Encoding.Default);
                    }

                }
                else if (batName.Equals("hisstart.bat"))
                {
                    string hisstartpath = batPath + "hisstart.bat";
                    if (File.Exists(hisstartpath))
                    {
                        Process hisstartprocess = new Process();
                        hisstartprocess.StartInfo = new ProcessStartInfo(hisstartpath);
                        hisstartprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        hisstartprocess.Start();
                    }
                    else
                    {
                        File.WriteAllText(hisstartpath, "%程序启动批命令%", Encoding.Default);
                    }
                }
            }
            catch (Exception ex)
            {
                errorException = ex.Message;
            }
        }

        private static readonly string CmdPath = @"C:\Windows\System32\cmd.exe";

        /// <summary>
        /// 执行cmd命令
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// 其他请百度
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        public static void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                p.Start();  // 启动程序

                // 向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                // 获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();    // 等待程序执行完退出进程
                p.Close();
            }
        }

        #region 诊间通用常量

        // 皮试标志
        public const int GY_PiShiBZ_NO = 0;     // 非皮试
        public const int GY_PiShiBZ_PuTong = 1; // 普通
        public const int GY_PiShiBZ_YuanYE = 2; // 原液

        // 皮试结果
        public const string GY_PiShiCLYJ_TongGuo = "0";     // 通过
        public const string GY_PiShiCLYJ_WeiTongGuo = "1";  // 未通过
        public const string GY_PiShiCLYJ_MianShi = "2";     // 免试

        // 毒理分类
        public const string GY_DuLiFL_PuTong = "0";     // 普通
        public const string GY_DuLiFL_DuXing = "1";     // 毒性
        public const string GY_DuLiFL_MaZui = "2";      // 麻醉
        public const string GY_DuLiFL_JingShen1 = "3";  // 精神1
        public const string GY_DuLiFL_JingShen2 = "5";  // 精神2

        public const string GY_YiCiJL = "遵医嘱";      // 在诊间yijicl无法输入的情况下，再嘱托中输入
        public const string GY_YiCiJL_SL = "适量";     // 诊间录入大小规格药品按给药方式控制打印剂量

        /// <summary>
        /// 西药
        /// </summary>
        public const string ChuFangLX_Xi = "100";

        /// <summary>
        /// 成药
        /// </summary>
        public const string ChuFangLX_Cheng = "010";

        /// <summary>
        /// 草药
        /// </summary>
        public const string ChuFangLX_Cao = "001";

        /// <summary>
        /// 西成药
        /// </summary>
        public const string ChuFangLX_XiCheng = "110";

        #endregion
    }
}
