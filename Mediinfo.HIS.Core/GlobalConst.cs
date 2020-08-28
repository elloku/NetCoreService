namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public partial class HISGlobalHelper
    {
        /// <summary>
        /// 全局静态配置
        /// </summary>
        public static class GlobalConst
        {
            public const int gs_ShuLiangXSW = 4;
            // 药品分类类别
            public const string GY_YaoPinFL_FenLeiLB = "1";
            // 库存管理类型
            public const string KF_KuCunGLLX = "1110";
            // 库房使用标志
            public const int GY_KuFangSY_YaoKu = 1;         // 药库使用
            public const int GY_KuFangSY_MenYao = 2;        // 门药使用
            public const int GY_KuFangSY_BingYao = 3;       // 病药使用
            public const int GY_KuFangSY_MenYao_ZY = 4;     // 门药使用_住院
            public const int GY_KuFangSY_BingYao_MZ = 5;    // 病药使用_门诊
            public const int GY_KuFangSY_JingMaiP = 6;      // 静脉配
            public const int GY_KuFangSY_ZhiJi = 7;         // 制剂(全部)
            public const int GY_KuFangSY_ZhiJiCP = 8;       // 制剂(成品)
            public const int GY_KuFangSY_ZhiJiFCP = 9;      // 制剂(非成品)
            public const int GY_KuFangSY_ZhongYaoKLSY = 10;     // 中药散配颗粒
            public const int GY_KuFangSY_MenYao_BingYao = 11;   // 根据应用ID的前两位，自动取门药使用或病药使用
            public const int GY_KuFangSY_ZhongYaoKLZBSY = 12;   // 中药颗粒整包使用  HR3-13306(171599)
            public const int GY_KuFangSY_ZhongYaoKLSY_MenYao_ZY = 13;       // 住院用的散配颗粒
            public const int GY_KuFangSY_ZhongYaoKLZBSY_MenYao_ZY = 14;     // 住院用的整包颗粒
            public const int GY_KuFangSY_YinPianXBZ = 15;       // 门诊饮片小包装 HR3-20532(238252)
            public const int GY_KuFangSY_YinPianXBZ_ZY = 16;    // 住院饮片小包装 HR3-20532(238252)
            // 处方类型
            public const string GY_ChuFangLX_XiYao = "1";       // 西药
            public const string GY_ChuFangLX_ChengYao = "2";    // 成药
            public const string GY_ChuFangLX_CaoYao = "3";      // 草药
            public const string GY_ChuFangLX_ShouFeiXM = "4";   // 收费项目
            // 支付方式
            public const string GY_ZhiFuFS_XianJin = "1";  // 现金
            public const string GY_ZhiFuFS_ZhiPiao = "2";  // 支票
            public const string GY_ZhiFuFS_HuiPiao = "3";  // 汇票
            // 票据类型
            public const string GY_PiaoJuLX_GuaHao = "1";       // 挂号票据
            public const string GY_PiaoJuLX_MenZhenSF = "2";    // 门诊收费票据
            public const string GY_PiaoJuLX_ZhuYuanSF = "3";    // 住院收费票据
            public const string GY_PiaoJuLX_MenZhenYJK = "4";   // 门诊预交款票据
            public const string GY_PiaoJuLX_ZhuYuanYJK = "5";   // 住院预交款票据
            // 费用类别
            public const string GY_FeiYongLB_ZiFei = "01";      // 自费
            // 费用性质
            public const string GY_FeiYongXZ_ZiFei = "XJ01";    // 自费
            // 门诊收费类型
            public const string MZ_ShouFeiLX_PuTongGH = "10";   // 普通挂号
            public const string MZ_ShouFeiLX_JiZhenGH = "11";   // 急诊挂号
            public const string MZ_ShouFeiLX_PuTongSF = "20";   // 普通收费
            public const string MZ_ShouFeiLX_JiZhenSF = "21";   // 急诊收费
            public const string MZ_ShouFeiLX_GuiDingBZ = "30";  // 规定病种
            public const string MZ_ShouFeiLX_TiJianJS = "40";   // 体检结算
            public const string MZ_ShouFeiLX_JIZHENLG = "50";   // 急诊留观 Add By YUB 2010-12-07 HR3-5523
            // 皮试标志
            public const int GY_PiShiBZ_NO = 0;         // 非皮试
            public const int GY_PiShiBZ_PuTong = 1;     // 普通
            public const int GY_PiShiBZ_YuanYE = 2;     // 原液
            // 皮试结果
            public const string GY_PiShiCLYJ_TongGuo = "0";     // 通过
            public const string GY_PiShiCLYJ_WeiTongGuo = "1";  // 未通过
            public const string GY_PiShiCLYJ_MianShi = "2";     // 免试
            // 毒理分类
            public const string GY_DuLiFL_PuTong = "0";         // 普通
            public const string GY_DuLiFL_DuXing = "1";         // 毒性
            public const string GY_DuLiFL_MaZui = "2";          // 麻醉
            public const string GY_DuLiFL_JingShen1 = "3";      // 精神1
            public const string GY_DuLiFL_JingShen2 = "5";      // 精神2
            // 坐诊类型
            public const string GY_ZuoZhenLX_Putong = "1";      // 普通
            public const string GY_ZuoZhenLX_ZhuanJia = "4";    // 专家
            public const string GY_ZuoZhenLX_TeShu = "-1";      // 特殊
        }
    }
}
