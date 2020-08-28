using Mediinfo.DTO.Core;
using Mediinfo.Enterprise;
using Mediinfo.Infrastructure.Core.DBContext;
using Mediinfo.Infrastructure.Core.EventBus;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace Mediinfo.Infrastructure.Core.Service
{
    /// <summary>
    /// 服务基础类
    /// </summary>
    public class ServiceBase : MarshalByRefObject, IService
    {
        /// <summary>
        /// 立即发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void SendMessageImmediately<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.Add<T>(args);
        }
        /// <summary>
        /// 等待提交时发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void SendMessage<T>(T args) where T : MessageEventArgs, new()
        {
            MessageEventBus.AddWaitCommit<T>(args);
        }
        /// <summary>
        /// 数据库链接上下文
        /// </summary>
        public DBContextBase DBContext { get; set; }
        /// <summary>
        /// 应用缓存池上下文
        /// </summary>
        public ServiceContext ServiceContext { get; set; }

        /// <summary>
        /// 获取数据库标准时间 如果数据库时间也不标准就没有办法了
        /// </summary>
        /// <returns></returns>
        protected internal DateTime GetSYSDate()
        {
            return DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault();
        }
        /// <summary>
        /// 获取数据库标准时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime()
        {
            return (DBContext.Database.SqlQuery<DateTime>("select sysdate from dual").FirstOrDefault());
        }

        /// <summary>
        /// 获取实体数据（不需要创建实体）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> Get<T>(string where = null, params object[] parameters) where T : DTOBase, new()
        {
            T entity = new T();

            if (!string.IsNullOrWhiteSpace(where))
                entity.Where(where, parameters);

            var sql = CreateSql.Build((DTOBase)entity);

            List<T> list = null;
            list = GetList<T>(entity, sql, entity.QueryParams);
            
            return list;
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<T> Get<T>(T entity) where T : DTOBase, new()
        {
            var sql = CreateSql.Build((DTOBase)entity);
            
            List<T> list = null;

            list = GetList<T>(entity,sql, entity.QueryParams);

            return list;
        }

        /// <summary>
        /// 内部真正查询数据的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sql"></param>
        /// <param name="Parameties"></param>
        /// <returns></returns>
        internal List<T> GetList<T>(T entity, string sql, Dictionary<string, object> Parameties = null) where T : DTOBase, new()
        {
            List<DbParameter> DbParameters = new List<DbParameter>();

            //初始化查询参数
            var cmd = DBContext.Database.Connection.CreateCommand();
            Parameties.ToList().ForEach(o =>
            {
                var parm = cmd.CreateParameter();
                parm.ParameterName = o.Key;
                parm.Value = o.Value;
                DbParameters.Add(parm);
            });

#if DEBUG
            Stopwatch watch = new Stopwatch();
            watch.Restart();
#endif
            Debug.Print(sql);

            //查询
            var list = DBContext.Database.SqlQuery<T>(sql, DbParameters.ToArray()).ToList();

            //查询完成后将状态置位未更改
            list.ForEach(o =>
            {
                o.SetState( DTOState.UnChange);
                o.SetTraceChange(true);
            });

            DbParameters.Clear();

            //清空查询条件
            entity.ResetQuery();

#if DEBUG
            watch.Stop();
            Debug.Print("--耗时：{0}分,{1}秒,{2}毫秒",watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.Milliseconds);
#endif
            return list;
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type targetType)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                NullableConverter nullableConverter = new NullableConverter(targetType);
                Type convertType = nullableConverter.UnderlyingType;
                return Convert.ChangeType(value, convertType);
            }
            if (value == null && targetType.IsGenericType)
            {
                return Activator.CreateInstance(targetType);
            }
            if (value == null)
            {
                return null;
            }
            if (targetType == value.GetType())
            {
                return value;
            }
            if (targetType.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(targetType, value as string);
                }
                else
                {
                    return Enum.ToObject(targetType, value);
                }
            }
            if (!targetType.IsInterface && targetType.IsGenericType)
            {
                Type innerType = targetType.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(targetType, new object[] { innerValue });
            }
            if (value is string && targetType == typeof(Guid))
            {
                return new Guid(value as string);
            }
            if (value is string && targetType == typeof(Version))
            {
                return new Version(value as string);
            }
            if (!(value is IConvertible))
            {
                return value;
            }
            return Convert.ChangeType(value, targetType);
        }

        //internal List<T> GetList<T>(string sql) where T : DTOBase, new()
        //{
        //    List<T> ts = new List<T>();
        //    var properties = typeof(T).GetProperties();
        //    Dictionary<string, Action<object, object>> setvalues = new Dictionary<string, Action<object, object>>();
        //    for (int i = 0; i < properties.Length; i++)
        //    {
        //        setvalues.Add(properties[i].Name.ToUpper(), properties[i].SetValue);
        //    }
        //    if (DBContext.Database.Connection.State != ConnectionState.Open)
        //    {
        //        DBContext.Database.Connection.Open();
        //    }
        //    var cmd = DBContext.Database.Connection.CreateCommand();
        //    cmd.CommandText = sql;
        //    var read = cmd.ExecuteReader();
        //    while (read.Read())
        //    {
        //        T t = new T();
        //        for (int i = 0; i < read.FieldCount; i++)
        //        {
        //            var column = read.GetName(i);
        //            if (setvalues.ContainsKey(column))
        //            {
        //                if (read[i] != DBNull.Value)
        //                {
        //                    setvalues[column](t, read[i]);
        //                }
        //            }
        //            else
        //            {
        //                throw new DTOException("未找到" + read.GetName(i) + "对应的属性,在" + typeof(T).Name);
        //            }
        //        }
        //        t.State = DTOState.UnChange;
        //        ts.Add(t);
        //    }
        //    if (DBContext.Database.Connection.State != ConnectionState.Open)
        //    {
        //        DBContext.Database.Connection.Close();
        //    }

        //    return ts;
        //}

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        //public DataTable Get(Parm<string> Sql)
        //{
        //    var table = GetTable(Sql.Entity);
        //    return table;
        //}

        //获取datatable
        private DataTable GetTable(string Sql)
        {
            if (DBContext.Database.Connection.State != ConnectionState.Open)
            {
                DBContext.Database.Connection.Open();
            }

            DataTable table = new DataTable();
            var cmd = DBContext.Database.Connection.CreateCommand();
            cmd.CommandText = Sql;

            var read = cmd.ExecuteReader();
            for (int i = 0; i < read.FieldCount; i++)
            {
                //创建table的第一列
                DataColumn Column = new DataColumn();
                //该列的数据类型
                Column.DataType = read.GetFieldType(i);
                //该列得名称
                Column.ColumnName = read.GetName(i);
                table.Columns.Add(Column);
            }

            while (read.Read())
            {
                var row = table.NewRow();
                for (int i = 0; i < read.FieldCount; i++)
                {
                    if (read[i] != DBNull.Value)
                    {
                        row[i] = read[i];
                    }
                }
                table.Rows.Add(row);
            }
            if (DBContext.Database.Connection.State != ConnectionState.Open)
            {
                DBContext.Database.Connection.Close();
            }

            return table;
        }
    }
}
