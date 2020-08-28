namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// DTO实体状态
    /// </summary>
    public enum DTOState 
    {
        /// <summary>
        /// 新增
        /// </summary>
        New = 0,

        /// <summary>
        /// 修改
        /// </summary>
        Update = 1,
        
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2 ,

        /// <summary>
        /// 未变化
        /// </summary>
        UnChange = 3 ,

        /// <summary>
        /// 取消状态（New->Cancel)
        /// </summary>
        Cancel = 4  
    }
}
