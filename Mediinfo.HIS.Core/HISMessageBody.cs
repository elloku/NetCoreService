using System;

namespace Mediinfo.HIS.Core
{
    /// <summary>
    /// HIS内部消息体
    /// </summary>
    [Serializable]
    public class HISMessageBody
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string ShouJianRen { get; set; }
        /// <summary>
        /// 接收人列表
        /// </summary>
        public string[] Receivers { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string XiaoXiID { get; set; }

        /// <summary>
        /// 消息编码
        /// </summary>
        public string XiaoXiBM { get; set; }

        /// <summary>
        /// 就诊ID
        /// </summary>
        public string BingRenID { get; set; }

        /// <summary>
        /// 病人住院ID
        /// </summary>
        public int MenZhenZyBz { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string XiaoXiBT { get; set; }

        /// <summary>
        /// 消息摘要
        /// </summary>
        public string XiaoXiZY { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public object XiaoXiNR { get; set; }

        /// <summary>
        /// 附件标识
        /// </summary>
        public int FuJianBz { get; set; }

        /// <summary>
        /// 有效时间(分钟数),为0则标识无限期
        /// </summary>
        public DateTime? YouXiaoSj { get; set; }

        /// <summary>
        /// 保密信息标识(需要用户校验才能查看)
        /// </summary>
        public int? BaoMiXxBz { get; set; }

        /// <summary>
        /// 优先级(1.低  2.中 3.高）
        /// </summary>
        public int? YouXianJi { get; set; }

        /// <summary>
        /// 回执标识（标志是否在月度邮件后,发送一个回执给发件人）
        /// </summary>
        public int? HuiZhiBz { get; set; }

        /// <summary>
        /// 提醒类型(1.右下角浮动框 2.弹出消息 0.不提示 )
        /// </summary>
        public string TiXingLx { get; set; }

        /// <summary>
        /// 一次性标识
        /// </summary>
        public int? YiCiXBZ { get; set; }

        /// <summary>
        /// 消息名称
        /// </summary>
        public string XiaoXiMc { get; set; }

        /// <summary>
        /// 消息简称
        /// </summary>
        public string XiaoXiJc { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime FaSongSj { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public string FaSongRen { get; set; }

        public string XiaoXiBD { get; set; }

        /// <summary>
        /// 接收标志
        /// </summary>
        public int? JieShouBZ { get; set; }

        /// <summary>
        /// 消息来源
        /// </summary>
        public string XiaoXiLY { get; set; }
    }
}
