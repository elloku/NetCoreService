using Mediinfo.Infrastructure.Core.MessageQueue.BindingsEntity;

using System;
using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue.QueueEntity
{
    /// <summary>
    /// 队列模型
    /// </summary>
    public class QueueModel
    {
        public List<BindingsModel> BindingsModelList { get; set; } = new List<BindingsModel>();

        /// <summary>
        /// Messages_details
        /// </summary>
        public Messages_details messages_details { get; set; }

        /// <summary>
        /// Messages
        /// </summary>
        public int messages { get; set; }

        /// <summary>
        /// Messages_unacknowledged_details
        /// </summary>
        public Messages_unacknowledged_details messages_unacknowledged_details { get; set; }

        /// <summary>
        /// Messages_unacknowledged
        /// </summary>
        public int messages_unacknowledged { get; set; }

        /// <summary>
        /// Messages_ready_details
        /// </summary>
        public Messages_ready_details messages_ready_details { get; set; }

        /// <summary>
        /// Messages_ready
        /// </summary>
        public int messages_ready { get; set; }

        /// <summary>
        /// Reductions_details
        /// </summary>
        public Reductions_details reductions_details { get; set; }

        /// <summary>
        /// Reductions
        /// </summary>
        public int reductions { get; set; }

        /// <summary>
        /// rabbit@WIN-NM43P15CS17
        /// </summary>
        public string node { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        public Arguments arguments { get; set; }

        /// <summary>
        /// Exclusive
        /// </summary>
        public bool exclusive { get; set; }

        /// <summary>
        /// Auto_delete
        /// </summary>
        public bool auto_delete { get; set; }

        /// <summary>
        /// Durable
        /// </summary>
        public bool durable { get; set; }

        /// <summary>
        /// /
        /// </summary>
        public string vhost { get; set; }

        /// <summary>
        /// HIS6_GONGYONG
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Message_bytes_paged_out
        /// </summary>
        public int message_bytes_paged_out { get; set; }

        /// <summary>
        /// Messages_paged_out
        /// </summary>
        public int messages_paged_out { get; set; }

        /// <summary>
        /// Backing_queue_status
        /// </summary>
        public Backing_queue_status backing_queue_status { get; set; }

        /// <summary>
        /// Head_message_timestamp
        /// </summary>
        public string head_message_timestamp { get; set; }

        /// <summary>
        /// Message_bytes_persistent
        /// </summary>
        public int message_bytes_persistent { get; set; }

        /// <summary>
        /// Message_bytes_ram
        /// </summary>
        public int message_bytes_ram { get; set; }

        /// <summary>
        /// Message_bytes_unacknowledged
        /// </summary>
        public int message_bytes_unacknowledged { get; set; }

        /// <summary>
        /// Message_bytes_ready
        /// </summary>
        public int message_bytes_ready { get; set; }

        /// <summary>
        /// Message_bytes
        /// </summary>
        public int message_bytes { get; set; }

        /// <summary>
        /// Messages_persistent
        /// </summary>
        public int messages_persistent { get; set; }

        /// <summary>
        /// Messages_unacknowledged_ram
        /// </summary>
        public int messages_unacknowledged_ram { get; set; }

        /// <summary>
        /// Messages_ready_ram
        /// </summary>
        public int messages_ready_ram { get; set; }

        /// <summary>
        /// Messages_ram
        /// </summary>
        public int messages_ram { get; set; }

        /// <summary>
        /// Garbage_collection
        /// </summary>
        public Garbage_collection garbage_collection { get; set; }

        /// <summary>
        /// running
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Recoverable_slaves
        /// </summary>
        public List<string> recoverable_slaves { get; set; }

        /// <summary>
        /// Synchronised_slave_nodes
        /// </summary>
        public List<string> synchronised_slave_nodes { get; set; }

        /// <summary>
        /// Slave_nodes
        /// </summary>
        public List<string> slave_nodes { get; set; }

        /// <summary>
        /// Consumers
        /// </summary>
        public int consumers { get; set; }

        /// <summary>
        /// Exclusive_consumer_tag
        /// </summary>
        public string exclusive_consumer_tag { get; set; }

        /// <summary>
        /// HIS6
        /// </summary>
        public string policy { get; set; }

        /// <summary>
        /// Consumer_utilisation
        /// </summary>
        public string consumer_utilisation { get; set; }

        /// <summary>
        /// 2018-06-07 3:08:11
        /// </summary>
        public DateTime idle_since { get; set; }

        /// <summary>
        /// Memory
        /// </summary>
        public int memory { get; set; }
    }
}
