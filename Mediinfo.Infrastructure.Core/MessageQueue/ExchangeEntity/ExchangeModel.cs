namespace Mediinfo.Infrastructure.Core.MessageQueue.ExchangeEntity
{
    /// <summary>
    /// 交换机信息
    /// </summary>
    public class ExchangeModel
    {
        /// <summary>
        /// HIS-GongYong_EXCHANGE
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public string vhost { get; set; }

        /// <summary>
        /// topic
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Durable
        /// </summary>
        public bool durable { get; set; }

        /// <summary>
        /// Auto_delete
        /// </summary>
        public bool auto_delete { get; set; }

        /// <summary>
        /// Internal
        /// </summary>
        public bool Internal { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        public Arguments arguments { get; set; }

        /// <summary>
        /// Message_stats
        /// </summary>
        public Message_stats message_stats { get; set; }
    }
}
