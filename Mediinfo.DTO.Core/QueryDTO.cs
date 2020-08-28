using Mediinfo.Enterprise.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 查询状态的DTO
    /// </summary>
    public class QueryDTO : DTOBase
    {
        public List<UnionAll> UnionAlls { get; set; }
        public List<Union> Unions { get; set; }
        public List<Minus> Minuss { get; set;}
        public List<KeyValuePair<string, string>> Selects { get; set; }
        public List<Property> GroupBys { get; set; }
        public List<Property> OrderBys { get; set; }
        public Join Joins { get; set; }
        public Where QueryWhere { get; set; }
        public Where QueryHaving { get; set; }
        public Type DTOtype { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QueryDTO()
        { 
            Unions = new List<Union>();
            DTOtype = this.GetType();
            DTOtype.GetFields().ToList().ForEach(o =>
            {
                if (o.FieldType.FullName == typeof(Property).FullName)
                {
                    o.SetValue(this, new Property() { ColumnName = o.Name });
                }
            });
        }

        /// <summary>
        /// 查询一个自定义字段列表
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual QueryDTO Select(params Property[] properties)
        {
            if (Selects != null)
            {
                throw new DTOException("已经使用过Select方法");
            }
            Selects = new List<KeyValuePair<string, string>>();
            var pros = DTOtype.GetProperties().ToList();
            for (int i = 0; i < properties.Length; i++)
            {
                var column = pros.Where(o => o.Name.ToUpper() == properties[i].ColumnName.ToUpper()).FirstOrDefault();
                if (column == null)
                {
                    throw new DTOException("字段" + properties[i].ColumnName + "没有找到对应的属性");
                }
                else
                {
                    var cattr = column.GetCustomAttributes(typeof(TableColumnAttribute), false);
                    if (cattr.Count() == 0)
                    {
                        var fcattr = column.GetCustomAttributes(typeof(FictitiousAttribute), false);
                        if (fcattr.Count() > 1)
                        {
                            throw new DTOException("属性" + column.Name + "未标记了多个FictitiousAttribute");
                        }
                        else if (fcattr.Count() == 1)
                        {
                            Selects.Add(new KeyValuePair<string, string>((fcattr[0] as FictitiousAttribute).DescribColmun ?? string.Empty, null));
                        }
                        else
                        {
                            throw new DTOException("属性" + column.Name + "未标记为TableColumnAttribute或者FictitiousAttribute");
                        }
                    }
                    else if (cattr.Count() > 1)
                    {
                        throw new DTOException("属性" + column.Name + "标记了多个TableColumnAttribute");
                    }
                    else
                    {
                        var tableColumn = (cattr[0] as TableColumnAttribute).TableName + "." + (cattr[0] as TableColumnAttribute).ColumnName;
                        Selects.Add(new KeyValuePair<string, string>(tableColumn, properties[i].Aggregates.Count > 0 ? properties[i].Aggregates[Selects.Where(o => o.Key == tableColumn).Count()] : string.Empty ));
                    }
                }
            }
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Aggregates.Count > 0)
                {
                    properties[i].Aggregates = new List<string>();
                }
            }
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public virtual QueryDTO Join(Join joins)
        {
            if (Joins != null)
            {
                throw new DTOException("已经使用过Join方法");
            }
            Joins = joins;
            return this;
        }

        /// <summary>
        /// 在使用Where条件时如果需要括括号请使用A & ( B | C ) 不能使用 ( B | C ) & A 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual QueryDTO Where(Where where)
        {
            if (QueryWhere != null)
            {
                throw new DTOException("已经使用过Where方法");
            }
            QueryWhere = where;
            return this;
        }

        /// <summary>
        /// Having
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public virtual QueryDTO Having(Where having)
        {
            if (QueryHaving != null)
            {
                throw new DTOException("已经使用过Having方法");
            }
            QueryHaving = having;
            return this;
        }

        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual QueryDTO GroupBy(params Property[] properties)
        {
            if (GroupBys != null)
            {
                throw new DTOException("已经使用过GroupBy方法");
            }
            if (properties != null)
            {
                var groupBys = new List<Property>();
                properties.ToList().ForEach(o => {
                    groupBys.Add(new Property() {
                        ColumnName = o.ColumnName,
                        Descing = o.Descing,
                    });
                });
                properties.ToList().ForEach(o =>
                {
                    o.Descing = null;
                });
                GroupBys = groupBys;
            }
            
            return this;
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual QueryDTO OrderBy(params Property[] properties)
        {
            if (OrderBys != null)
            {
                throw new DTOException("已经使用过OrderBy方法");
            }
            if (properties != null)
            {
                var orderbys = new List<Property>();
                properties.ToList().ForEach(o => {
                    orderbys.Add(new Property()
                    {
                        ColumnName = o.ColumnName,
                        Descing = o.Descing,
                    });
                });
                properties.ToList().ForEach(o =>
                {
                    o.Descing = null;
                });
                OrderBys = orderbys;
            }
            return this;
        }

        /// <summary>
        /// Union
        /// </summary>
        /// <returns></returns>
        public virtual QueryDTO Union()
        {
            if (OrderBys != null)
            {
                throw new DTOException("使用Union不能使用Order By");
            }
            Union union = new Union();
            union.QueryWhere = QueryWhere;
            union.QueryHaving = QueryHaving;
            union.Selects = Selects;
            union.GroupBys = GroupBys;
            union.OrderBys = OrderBys;
            union.Joins = Joins;
            union.DTOtype = DTOtype;
            Unions.Add(union);
            Selects = null;
            QueryWhere = null;
            QueryHaving = null;
            GroupBys = null;
            OrderBys = null;
            Joins = null;
            return this;
        }

        /// <summary>
        /// UnionAll
        /// </summary>
        /// <returns></returns>
        public virtual QueryDTO UnionAll()
        {
            if (OrderBys != null)
            {
                throw new DTOException("使用Union All不能使用Order By");
            }
            UnionAll union = new UnionAll();
            union.QueryWhere = QueryWhere;
            union.QueryHaving = QueryHaving;
            union.Selects = Selects;
            union.GroupBys = GroupBys;
            union.OrderBys = OrderBys;
            union.Joins = Joins;
            union.DTOtype = DTOtype;
            UnionAlls.Add(union);
            Selects = null;
            QueryWhere = null;
            QueryHaving = null;
            GroupBys = null;
            OrderBys = null;
            Joins = null;
            return this;
        }

        /// <summary>
        /// Minus
        /// </summary>
        /// <returns></returns>
        public virtual QueryDTO Minus()
        {
            Minus minus = new Minus();
            minus.QueryWhere = QueryWhere;
            minus.QueryHaving = QueryHaving;
            minus.Selects = Selects;
            minus.GroupBys = GroupBys;
            minus.OrderBys = OrderBys;
            minus.Joins = Joins;
            minus.DTOtype = DTOtype;
            Minuss.Add(minus);
            Selects = null;
            QueryWhere = null;
            QueryHaving = null;
            GroupBys = null;
            OrderBys = null;
            Joins = null;
            return this;
        }
    }
}
