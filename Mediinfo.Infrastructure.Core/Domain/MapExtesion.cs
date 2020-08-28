using Mediinfo.DTO.Core;
using Mediinfo.Enterprise.Exceptions;
using Mediinfo.Infrastructure.Core.Entity;

using NLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Mediinfo.Infrastructure.Core.Domain
{
    /// <summary>
    /// DTO和DOMAIN的转换
    /// </summary>
    public static partial class MapExtesion
    {
        /// <summary>
        /// 通过DTO对象更新DBEntity
        /// </summary>
        /// <param name="entity">DBEntity</param>
        /// <param name="margeEntity">DTO对象</param>
        /// <param name="mergeUpdateOnly">是否只更新变化的数据</param>
        public static void MargeDTO<T, P>(this T entity, P margeEntity, bool mergeUpdateOnly = true)
            where T : EntityBase
            where P : DTOBase
        {
            if (margeEntity.GetState() == DTOState.UnChange)
            {
                return;
            }

            var Tproperties = typeof(T).GetProperties();
            foreach (var property in typeof(P).GetProperties())
            {
                if (!margeEntity.OriginalValues.ContainsKey(property.Name) && mergeUpdateOnly)
                    continue;

                object value = property.GetValue(margeEntity);
                //if (value != null)
                //{
                var tproperty = Tproperties.Where(o => o.Name == property.Name && o.CustomAttributes.Where(p => p.AttributeType == typeof(KeyAttribute)).FirstOrDefault() == null).FirstOrDefault();
                if (tproperty != null)
                {
                    if (value == null)
                    {
                        tproperty.SetValue(entity, null);
                    }
                    else if (tproperty.PropertyType.BaseType == typeof(ValueType)
                          && tproperty.PropertyType.GenericTypeArguments.Length > 0)
                    {
                        tproperty.SetValue(entity, Convert.ChangeType(value, tproperty.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                    {
                        tproperty.SetValue(entity, value);
                    }
                }
                //}
            }
        }

        /// <summary>
        ///合并domian
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="margeEntity"></param>
        public static void MargeDB<T>(this T entity, T margeEntity)
             where T : EntityBase
        {
            foreach (var property in typeof(T).GetProperties().Where(o => o.CustomAttributes.Where(p => p.AttributeType == typeof(KeyAttribute)).FirstOrDefault() == null))
            {
                object value = property.GetValue(margeEntity);
                if (value != null)
                {
                    if (property.PropertyType.BaseType == typeof(ValueType)
                              && property.PropertyType.GenericTypeArguments.Length > 0)
                    {
                        property.SetValue(entity, Convert.ChangeType(value, property.PropertyType.GenericTypeArguments[0]));
                    }
                    else
                    {
                        property.SetValue(entity, value);
                    }
                }
            }
        }

        /// <summary>
        /// 合并实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="margeEntity"></param>
        public static void Marge<T>(this T entity, T margeEntity)
             where T : DTOBase
        {
            foreach (var property in typeof(T).GetProperties())
            {
                object value = property.GetValue(margeEntity);
                if (value != null)
                {
                    property.SetValue(entity, value);
                }
            }
        }

        /// <summary>
        /// domain转成DTO
        /// </summary>
        /// <typeparam name="W">domian</typeparam>
        /// <typeparam name="T">dto</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> DBToE<W, T>(this List<W> entitys)
           where W : EntityBase
           where T : DTOBase
        {
            return MapList<W, T>(entitys);
        }

        /// <summary>
        /// DTO转换成domian
        /// </summary>
        /// <typeparam name="W">dto</typeparam>
        /// <typeparam name="T">domian</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> EToDB<W, T>(this List<W> entitys)
           where W : DTOBase
           where T : EntityBase
        {
            return MapList<W, T>(entitys);
        }

        /// <summary>
        /// 复制domain
        /// </summary>
        /// <typeparam name="W">EntityBase</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> DBToDB<W, T>(this List<W> entitys)
          where W : EntityBase
          where T : EntityBase
        {
            return MapList<W, T>(entitys);
        }

        /// <summary>
        /// 复制DTO
        /// </summary>
        /// <typeparam name="W">DTOBase</typeparam>
        /// <typeparam name="T">DTOBase</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> EToE<W, T>(this List<W> entitys)
         where W : DTOBase
         where T : DTOBase
        {
            return MapList<W, T>(entitys);
        }

        /// <summary>
        /// domian转换成dto
        /// </summary>
        /// <typeparam name="W">EntityBase</typeparam>
        /// <typeparam name="T">DTOBase</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T DBToE<W, T>(this W entity)
           where W : EntityBase
           where T : DTOBase
        {
            return MapEntity<W, T>(entity);
        }

        /// <summary>
        /// dto转换成domian
        /// </summary>
        /// <typeparam name="W">DTOBase</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T EToDB<W, T>(this W entity)
           where W : DTOBase
           where T : EntityBase
        {
            return MapEntity<W, T>(entity);
        }

        /// <summary>
        /// 复制domian
        /// </summary>
        /// <typeparam name="W">EntityBase</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T DBToDB<W, T>(this W entity)
          where W : EntityBase
          where T : EntityBase
        {
            return MapEntity<W, T>(entity);
        }

        /// <summary>
        /// 复制dto
        /// </summary>
        /// <typeparam name="W">DTOBase</typeparam>
        /// <typeparam name="T">DTOBase</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T EToE<W, T>(this W entity)
         where W : DTOBase
         where T : DTOBase
        {

            return MapEntity<W, T>(entity);
        }

        /// <summary>
        /// QueryDTO转换成domian
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T ToDB<W, T>(this W entity)
        where W : QueryDTO
        where T : EntityBase, new()
        {
            var TableAttrList = typeof(T).GetCustomAttributes(typeof(TableAttribute), false);
            string tableName = string.Empty;
            if (TableAttrList.Count() == 1)
            {
                tableName = (TableAttrList[0] as TableAttribute).Name.ToUpper();
            }
            else
            {
                throw new DTOException(typeof(T).Name + "类未标记TableAttribute");
            }
            Dictionary<string, object> values = new Dictionary<string, object>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                var cattr = o.GetCustomAttributes(typeof(TableColumnAttribute), false);
                if (cattr.Count() == 1)
                {
                    if ((cattr[0] as TableColumnAttribute).TableName.ToUpper() == tableName)
                    {
                        values.Add((cattr[0] as TableColumnAttribute).ColumnName.ToUpper(), (o.GetValue(entity) as Property).Value);
                    }
                }
            });
            T t = new T();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                if (values.ContainsKey(o.Name.ToUpper()))
                {
                    if (values[o.Name.ToUpper()] != DBNull.Value && values[o.Name.ToUpper()] != null)
                    {
                        if (values[o.Name.ToUpper()].GetType().BaseType == typeof(ValueType)
                               && o.PropertyType.GenericTypeArguments.Length > 0)
                        {
                            o.SetValue(t, Convert.ChangeType(values[o.Name.ToUpper()], o.PropertyType.GenericTypeArguments[0]));
                        }
                        else
                        {
                            o.SetValue(t, values[o.Name.ToUpper()]);
                        }
                    }
                }
            });
            return t;
        }

        /// <summary>
        /// QueryDTO转换成domian
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> ToDB<W, T>(this List<W> entitys)
        where W : QueryDTO
        where T : EntityBase, new()
        {
            var TableAttrList = typeof(T).GetCustomAttributes(typeof(TableAttribute), false);
            string tableName = string.Empty;
            if (TableAttrList.Count() == 1)
            {
                tableName = (TableAttrList[0] as TableAttribute).Name.ToUpper();
            }
            else
            {
                throw new DTOException(typeof(T).Name + "类未标记TableAttribute");
            }
            List<Tuple<string, Type, Action<object, object>>> tdic = new List<Tuple<string, Type, Action<object, object>>>();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                tdic.Add(new Tuple<string, Type, Action<object, object>>(o.Name.ToUpper(), (o.PropertyType.GenericTypeArguments.Length == 0 ? null : o.PropertyType.GenericTypeArguments[0]), o.SetValue));
            });
            Dictionary<string, Func<object, object>> values = new Dictionary<string, Func<object, object>>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                var cattr = o.GetCustomAttributes(typeof(TableColumnAttribute), false);
                if (cattr.Count() == 1)
                {
                    if ((cattr[0] as TableColumnAttribute).TableName.ToUpper() == tableName)
                    {
                        values.Add((cattr[0] as TableColumnAttribute).ColumnName.ToUpper(), o.GetValue);
                    }
                }
            });
            List<T> ts = new List<T>();
            entitys.ForEach(o =>
            {
                T t = new T();
                tdic.ForEach(p =>
                {
                    if (values.ContainsKey(p.Item1))
                    {
                        try
                        {
                            if ((values[p.Item1](o) as Property).Value != DBNull.Value && (values[p.Item1](o) as Property).Value != null)
                            {
                                if ((values[p.Item1](o) as Property).Value.GetType().BaseType == typeof(ValueType)
                                && p.Item2 != null)
                                {
                                    p.Item3(t, Convert.ChangeType((values[p.Item1](o) as Property).Value, p.Item2));
                                }
                                else
                                {
                                    p.Item3(t, (values[p.Item1](o) as Property).Value);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                });
                ts.Add(t);
            });
            return ts;
        }

        /// <summary>
        /// QueryDTO转换成domian
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static T ToDTO<W, T>(this W entity)
        where W : QueryDTO
        where T : DTOBase, new()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                var value = o.GetValue(entity);
                if (!dic.ContainsKey(o.Name.ToUpper()))
                {
                    dic.Add(o.Name.ToUpper(), (value as Property).Value);
                }
            });
            T t = new T();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                if (dic.ContainsKey(o.Name.ToUpper()))
                {
                    if (o.Name.ToUpper() == "STATE")
                    {
                        t.SetState(entity.GetState());
                    }
                    else if (dic[o.Name.ToUpper()] != DBNull.Value && dic[o.Name.ToUpper()] != null)
                    {
                        if (dic[o.Name.ToUpper()].GetType().BaseType == typeof(ValueType)
                               && o.PropertyType.GenericTypeArguments.Length > 0)
                        {
                            o.SetValue(t, Convert.ChangeType(dic[o.Name.ToUpper()], o.PropertyType.GenericTypeArguments[0]));
                        }
                        else
                        {
                            o.SetValue(t, dic[o.Name.ToUpper()]);
                        }
                    }
                }
            });
            return t;
        }

        /// <summary>
        /// 复制QueryDTO
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">QueryDTO</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T ToQueryDTO<W, T>(this W entity)
      where W : QueryDTO
      where T : QueryDTO, new()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                if (!dic.ContainsKey(o.Name.ToUpper()))
                {
                    dic.Add(o.Name.ToUpper(), o.GetValue(entity));
                }
            });
            T t = new T();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                if (dic.ContainsKey(o.Name.ToUpper()))
                {
                    if (o.Name.ToUpper() == "STATE")
                    {
                        t.SetState(entity.GetState());
                    }
                    else if (dic[o.Name.ToUpper()] is Property && (dic[o.Name.ToUpper()] as Property).Value != DBNull.Value && (dic[o.Name.ToUpper()] as Property).Value != null)
                    {
                        o.SetValue(t, (dic[o.Name.ToUpper()] as Property).Value);
                    }
                }
            });
            return t;
        }

        /// <summary>
        /// 复制QueryDTO
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">QueryDTO</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> ToQueryDTO<W, T>(this List<W> entitys)
        where W : QueryDTO
        where T : QueryDTO, new()
        {
            Dictionary<string, Func<object, object>> wdic = new Dictionary<string, Func<object, object>>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                wdic.Add(o.Name.ToUpper(), o.GetValue);
            });
            List<KeyValuePair<string, Action<object, object>>> tdic = new List<KeyValuePair<string, Action<object, object>>>();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                tdic.Add(new KeyValuePair<string, Action<object, object>>(o.Name.ToUpper(), o.SetValue));
            });
            List<T> ts = new List<T>();
            entitys.ForEach(o =>
            {
                T t = new T();
                tdic.ForEach(p =>
                {
                    if (p.Key == "STATE")
                    {
                        t.SetState(o.GetState());
                    }
                    else if (wdic.ContainsKey(p.Key))
                    {
                        try
                        {
                            if (wdic[p.Key](o) is Property && (wdic[p.Key](o) as Property).Value != DBNull.Value && (wdic[p.Key](o) as Property).Value != null)
                            {
                                p.Value(t, (wdic[p.Key](o) as Property).Value);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                });
                ts.Add(t);
            });
            return ts;
        }

        /// <summary>
        /// QueryDTO转DTO
        /// </summary>
        /// <typeparam name="W">QueryDTO</typeparam>
        /// <typeparam name="T">DTOBase</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> ToDTO<W, T>(this List<W> entitys)
        where W : QueryDTO
        where T : DTOBase, new()
        {
            Dictionary<string, Func<object, object>> wdic = new Dictionary<string, Func<object, object>>();
            typeof(W).GetProperties().ToList().ForEach(o =>
            {
                wdic.Add(o.Name.ToUpper(), o.GetValue);
            });
            List<Tuple<string, Type, Action<object, object>>> tdic = new List<Tuple<string, Type, Action<object, object>>>();
            typeof(T).GetProperties().ToList().ForEach(o =>
            {
                tdic.Add(new Tuple<string, Type, Action<object, object>>(o.Name.ToUpper(), (o.PropertyType.GenericTypeArguments.Length == 0 ? null : o.PropertyType.GenericTypeArguments[0]), o.SetValue));
            });
            List<T> ts = new List<T>();
            entitys.ForEach(o =>
            {
                T t = new T();
                tdic.ForEach(p =>
                {
                    if (p.Item1 == "STATE")
                    {
                        t.SetState(o.GetState());
                    }
                    else if (wdic.ContainsKey(p.Item1))
                    {
                        try
                        {
                            if ((wdic[p.Item1](o) as Property).Value != DBNull.Value && (wdic[p.Item1](o) as Property).Value != null)
                            {
                                if ((wdic[p.Item1](o) as Property).Value.GetType().BaseType == typeof(ValueType)
                                && p.Item2 != null)
                                {
                                    p.Item3(t, Convert.ChangeType((wdic[p.Item1](o) as Property).Value, p.Item2));
                                }
                                else
                                {
                                    p.Item3(t, (wdic[p.Item1](o) as Property).Value);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                });
                ts.Add(t);
            });
            return ts;
        }

        /// <summary>
        /// 合并list
        /// </summary>
        /// <typeparam name="W">class</typeparam>
        /// <typeparam name="T">class</typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> MapList<W, T>(List<W> entitys)
            where T : class
            where W : class
        {

            return entitys.Select(s => MapEntity<W, T>(s)).ToList();
            //return Mapper.Map<List<W>, List<T>>(dbEntity);
        }

        /// <summary>
        /// 合并类型
        /// </summary>
        /// <typeparam name="W">class</typeparam>
        /// <typeparam name="T">class</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T MapEntity<W, T>(W entity)
            where T : class
            where W : class
        {
            //Expression+Generic+cache
            return ExpressionGenericMapper<W, T>.Trans(entity);
            //ObjectsMapper<W, T> mapper = ObjectMapperManager.DefaultInstance.GetMapper<W, T>();
            //return mapper.Map(entity);
            //return Mapper.Map<W, T>(entity);
        }
    }
}
