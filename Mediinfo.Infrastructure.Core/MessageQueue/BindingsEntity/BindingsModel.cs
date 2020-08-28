using Mediinfo.Infrastructure.Core.MessageQueue.ExchangeEntity;

using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue.BindingsEntity
{
    /// <summary>
    /// 队列和路由绑定
    /// </summary>
    public class BindingsModel
    {
        public List<ExchangeModel> ExchangeModelList { get; set; } = new List<ExchangeModel>();

        /// <summary>
        /// HIS-GongYong_EXCHANGE
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public string vhost { get; set; }

        /// <summary>
        /// HIS6_GONGYONG
        /// </summary>
        public string destination { get; set; }

        /// <summary>
        /// queue
        /// </summary>
        public string destination_type { get; set; }

        /// <summary>
        /// lo
        /// </summary>
        public string routing_key { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        public Arguments arguments { get; set; }

        /// <summary>
        /// lo
        /// </summary>
        public string properties_key { get; set; }
    }
}
