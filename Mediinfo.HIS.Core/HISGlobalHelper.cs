using System;
using System.Collections.Generic;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public partial class HISGlobalHelper
    {
        static HISGlobalHelper()
        {
            GlobalSetting = HISGlobalSetting.Load();
            HttpConfigs = HISGlobalSetting.LoadHttpInfos();
        }

        public static HISGlobalSetting GlobalSetting { get; set; }
        public static List<HTTPUpdateConfig> CheckHttpConfig(List<HTTPUpdateConfig> hTTPUpdateConfigs)
        {
            //检查读到得HTTP配置文件
            List<HTTPUpdateConfig> result = new List<HTTPUpdateConfig>();
            if (hTTPUpdateConfigs.Count != 0)
            {
                foreach (HTTPUpdateConfig item in hTTPUpdateConfigs)
                {
                    if (item.JIXIANMC == null || item.BanBenHao == null) continue;
                    result.Add(item);
                }
            }
            return result;
        }
        /// <summary>
        /// http配置文件
        /// </summary>
        public static List<HTTPUpdateConfig> HttpConfigs { get; set; }
        /// <summary>
        /// 病区类型
        /// </summary>
        public static class BingQuLX
        {
            public const string PuTong = "0";
            public const string ICU = "1";
            public const string CCU = "2";
            public const string LGS = "3";
            public const string QJS = "4";
            public const string XTBQ = "5";

            public static Dictionary<string, string> DataSource { get; set; }

            static BingQuLX()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(PuTong, "普通病区");
                DataSource.Add(ICU, "ICU病区");
                DataSource.Add(CCU, "CCU病区");
                DataSource.Add(LGS, "留观室");
                DataSource.Add(QJS, "抢救室");
                DataSource.Add(XTBQ, "血透病区");
            }
        }

        /// <summary>
        /// 产地类型
        /// </summary>
        public static class ChanDiLB
        {
            public const string GuoChan = "1";
            public const string HeZi = "2";
            public const string JinKou = "3";
            public const string ZiZhi = "4";
            public const string QiTa = "9";

            public static Dictionary<string, string> DataSource { get; set; }

            static ChanDiLB()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(GuoChan, "国产");
                DataSource.Add(HeZi, "合资");
                DataSource.Add(JinKou, "进口");
                DataSource.Add(ZiZhi, "自制");
                DataSource.Add(QiTa, "其它");
            }
        }

        /// <summary>
        /// 代码类别
        /// </summary>
        public static class DaiMaLB
        {
            // 省份
            public static string ShengFen = "0006";
            // 性别
            public static string XinBie = "0008";
            // 关系
            public static string GuanXi = "0009";
            // 职务
            public static string ZhiWu = "0010";
            // 职称
            public static string ZhiCheng = "0011";
            // 地区
            public static string DiQu = "0012";
            // 帐簿类别
            public static string ZhangBuLB = "0023";
            // 门诊类别
            public static string MenZhenLB = "0025";
            // 皮试结果
            public static string PiShiJG = "0028";
            // 皮试处理意见
            public static string PiShiCLYJ = "0083";
            // 挂号预约类型
            public static string YuYueLX = "0154";
            // 支付方式
            public static string ZhiFuFS = "0014";
            // 票据类型
            public static string PiaoJuLX = "0057";
            // 介质类型
            public static string JieZhiLX = "0063";
            // 药品单位
            public static string YaoPinDW = "0065";
            // 计价单位
            public static string JiJiaDW = "0068";
            // 职工权限
            public static string ZhiGongQX = "0073";
            // 职工类别
            public static string ZhiGongLB = "0074";
            // 给药方式类型
            public static string GeiYaoFSLX = "0085";
            // 输血成分
            public static string ShuXueCF = "0122";
            // 担保类型
            public static string DanBaoLX = "0123";
            // 公用汇率类别
            public static string HuiLvLB = "0141";
            // 药品其他属性
            public static string QiTaSX = "0182";
            // 医生等级
            public static string YiShengDJ = "0206";
            // 职工在职状态
            public static string ZhiGongZZZT = "0227";
            // 职工执业资格
            public static string ZhiGongZYZG = "0228";
            // 职工编制类别
            public static string ZhiGongBZLB = "0229";
            // 医疗组内角色
            public static string ZuNeiJS = "0230";
            // 学历
            public static string XueLi = "0257";
            // 是否入院检查
            public static string ShiFouRYJC = "0521";
        }

        /// <summary>
        /// 药品类型
        /// </summary>
        public static class YaoPinLX
        {
            public const string QuanBu = "-1";
            public const string Xi = "1";
            public const string Cheng = "2";
            public const string Cao = "3";
            public const string Wei = "4";
            public const string Zhi = "5";
            public const string Shi = "6";
            public const string Xie = "8";

            public static Dictionary<string, string> DataSource { get; set; }

            static YaoPinLX()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(QuanBu, "全部");
                DataSource.Add(Xi, "西药");
                DataSource.Add(Cheng, "成药");
                DataSource.Add(Cao, "草药");
                DataSource.Add(Wei, "卫材");
                DataSource.Add(Zhi, "制剂");
                DataSource.Add(Shi, "试剂");
                DataSource.Add(Xie, "协定");
            }
        }

        /// <summary>
        /// 医嘱输入类型
        /// </summary>
        public static class YiZhuShuRuLX
        {
            /// <summary>
            /// 文本医嘱
            /// </summary>
            public const string WenBen = "-";

            /// <summary>
            /// 诊疗医嘱
            /// </summary>
            public const string ZhenLiao = "/";

            /// <summary>
            /// 药品医嘱
            /// </summary>
            public const string YaoPin = "&";
        }

        public static class GYKeShiYP
        {
            public const int WeiSheZhi = 0;     // 未设置
            public const int ZhunRuFa = 1;      // 准入法
            public const int PaiChuFa = -1;     // 排除法
        }

        public static class GYKuFangSY
        {
            public const int YaoKu = 1;         // 药库使用
            public const int MenYao = 2;        // 门药使用
            public const int BingYao = 3;       // 病药使用
            public const int MenYao_ZY = 4;     // 门药使用_住院
            public const int BingYao_MZ = 5;    // 病药使用_门诊
            public const int JingMaiP = 6;      // 静脉配
            public const int ZhiJi = 7;         // 制剂(全部)
            public const int ZhiJiCP = 8;       // 制剂(成品)
            public const int ZhiJiFCP = 9;      // 制剂(非成品)
            public const int ZhongYaoKLSY = 10;	            // 中药散配颗粒
            public const int MenYao_BingYao = 11;           // 根据应用ID的前两位，自动取门药使用或病药使用
            public const int ZhongYaoKLZBSY = 12;	        // 中药颗粒整包使用  HR3-13306(171599)
            public const int ZhongYaoKLSY_MenYao_ZY = 13;   // 住院用的散配颗粒
            public const int ZhongYaoKLZBSY_MenYao_ZY = 14; // 住院用的整包颗粒
            public const int YinPianXBZ = 15;       // 门诊饮片小包装 
            public const int YinPianXBZ_ZY = 16;    // 住院饮片小包装 
        }

        public static class KFChuRuKFS
        {
            public const string QingLing = "04";        // 请领
            public const string ZhiLiangYS = "02";      // 质量验收
            public const string CaiGouJH = "03";        // 采购计划
            public const string YaoKuBR = "01";         // 药库拨入
            public const string TiaoJia = "21";         // 调价
            public const string PanCun = "22";          // 盘存
            public const string BaoSun = "23";          // 报损
            public const string WeiChaTZ = "81";        // 尾差调整
            public const string ShangYueJC = "82";      // 上月结存
            public const string BenYueJC = "83";        // 本月结存
            public const string YingFuKuan = "51";      // 应付款
            public const string MenZhenFY = "61";       // 门诊发药
            public const string MenZhenTY = "59";       // 门诊退药
            public const string GuiGeZHRK = "98";       // 规格转换入库
            public const string GuiGeZHCK = "99";       // 规格转换出库
            public const string YiChangCF = "60";       // 异常处方
            public const string BingQuLY = "62";        // 病区领药
            public const string BingQuFY = "67";        // 病区发药
            public const string BingQuTY = "68";        // 病区退药
            public const string ShouGongYZFY = "71";    // 手工医嘱发药
            public const string ShouGongYZTY = "72";    // 手工医嘱退药
            public const string TuiHuoCEFS = "08";      // 退货差额方式
            public const string JieYao = "31";          // 借药
            public const string HuanYao = "32";         // 还药

            public const string ShengChanRK = "96";     // 生产入库
            public const string ShengChanCK = "97";     // 生产出库
        }

        public static string YK_GuanLiZBLB { get; set; }

        public static int? ZhiJiYFBZ { get; set; }      // 制剂药房标志

        /// <summary>
        /// 药库房判断
        /// </summary>
        public static class YaoKuFPD
        {
            public const string XiTongID_YaoKu = "06";          // 代表药库 
            public const string XiTongID_MenZhenYF = "05";	    // 代表门诊药房
            public const string XiTongID_BingQuYF = "13";	    // 代表病区药房
            public const string XiTongID_MenZhenZJ = "04";      // 代表门诊诊间 
            public const string XiTongID_JingMaiPYF = "33";     // 代表静脉配药房
        }

        /// <summary>
        /// 星期
        /// </summary>
        public static class XingQi
        {
            public const int Monday = 2;
            public const int Tuesday = 3;
            public const int Wednesday = 4;
            public const int Thursday = 5;
            public const int Friday = 6;
            public const int Saturday = 7;
            public const int Sunday = 1;
            public const int AllDay = 0;

            public static Dictionary<int, string> DataSource { get; set; }

            static XingQi()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(Monday, "星期一");
                DataSource.Add(Tuesday, "星期二");
                DataSource.Add(Wednesday, "星期三");
                DataSource.Add(Thursday, "星期四");
                DataSource.Add(Friday, "星期五");
                DataSource.Add(Saturday, "星期六");
                DataSource.Add(Sunday, "星期日");
                DataSource.Add(AllDay, "整周");
            }
        }

        /// <summary>
        /// 取当前日期周
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static int GetWeek(DateTime datetime)
        {
            string dt = datetime.DayOfWeek.ToString();
            int week = 1;
            switch (dt)
            {
                case "Monday":
                    week = XingQi.Monday;
                    break;
                case "Tuesday":
                    week = XingQi.Tuesday;
                    break;
                case "Wednesday":
                    week = XingQi.Wednesday;
                    break;
                case "Thursday":
                    week = XingQi.Thursday;
                    break;
                case "Friday":
                    week = XingQi.Friday;
                    break;
                case "Saturday":
                    week = XingQi.Saturday;
                    break;
                case "Sunday":
                    week = XingQi.Sunday;
                    break;
            }
            return week;
        }

        ///// <summary>
        ///// 消息右边控件集合
        ///// </summary>
        //public static class XiaoXiRightNR
        //{
        //    public static Dictionary<string, string> XiaoXiNR { get; set; }
        //    static XiaoXiRightNR()
        //    {
        //        XiaoXiNR = new Dictionary<string, string>();
        //        XiaoXiNR.Add("0004", "XiaoXiRightMRNR");
        //    }
        //}

        /// <summary>
        /// 预交款交款方式
        /// </summary>
        public static class JiaoKuanLX
        {
            public const string BuJiao = "1";
            public const string MenZhenFPBX = "2";
            public const string ZhuYuanFPBX = "3";
            public const string ChuYuanZC = "4";
            public const string MenZhenYJK = "5";
        }
        
        /// <summary>
        /// 处方类型
        /// </summary>
        public static class ChuFangLX
        {
            public const string ChuFangLX_XiYao = "1";      // 西药
            public const string ChuFangLX_ChengYao = "2";   // 成药
            public const string ChuFangLX_CaoYao = "3";     // 草药
            public const string ChuFangLX_ShouFeiXM = "4";  // 收费项目

            public static Dictionary<string, string> DataSource { get; set; }

            static ChuFangLX()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(ChuFangLX_XiYao, "西");
                DataSource.Add(ChuFangLX_ChengYao, "成");
                DataSource.Add(ChuFangLX_CaoYao, "草");
                DataSource.Add(ChuFangLX_ShouFeiXM, "其他");
            }
        }

        private static List<string> YaoPinXMLXList = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        /// <summary>
        /// 根据项目类型判断项目是不是诊疗项目
        /// </summary>
        /// <param name="xiangMuLX">项目类型</param>
        /// <returns></returns>
        public static bool IsZhenLiao(string xiangMuLX)
        {
            return !YaoPinXMLXList.Contains(xiangMuLX);
        }

        /// <summary>
        /// 根据项目类型判断项目是不是药品
        /// </summary>
        /// <param name="xiangMuLX"></param>
        /// <returns></returns>
        public static bool IsYaoPin(string xiangMuLX)
        {
            return YaoPinXMLXList.Contains(xiangMuLX);
        }

        /// <summary>
        /// 介质类型
        /// </summary>
        public static class JieZhiLX
        {
            public const string Default = "0";      // 病人ID
            public const string BingLiBen = "1";    // 病历本
            public const string JiuZhenKa = "2";    // 就诊卡
            public const string YiBaoKa = "3";      // 医保卡
            public const string CiKa = "4";         // 磁卡
            public const string TiaoMa = "5";       // 条码
            public const string GongFeiZH = "6";    // 公费证号 
        }

        /// <summary>
        /// //就诊卡号生成方式
        /// </summary>
        public static class JIUZHENKHSCFS
        {
            public const int READ = 0;              // 读卡取得
            public const int AUTO = 1;              // 自动生成
            public const int USER = 2;              // 手工输入
            public const int BINGRENID = 3;         // 病人ID
        }

        /// <summary>
        /// 月份
        /// </summary>
        public static class YueFen
        {
            public const int January = 1;
            public const int February = 2;
            public const int March = 3;
            public const int April = 4;
            public const int May = 5;
            public const int June = 6;
            public const int July = 7;
            public const int August = 8;
            public const int September = 9;
            public const int October = 10;
            public const int November = 11;
            public const int December = 12;

            public static Dictionary<int, string> DataSource { get; set; }

            static YueFen()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(January, "一月");
                DataSource.Add(February, "二月");
                DataSource.Add(March, "三月");
                DataSource.Add(April, "四月");
                DataSource.Add(May, "五月");
                DataSource.Add(June, "六月");
                DataSource.Add(July, "七月");
                DataSource.Add(August, "八月");
                DataSource.Add(September, "九月");
                DataSource.Add(October, "十月");
                DataSource.Add(November, "十一月");
                DataSource.Add(December, "十二月");
            }
        }

        public static class ZhiFuFS
        {
            public const string ZhiFuFS_XianJin = "1";          // 现金
            public const string ZhiFuFS_ZhiPiao = "2";          // 支票
            public const string ZhiFuFS_HuiPiao = "3";          // 汇票

            public const string ZhiFuFS_YinHangKa = "10";       // 银行卡
            public const string ZhiFuFS_DianZiQB = "6";         // 电子钱包_也就是院内帐户
            public const string ZhiFuFS_DianZiZH = "7";         // 电子帐户
            public const string ZhiFuFS_ZanQianKuan = "11";     // 暂欠款

            public const string ZhiFuFS_YuJiaoKuan = "9";       // 预交款支付,用于结算时冲抵所有预交款
        }

        /// <summary>
        /// 介质号码生成方式 
        /// </summary>
        public static class JieZhiHMSCFS
        {
            public const string JieZhiHMSCFS_Read = "0";        // 读卡取得
            public const string JieZhiHMSCFS_Auto = "1";        // 自动生成
            public const string JieZhiHMSCFS_User = "2";        // 手工输入                
            public const string JieZhiHMSCFS_BingRenID = "3";   // 跟病人ID 
        }

        public static class GuaHaoLB
        {
            public const string GuaHaoLB_PuTong = "1";      // 普通
            public const string GuaHaoLB_JiZhen = "2";      // 急诊               
            public const string GuaHaoLB_ZhuanKe = "3";     // 专科 
        }

        /// <summary>
        /// 挂号类别归类
        /// </summary>
        public static class GuaHaoLBGL
        {
            public const string GuaHaoLBGL_PuTong = "1";        // 普通
            public const string GuaHaoLBGL_JiZhen = "2";        // 急诊               
            public const string GuaHaoLBGL_ZhuanKe = "3";       // 专科 
            public const string GuaHaoLBGL_ZhuanJia = "4";      // 专家

            public static Dictionary<string, string> DataSource { get; set; }

            static GuaHaoLBGL()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(GuaHaoLBGL_PuTong, "普通");
                DataSource.Add(GuaHaoLBGL_JiZhen, "急诊");
                DataSource.Add(GuaHaoLBGL_ZhuanKe, "专科");
                DataSource.Add(GuaHaoLBGL_ZhuanJia, "专家");
            }
        }

        /// <summary>
        /// 门诊坐诊类型
        /// </summary>
        public static class MenZhenZZLX
        {
            public const string ZuoZhenLX_Putong = "1";         // 普通
            public const string ZuoZhenLX_ZhuanJia = "4";       // 专家
        }

        /// <summary>
        /// 就诊状态（诊间）
        /// </summary>
        public static class JiuZhenZT
        {
            public const int DaiZhenZhong = 0;      // 待诊中                                                 
            public const int JiuZhenZhong = 1;      // 就诊中                                                
            public const int YiJiuZhen = 2;         // 已就诊

            public static Dictionary<int, string> DataSource { get; set; }

            static JiuZhenZT()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(DaiZhenZhong, "待诊中");
                DataSource.Add(JiuZhenZhong, "就诊中");
                DataSource.Add(YiJiuZhen, "已就诊");
            }
        }

        /// <summary>
        /// 上下午
        /// </summary>
        public static class ShangXiaWu
        {
            public const int ShangWu = 0;       // 上午                                            
            public const int XiaWu = 1;         // 下午                                                
            public const int WanShang = 2;      // 晚上

            public static Dictionary<int, string> DataSource { get; set; }

            static ShangXiaWu()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(ShangWu, "上午");
                DataSource.Add(XiaWu, "下午");
                DataSource.Add(WanShang, "晚上");
            }
        }

        /// <summary>
        /// 会诊状态
        /// </summary>
        public static class HuiZhenZT
        {
            public const int XinKaiDan = 0;     // 新开单
            public const int ShenHeTG = 1;      // 审核通过
            public const int ShenHeBTG = 2;     // 审核不通过
            public const int JieShou = 3;       // 接收
            public const int WanCheng = 4;      // 完成
            public const int DaYin = 5;         // 打印
            public const int ZuoFei = 9;        // 作废
            public const int JuJue = 10;        // 拒绝

            public static Dictionary<int, string> DataSource { get; set; }

            static HuiZhenZT()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(XinKaiDan, "新开单");
                DataSource.Add(ShenHeTG, "审核通过");
                DataSource.Add(ShenHeBTG, "审核不通过");
                DataSource.Add(JieShou, "接收");
                DataSource.Add(WanCheng, "完成");
                DataSource.Add(DaYin, "打印");
                DataSource.Add(ZuoFei, "作废");
                DataSource.Add(JuJue, "拒绝");
            }
        }

        /// <summary>
        /// 会诊类型
        /// </summary>
        public static class HuiZhenLX
        {
            public const int PuTongHZ = 0;  // 普通会诊单
            public const int JiZhenHZ = 1;  // 急诊会诊单

            public static Dictionary<int, string> DataSource { get; set; }

            static HuiZhenLX()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(PuTongHZ, "普通会诊单");
                DataSource.Add(JiZhenHZ, "急诊会诊单");
            }
        }

        /// <summary>
        /// 会诊类别
        /// </summary>
        public static class HuiZhenLB
        {
            public const int QuanBuHZ = 0;      // 全部会诊单

            public static Dictionary<int, string> DataSource { get; set; }

            static HuiZhenLB()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(QuanBuHZ, "全部");
            }
        }

        /// <summary>
        /// 医护标志（1护士2医生3诊间）
        /// </summary>
        public static class YiHuBZ
        {
            public const int HuShi = 1;     // 护士
            public const int YiSheng = 2;   // 医生
            public const int ZhenJian = 3;  // 诊间
        }

        /// <summary>
        /// 规定病
        /// </summary>
        public static class GuiDingBing
        {
            public const int PuTong = 0;        // 普通
            public const int GuiDingBZ = 1;     // 规定病种
            public const int BaoJianGB = 2;     // 保健干部
            public const int ShengYuBX = 5;     // 生育保险

            public static Dictionary<int, string> DataSource { get; set; }

            static GuiDingBing()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(PuTong, "普通");
                DataSource.Add(GuiDingBZ, "规定病种");
                DataSource.Add(BaoJianGB, "保健干部");
                //DataSource.Add(4, "规定病种保健干部");
                DataSource.Add(ShengYuBX, "生育保险");
            }
        }

        /// <summary>
        /// 病人列表种预约标志
        /// </summary>
        public static class YuYueBZ
        {
            public const int Kong = 0;      // 空
            public const int Yue = 1;       // 预
            public const int Dai = 2;       // 代
            public const int Zhuan = 3;     // 转
            public const int Tui = 4;       // 退

            public static Dictionary<int, string> DataSource { get; set; }

            static YuYueBZ()
            {
                DataSource = new Dictionary<int, string>();
                DataSource.Add(Kong, " ");
                DataSource.Add(Yue, "预");
                DataSource.Add(Dai, "代");
                DataSource.Add(3, "转");
                DataSource.Add(4, "退");
            }
        }

        /// <summary>
        /// 就诊信息的记录来源
        /// </summary>
        public static class JiuZhenXX_JiLuLY
        {
            public const string GuaHao = "0";       // 挂号
            public const string ZiZhuGH = "1";      // 自助挂号
            public const string MenZhenZJ = "2";    // 门诊诊间
            public const string ZhuanZhen = "3";    // 转诊
            public const string ZhenJianFZYY = "6"; // 诊间复诊预约
            public const string BuQuHui = "7";      // 诊间就产生就诊信息，对应之前的挂号记录（杭州市二的不取回）

            public static Dictionary<string, string> DataSource { get; set; }

            static JiuZhenXX_JiLuLY()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(GuaHao, "挂号");
                DataSource.Add(ZiZhuGH, "自助挂号");
                DataSource.Add(MenZhenZJ, "门诊诊间");
                DataSource.Add(ZhuanZhen, "转诊");
                DataSource.Add(ZhenJianFZYY, "诊间复诊预约");
                DataSource.Add(BuQuHui, "不取回");
            }
        }

        /// <summary>
        /// 担保范围
        /// </summary>
        public static class DanBaoFW
        {
            public const int LvSeTD = 5;
        }

        /// <summary>
        /// 路径一级菜单内容
        /// </summary>
        public static string[] jieDuan = new string[] { "文书", "医嘱", "处置" };

        /// <summary>
        /// 路径二级菜单内容
        /// </summary>
        public static string[] yiZhuJieDuan = new string[] { "诊疗医嘱", "草药医嘱", "药品医嘱", "检验医嘱", "检查医嘱", "文本医嘱", "手术医嘱" };

        public static class PaiBanMS
        {
            public const string YiZhouPB = "1";     // 一周排班
            public const string DangRiPB = "2";     // 单日排班           
            public const string TeShuPB = "3";      // 特殊排班
        }

        #region "医嘱常量" added by Hujian 20190419

        public const string 注射剂剂型 = "25";

        /// <summary>
        /// 医嘱费用项目来源 1、药品医嘱 2、诊疗医嘱 3、给药方式 4、医嘱核对
        /// </summary>
        public static class XiangMuLY
        {
            public const string YaoPin = "1";
            public const string ZhenLiao = "2";
            public const string GeiYaoFS = "3";
            public const string HeDui = "4";
        }

        /// <summary>
        /// /1.诊疗 2.西药 3.成药 4.草药 7.大输液 8、文本医嘱
        /// </summary>
        public static class YiZhuFL
        {
            /// <summary>
            /// 诊疗
            /// </summary>
            public const string ZhenLiao = "1";
            /// <summary>
            /// 西药
            /// </summary>
            public const string XiYao = "2";
            /// <summary>
            /// 成药
            /// </summary>
            public const string ChengYao = "3";
            /// <summary>
            /// 草药
            /// </summary>
            public const string CaoYao = "4";
            /// <summary>
            /// 大输液
            /// </summary>
            public const string DaShuYe = "7";
            /// <summary>
            /// 文本医嘱
            /// </summary>
            public const string WenBeng = "8";
            /// <summary>
            /// 检验
            /// </summary>
            public const string JianYan = "10";
            /// <summary>
            ///检查
            /// </summary>
            public const string JianCha = "9";
            /// <summary>
            /// 输血
            /// </summary>
            public const string ShuXue = "11";
            /// <summary>
            /// 手术
            /// </summary>
            public const string ShouShu = "12";
            /// <summary>
            /// 会诊
            /// </summary>
            public const string HuiZhen = "13";
        }

        /// <summary>
        /// 给药方式类型 1. 口服 2. 输液 3. 出院带药 4. 领药 5. 退药 6. 其他 7.辅药  8.注射 9.外用 12.煎药 13.静脉注射 14.肌注 15.皮试 16.TPN 17.微泵
        /// </summary>
        public static class GeiYaoFSLX
        {
            public const int KouFu = 1;
            public const int ShuYe = 2;
            public const int ChuYuanDaiYao = 3;
            public const int LingYao = 4;
            public const int TuiYao = 5;
            public const int QiTa = 6;
            public const int FuYao = 7;
            public const int ZhuShe = 8;
            public const int WaiYong = 9;
            public const int JianYao = 12;
            public const int JingMaiZS = 13;
            public const int JiZhu = 14;
            public const int PiShi = 15; //HR3-13631(174615)增加了皮试的给药方式类型
            public const int TPN = 16;
            public const int WeiBang = 17; //HR3-19743(225816) 增加了微泵的给药方式类型 
        }

        /// <summary>
        /// 医嘱状态0.未提交 1. 未确认 2. 已确认 3. 已执行 4. 已停止 5. 撤消
        /// </summary>
        public static class YiZhuZT
        {
            /// <summary>
            /// 未提交
            /// </summary>
            public const string WTJ = "0";
            /// <summary>
            /// 未确认
            /// </summary>
            public const string WQR = "1";
            /// <summary>
            /// 已确认
            /// </summary>
            public const string YQR = "2";
            /// <summary>
            /// 已执行
            /// </summary>
            public const string YZX = "3";
            /// <summary>
            /// 已停止
            /// </summary>
            public const string YTZ = "4";
            /// <summary>
            /// 撤销
            /// </summary>
            public const string CX = "5";
        }

        /// <summary>
        /// 排斥类型 0、不排斥 1、单组排斥 2、全排斥
        /// </summary>
        public static class PaiChiLX
        {
            public const int BPC = 0;
            public const int DZPC = 1;
            public const int QPC = 2;
        }

        /// <summary>
        ///医嘱当前状态
        ///0.未提交 1. 未确认 2. 已确认 3.新更动 4.未执行 5.已执行
        ///6. 不执行 7.已停止  8.待撤销  9.已撤销 10.待退药
        ///11.已退药 12.已发药  13. 缺药
        /// </summary>
        public static class DangQianZT
        {
            public const string WTJ = "0";
            public const string WQR = "1";
            public const string YQR = "2";
            public const string XGD = "3";
            public const string WZX = "4";
            public const string YZX = "5";
            public const string BZX = "6";
            public const string YTZ = "7";
            public const string DCX = "8";
            public const string YCX = "9";
            public const string DTY = "10";
            public const string YTY = "11";
            public const string YFY = "12";
            public const string QY = "13";
        }

        /// <summary>
        /// 发药状态
        ///0.待发药 1.已发药 2.缺药 3.待退药 4.已退药
        /// </summary>
        public static class FaYaoZT
        {
            public const string DFY = "0";
            public const string YFY = "1";
            public const string QY = "2";
            public const string DTY = "3";
            public const string YTY = "4";
        }

        /// <summary>
        /// 交易处理
        /// </summary>
        public static class JiaoYiCL
        {
            public const string YiZhuKL = "医嘱开立";
            public const string YiZhuFH = "医嘱复核";
            public const string YiZhuCX = "医嘱撤销";
            public const string YiZhuZX = "医嘱执行";
        }

        public static class YiZhuKLLX
        {
            public const int YaoPin = 1;
            public const int ZhenLiao = 2;
            public const int JianCha = 3;
            public const int JianYan = 4;
            public const int WenBen = 5;
            public const int ShuXue = 6;
            public const int ChangQi = 7;
            public const int LinSHi = 8;
            public const int Huizhen = 9;
            public const int ShouShu = 10;
        }

        /// <summary>
        /// 饮片/散配颗粒/整包颗粒
        /// </summary>
        public static class CaoYaoFXZ
        {
            /// <summary>
            /// 饮片
            /// </summary>
            public const string YINPIAN = "YINPIAN";
            public const int YINPIAN_Value = 0;
            public const string YINPIAN_Name = "饮片";
            /// <summary>
            /// 小包装饮片
            /// </summary>
            public const string YINPIANXBZ = "YINPIANXBZ";
            public const int YINPIANXBZ_Value = 3;
            public const string YINPIANXBZ_Name = "小包装饮片";
            /// <summary>
            /// 散配颗粒剂
            /// </summary>
            public const string KELI = "KELI";
            public const int KELI_Value = 1;
            public const string KELI_Name = "散配颗粒剂";
            /// <summary>
            /// 整包颗粒剂
            /// </summary>
            public const string KELIZB = "KELIZB";
            public const int KELIZB_Value = 2;
            public const string KELIZB_Name = "整包颗粒剂";
        }

        //限定的药品类型，对于药库和药房管理来说就是库存管理类型
        public const string KUCUNGLLX_CAOYAO = "0010";

        #endregion

        #region  对应类型，项目类型，门诊住院标志

        /// <summary>
        /// 对应类型
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetDuiYingLX()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("1", "门诊收费-药房(手工处方)");
            dic.Add("2", "门诊诊间-药房");
            dic.Add("3", "病区护士站-药房");
            dic.Add("4", "病区医生站-药房");
            dic.Add("5", "药房-窗口");
            dic.Add("6", "住院收费-药房");
            dic.Add("7", "手麻-药库");
            dic.Add("8", "手麻-住院药房");
            dic.Add("9", "手麻-门诊药房");
            dic.Add("10", "病区护士站-静脉配");
            dic.Add("11", "住院医生站-静脉配");
            dic.Add("12", "病区护士站-化疗");
            dic.Add("13", "住院医生站-化疗");
            dic.Add("14", "病区护士站-三升袋");
            dic.Add("15", "住院医生站-三升袋");
            dic.Add("16", "药房-药房(代发医嘱用药)");
            dic.Add("17", "对应介入手术应用");
            dic.Add("18", "对应普通手术应用");
            dic.Add("19", "皮试系统对应");
            dic.Add("20", "分诊台对应");
            return dic;
        }

        /// <summary>
        /// 项目类型
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetXiangMuLX()
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            Dic.Add("1", "西药");
            Dic.Add("2", "成药");
            Dic.Add("3", "草药");
            Dic.Add("4", "卫材");
            Dic.Add("5", "制剂");
            Dic.Add("6", "试剂");
            Dic.Add("7", "资产");
            Dic.Add("8", "低耗");
            Dic.Add("9", "材料");
            Dic.Add("21", "检验");
            Dic.Add("22", "检查");
            Dic.Add("23", "处置");
            Dic.Add("24", "护理");
            Dic.Add("25", "手术");
            Dic.Add("26", "治疗");
            Dic.Add("27", "麻醉");
            Dic.Add("28", "饮食");
            Dic.Add("29", "输血");
            Dic.Add("30", "输氧");
            Dic.Add("31", "卫材(收费)");
            Dic.Add("40", "其它");
            Dic.Add("51", "套餐");
            Dic.Add("52", "配方");
            Dic.Add("81", "核算");
            Dic.Add("82", "收费");
            return Dic;
        }

        /// <summary>
        /// 门诊住院标志
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetMenZhenZhuYuanBZ()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("0", "门诊");
            dic.Add("1", "住院");
            dic.Add("3", "急诊");
            dic.Add("4", "静脉配");
            return dic;
        }

        #endregion

        /*
           费用冻结控制点
           1.开立长期医嘱 2.开立临时医嘱 3.执行长期医嘱
           4.执行临时医嘱 5.医嘱复核费用录入 6.开立检查单
           7.开立检验单 8.手术费用录入 9.手术费用录入_暂存 10 手术申请 12 医技系统费用录入
         */
        public static class FeiYongDJKZD
        {
            public const string KaiLiCQYZ = "1";
            public const string KaiLiLSYZ = "2";
            public const string ZhiXingCQYZ = "3";
            public const string ZhiXingLSYZ = "4";
            public const string YiZhuFHFYLR = "5";
            public const string JianChaKD = "6";
            public const string JianYanKD = "7";
            public const string ShouShuFYLR = "8";
            public const string ShouShuFYLR_ZC = "9";
            public const string ShouShuFYLR_SSSQ = "10";
            public const string HuShiZhanFYLR = "11";
            public const string YiJiFYLR = "12";
            public const string XiangMuFYLR = "13";
        }

        /// <summary>
        /// 费用控制对象
        /// </summary>
        public static class FeiYongKZDX
        {
            public const string DongJieKZDX_QuanBu = "0";       // 全部
            public const string DongJieKZDX_YaoPin = "1";       // 药品
            public const string DongJieKZDX_ZhenLiao = "2";     // 诊疗
            public const string DongJieKZDX_FeiKouFu = "3";     // 非口服药品
            public const string DongJieKZDX_JianCha = "4";      // HR3-25480(273090)：检查
            public const string DongJieKZDX_JianYan = "5";      // HR3-25480(273090)：检验
            public const string DongJieKZDX_ShuXue = "6";       // HR3-25480(273090)：输血
        }

        //add by chen chao for HR6-45(405873)
        public static class YuJieSuanFS
        {
            public const int YuJieSuanFS_ChaXun = 1;        // 查询方式 
            public const int YuJieSuanFS_XiangMuFYHZ = 2;   // 项目费用汇总  1和2的结算方式都处理成为不和医保交易的方式，没有区别 BY重构
            public const int YuJieSuanFS_All = 3;           // 全结算 
            public const int YuJieSuanFS_ChuYuanYJS = 4;    // 出院前预结算  3和4的结算方式都处理成和医保交易的方式，区别出院前结算和弹出交易框
        }

        public static class QuXiaoJSFS
        {
            public const int QuXiaoJSFS_PuTong = 0;     // 正常取消结算方式 
            public const int QuXiaoJSFS_BenDIJY = 1;    // 取消结算本地交易
        }

        /// <summary>
        /// 票据类型
        /// </summary>
        public static class PiaoJuLX
        {
            public const String GY_PiaoJuLX_GuaHao = "1";       // 挂号票据
            public const String GY_PiaoJuLX_MenZhenSF = "2";    // 门诊收费票据
            public const String GY_PiaoJuLX_ZhuYuanSF = "3";    // 住院收费票据
            public const String GY_PiaoJuLX_MenZhenYJK = "4";   // 门诊预交款票据
            public const String GY_PiaoJuLX_ZhuYuanYJK = "5";   // 住院预交款票据
        }

        public static class LingYaoFS
        {
            public const string PuTong = "0";       // 普通
            public const string JiShuYao = "1";     // 基数药
            public const string ZiBeiYao = "2";     // 自备药
            public const string ZhuTuo = "3";       // 嘱托
            public const string DaGuiGeLY = "4";    // 大规格领药
            public const string JiFei = "5";        // 计费
            public const string DanRiLJQZ = "6";    // 单日累计取整
            public const string DanYiZhuLJ = "7";   // 单医嘱累计
            public const string LinShiLY = "8";     // 临时领药
        }

        /// <summary>
        /// 医嘱项目类型
        /// </summary>
        public static class YiZhuXMLX
        {
            /// <summary>
            /// 饮食
            /// </summary>
            public const string YinShi = "5";
            /// <summary>
            /// 转科
            /// </summary>
            public const string ZhuanKe = "10";
            /// <summary>
            /// 转床
            /// </summary>
            public const string ZhuanChuang = "18";
            /// <summary>
            /// 管道
            /// </summary>
            public const string GuanDao = "19";
            /// <summary>
            /// 隔离
            /// </summary>
            public const string GeLi = "20";
            /// <summary>
            /// 操作
            /// </summary>
            public const string CaoZuo = "21";
            /// <summary>
            /// 血糖
            /// </summary>
            public const string XueTang = "22";
        }

        /// <summary>
        /// 医技收费方式
        /// </summary>
        public class ShouFeiFS_YiJi
        {
            /// <summary>
            /// 应急收费
            /// </summary>
            public const string YingJiSF = "-1";
            /// <summary>
            /// 普通收费
            /// </summary>
            public const string PuTongSF = "0";
            /// <summary>
            /// 门诊发票报销
            /// </summary>
            public const string MenZhenFP = "1";
            /// <summary>
            /// 外院治疗报销
            /// </summary>
            public const string WaiYuanZL = "2";
            /// <summary>
            /// 自动记账项目
            /// </summary>
            public const string ZiDongJZ = "3";
            /// <summary>
            /// 给药方式
            /// </summary>
            public const string GeiYaoFS = "4";
            /// <summary>
            /// 煎药方式
            /// </summary>
            public const string JianYaoFS = "5";
            /// <summary>
            /// 皮试
            /// </summary>
            public const string PiShi = "6";
            /// <summary>
            /// 挂号补收
            /// </summary>
            public const string GuaHaoBS = "7";
            /// <summary>
            /// 诊疗代收
            /// </summary>
            public const string ZhenLiaoDS = "8";
            /// <summary>
            /// 附加收费（一次性材料费）
            /// </summary>
            public const string FuJiaSF = "9";
            /// <summary>
            /// 检验（一次性材料费）
            /// </summary>
            public const string JianYanCL = "10";
            /// <summary>
            /// 诊间处置
            /// </summary>
            public const string ZhenJianCZ = "11";
            /// <summary>
            /// 体检
            /// </summary>
            public const string TiJian = "12";
            /// <summary>
            /// 检验
            /// </summary>
            public const string JianYan = "13";
            /// <summary>
            /// 检查
            /// </summary>
            public const string JianCha = "14";
            /// <summary>
            /// 手术
            /// </summary>
            public const string ShouShu = "15";
            /// <summary>
            /// 麻醉
            /// </summary>
            public const string MaZui = "16";
            /// <summary>
            /// 挂号代收项目
            /// </summary>
            public const string GuaHaoDS = "17";
            /// <summary>
            /// 给药方式（外配的）
            /// </summary>
            public const string WaiPeiGYFS = "18";
            /// <summary>
            /// 治疗单
            /// </summary>
            public const string ZhiLiaoDan = "19";
            /// <summary>
            /// 治疗单补录费用
            /// </summary>
            public const string ZhiLiaoDanBL = "20";
            /// <summary>
            /// 用血
            /// </summary>
            public const string YongXue = "21";
            /// <summary>
            /// 医技费用录入
            /// </summary>
            public const string YiJiFYLR = "21";
            /// <summary>
            /// 收费划价
            /// </summary>
            public const string ShouFeiHJ = "22";
            /// <summary>
            /// 草药加工费
            /// </summary>
            public const string CaoYaoJGF = "23";
            /// <summary>
            /// 挂号诊疗费补差(诊间完成接诊)
            /// </summary>
            public const string GuaHaoZLFBC = "24";
            /// <summary>
            /// 普通皮试
            /// </summary>
            public const string PuTongPS = "61";
            /// <summary>
            /// 原液皮试
            /// </summary>
            public const string YuanYePS = "62";
        }

        /// <summary>
        /// 关联类型
        /// </summary>
        public class GuanLianLX
        {
            /// <summary>
            /// 处方
            /// </summary>
            public const string ChuFang = "1";
            /// <summary>
            /// 医技
            /// </summary>
            public const string YiJi = "2";
            /// <summary>
            /// 挂号
            /// </summary>
            public const string GuaHao = "3";
            /// <summary>
            /// 划价
            /// </summary>
            public const string HuaJia = "4";
        }

        /// <summary>
        /// 加解锁类型
        /// </summary>
        public class JiaJieSuoLX
        {
            /// <summary>
            /// 加锁
            /// </summary>
            public const int JiaSuo = 1;
            /// <summary>
            /// 解锁
            /// </summary>
            public const int JieSuo = 2;
        }

        /// <summary>
        /// 诊断类别
        /// </summary>
        public class ZhenDuanLB
        {
            /// <summary>
            /// 主诊断
            /// </summary>
            public const string ZhuZhenDuan = "1";
            /// <summary>
            /// 次诊断
            /// </summary>
            public const string CiZhenDuan = "2";
        }

        /// <summary>
        /// 毒理分类
        /// </summary>
        public class DuLiFL
        {
            /// <summary>
            /// 普通
            /// </summary>
            public const string PuTong = "0";
            /// <summary>
            /// 毒
            /// </summary>
            public const string Du = "1";
            /// <summary>
            /// 麻醉
            /// </summary>
            public const string MaZui = "2";
            /// <summary>
            /// 精神1
            /// </summary>
            public const string JingShen1 = "3";
            /// <summary>
            /// 精神2
            /// </summary>
            public const string JingShen2 = "5";

            public static Dictionary<string, string> DataSource { get; set; }

            static DuLiFL()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(PuTong, "普通");
                DataSource.Add(Du, "毒");
                DataSource.Add(MaZui, "麻醉");
                DataSource.Add(JingShen1, "精神1");
                DataSource.Add(JingShen2, "精神2");
            }
        }
        /// <summary>
        /// 收费类型
        /// </summary>
        public class ShouFeiLX
        {
            /// <summary>
            /// 普通挂号
            /// </summary>
            public const string ShouFeiLX_PuTongGH = "10";
            /// <summary>
            /// 急诊挂号
            /// </summary>
            public const string ShouFeiLX_JiZhenGH = "11";
            /// <summary>
            /// 普通门诊
            /// </summary>
            public const string ShouFeiLX_PuTongMZ = "20";
            /// <summary>
            /// 急诊门诊
            /// </summary>
            public const string ShouFeiLX_JiZhenMZ = "21";
            /// <summary>
            /// 规定病种
            /// </summary>
            public const string ShouFeiLX_GuiDingBZ = "30";
            /// <summary>
            /// 体检结算
            /// </summary>
            public const string ShouFeiLX_TiJianJS = "40";
            /// <summary>
            /// 急诊留观
            /// </summary>
            public const string ShouFeiLX_JiZhenLG = "50";
            /// <summary>
            /// 生育保险
            /// </summary>
            public const string ShouFeiLX_ShengYuBX = "60";

            public static Dictionary<string, string> DataSource { get; set; }

            static ShouFeiLX()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(ShouFeiLX_PuTongGH, "普通挂号");
                DataSource.Add(ShouFeiLX_JiZhenGH, "急诊挂号");
                DataSource.Add(ShouFeiLX_PuTongMZ, "普通门诊");
                DataSource.Add(ShouFeiLX_JiZhenMZ, "急诊门诊");
                DataSource.Add(ShouFeiLX_GuiDingBZ, "规定病种");
                DataSource.Add(ShouFeiLX_TiJianJS, "体检结算");
                DataSource.Add(ShouFeiLX_JiZhenLG, "急诊留观");
                DataSource.Add(ShouFeiLX_ShengYuBX, "生育保险");
            }
        }

        public class ShouFeiLB
        {
            /// <summary>
            /// 挂号
            /// </summary>
            public const string ShouFeiLB_GuaHao = "1";
            /// <summary>
            /// 收费
            /// </summary>
            public const string ShouFeiLB_ShouFei = "2";
            
            public static Dictionary<string, string> DataSource { get; set; }

            static ShouFeiLB()
            {
                DataSource = new Dictionary<string, string>();
                DataSource.Add(ShouFeiLB_GuaHao, "挂号");
                DataSource.Add(ShouFeiLB_ShouFei, "收费");
            }
        }
    }
}
