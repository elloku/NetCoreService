using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.DbMessage
{
    /// <summary>
    /// 由数据库更改产生的消息
    /// </summary>
    public class DbMessage
    {
        public bool Changed
        {
            get
            {
                return AddedEntityList.Count + DeletedEntityList.Count + ModifiedEntityList.Count > 0;
            }
        }

        /// <summary>
        /// 涉及到的实体列表
        /// </summary>
        public ISet<string> EntityNameList { get; set; } = new HashSet<string>();
        
        /// <summary>
        /// 新增实体列表
        /// </summary>
        public List<MessageEntity> AddedEntityList { get; set; } = new List<MessageEntity>();
        
        /// <summary>
        /// 删除实体列表
        /// </summary>
        public List<MessageEntity> DeletedEntityList { get; set; } = new List<MessageEntity>();
        
        /// <summary>
        /// 修改实体列表
        /// </summary>
        public List<MessageEntity> ModifiedEntityList { get; set; } = new List<MessageEntity>();
    }
}
