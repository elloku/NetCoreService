namespace Mediinfo.Infrastructure.Core.EventBus
{
    public class MQMessage
    {
        public string MessageType { get; set; }

        public string Channel { get; set; }

        public string Target { get; set; }

        public string HospitalID { get; set; }

        public string JiuZhenKH { get; set; }

        /// <summary>
        /// 触发的数据
        /// </summary>
        public object Messages { get; set; }
    }
}
