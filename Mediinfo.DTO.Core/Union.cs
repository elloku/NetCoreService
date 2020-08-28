using System;
using System.Collections.Generic;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 构造Union
    /// </summary>
    public class Union
    {
        public List<KeyValuePair<string, string>> Selects { get; set; }
        public List<Property> GroupBys { get; set; }
        public List<Property> OrderBys { get; set; }
        public Join Joins { get; set; }
        public Where QueryWhere { get; set; }
        public Where QueryHaving { get; set; }
        public Type DTOtype { get; set; }
    }
}
