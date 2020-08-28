using System;
using Mediinfo.Enterprise.Exceptions;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// 构造sql语句
    /// </summary>
    public class CreateSql
    {
        /// <summary>
        /// 构造查询语句
        /// </summary>
        /// <param name="Entity">DTO</param>
        /// <returns></returns>
        public static string Build(DTOBase Entity)
        {
           StringBuilder sql = new StringBuilder(256);

            var type = Entity.GetType();

            sql.Append("select ");

            if (Entity.SelectedColumns.Count == 0)
            {
                type.GetProperties().ToList().ForEach(o =>
                {
                    var notfieldAttr = o.GetCustomAttributes(typeof(NotFieldAttribute), false);
                    if (!notfieldAttr.Any())
                    {
                        var dattr = o.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (!dattr.Any())
                        {
                            var fattr = o.GetCustomAttributes(typeof(FictitiousAttribute), false);
                            if (!fattr.Any())
                            {
                                throw new DTOException("属性" + o.Name + "未标记为DescriptionAttribute或者FictitiousAttribute");
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace((fattr[0] as FictitiousAttribute)?.DescribColmun))
                                {
                                    sql.Append(o);
                                    sql.Append(',');
                                }
                                else
                                {
                                    sql.Append((fattr[0] as FictitiousAttribute)?.DescribColmun);
                                    sql.Append(' ');
                                    sql.Append(o);
                                    sql.Append(',');
                                }
                            }
                        }
                        else
                        {
                            sql.Append(o.Name);
                            sql.Append(',');
                        }
                    }
                });
            }
            else
            {
                var propertys = type.GetProperties().ToList();
                Entity.SelectedColumns.ForEach(o =>
                {
                    var property = propertys.FirstOrDefault(p => o == p.Name);
                    if (property == null)
                    {
                        throw new DTOException("在实体" + type.Name + "中,未找到属性" + o);
                    }
                    else
                    {
                        var dattr = property.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (!dattr.Any())
                        {
                            var fattr = property.GetCustomAttributes(typeof(FictitiousAttribute), false);
                            if (!fattr.Any())
                            {
                                throw new DTOException("属性" + o + "未标记为DescriptionAttribute或者FictitiousAttribute");
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace((fattr[0] as FictitiousAttribute)?.DescribColmun))
                                {
                                    sql.Append(o);
                                    sql.Append(',');
                                }
                                else
                                {
                                    sql.Append((fattr[0] as FictitiousAttribute)?.DescribColmun);
                                    sql.Append(' ');
                                    sql.Append(o);
                                    sql.Append(',');
                                }
                            }
                        }
                        else
                        {
                            sql.Append(o);
                            sql.Append(',');
                        }
                    }
                });
            }
            sql = sql.Remove(sql.Length - 1, 1);
            sql.Append(" from (");

            if (string.IsNullOrWhiteSpace(Entity.GetDefaultSQL()))
            {
                if (string.IsNullOrWhiteSpace(Entity.QuerySql) || Entity.QuerySql.ToUpper().IndexOf("Select", StringComparison.Ordinal) < 0)
                {
                    throw new DTOException("Sql语句无法执行,请检查传入的WHERE条件和" + type.Name + "类是否标记了DefaultSql属性");
                }

                sql.AppendFormat(" {0}", Entity.QuerySql);
                sql.Append(")");
            }
            else
            {
                sql.Append(Entity.GetDefaultSQL());
                sql.Append(")");
                sql.AppendFormat(" {0}", Entity.QuerySql);
            }
            return sql.ToString();
        }
    }
}
