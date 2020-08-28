namespace Mediinfo.Infrastructure.Core.MessageQueue.QueueEntity
{
    public class Garbage_collection
    {
        /// <summary>
        /// Minor_gcs
        /// </summary>
        public int minor_gcs { get; set; }

        /// <summary>
        /// Fullsweep_after
        /// </summary>
        public int fullsweep_after { get; set; }

        /// <summary>
        /// Min_heap_size
        /// </summary>
        public int min_heap_size { get; set; }

        /// <summary>
        /// Min_bin_vheap_size
        /// </summary>
        public int min_bin_vheap_size { get; set; }

        /// <summary>
        /// Max_heap_size
        /// </summary>
        public int max_heap_size { get; set; }
    }

}
