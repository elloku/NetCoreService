using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue.QueueEntity
{
    public class Backing_queue_status
    {
        /// <summary>
        /// default
        /// </summary>
        public string mode { get; set; }

        /// <summary>
        /// Q1
        /// </summary>
        public int q1 { get; set; }

        /// <summary>
        /// Q2
        /// </summary>
        public int q2 { get; set; }

        /// <summary>
        /// Delta
        /// </summary>
        public List<string> delta { get; set; }

        /// <summary>
        /// Q3
        /// </summary>
        public int q3 { get; set; }

        /// <summary>
        /// Q4
        /// </summary>
        public int q4 { get; set; }

        /// <summary>
        /// Len
        /// </summary>
        public int len { get; set; }

        /// <summary>
        /// infinity
        /// </summary>
        public string target_ram_count { get; set; }

        /// <summary>
        /// Next_seq_id
        /// </summary>
        public int next_seq_id { get; set; }

        /// <summary>
        /// Avg_ingress_rate
        /// </summary>
        public string avg_ingress_rate { get; set; }

        /// <summary>
        /// Avg_egress_rate
        /// </summary>
        public string avg_egress_rate { get; set; }

        /// <summary>
        /// Avg_ack_ingress_rate
        /// </summary>
        public string avg_ack_ingress_rate { get; set; }

        /// <summary>
        /// Avg_ack_egress_rate
        /// </summary>
        public string avg_ack_egress_rate { get; set; }

        /// <summary>
        /// Mirror_seen
        /// </summary>
        public int mirror_seen { get; set; }

        /// <summary>
        /// Mirror_senders
        /// </summary>
        public int mirror_senders { get; set; }
    }
}
