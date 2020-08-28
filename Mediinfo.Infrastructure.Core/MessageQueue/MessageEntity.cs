using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    public class MessageEntity
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public OperationType Type { get; set; }

        /// <summary>
        /// 实体
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 实体主键
        /// </summary>
        public List<string> KeyNameList { get; set; } = new List<string>();

        /// <summary>
        /// 当前值
        /// </summary>
        public Dictionary<string, object> CurrentValues { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 原始值
        /// </summary>
        public Dictionary<string, object> OriginalValues { get; set; } = new Dictionary<string, object>();
    }

    public enum OperationType : int
    {
        ADDED = 1,
        DELETED = 2,
        MODIFIED = 3
    }
}
