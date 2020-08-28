using Mediinfo.DTO.Core;
using Mediinfo.Enterprise.Log;
using Mediinfo.Enterprise.PagedResult;
using Mediinfo.Infrastructure.Core.Repository;
using Mediinfo.Infrastructure.Core.UnitOfWork;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Mediinfo.Infrastructure.Core
{
    /// <summary>
    /// 查询服务
    /// </summary>
    public class QueryService  // : ServiceBase
    {
        protected IRepositoryContext IRepository = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QueryService()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iRepository"></param>
        public QueryService(IUnitOfWork iRepository)
        {
            IRepository = iRepository as IRepositoryContext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iRepository"></param>
        public QueryService(IRepositoryContext iRepository)
        {
            IRepository = iRepository;
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetSYSTime()
        {
            return this.IRepository.GetSYSTime();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <param name="columns"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        public List<T> Get<T>(string where = null, object[] parameters = null, object[] columns = null, bool Cache = false) where T : DTOBase, new()
        {
            T Entity = new T();

            if (columns != null)
                Entity.Select(columns);

            if (!string.IsNullOrWhiteSpace(where))
                Entity.Where(where, parameters);

            var sql = CreateSql.Build((DTOBase)Entity);

            List<T> list = null;
            list = Cache ? Level2CacheRegister.Get<T>(sql, Entity.QueryParams, GetList<T>)
                         : GetList<T>(sql, Entity.QueryParams);
            Entity.ResetQuery();
            return list;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        public List<T> Get<T>(T Entity, bool Cache = false) where T : DTOBase, new()
        {
            var sql = CreateSql.Build((DTOBase)Entity);
            List<T> list = null;
            if (Cache)
            {
                list = Level2CacheRegister.Get<T>(sql, Entity.QueryParams, GetList<T>);
            }
            else
            {
                list = GetList<T>(sql, Entity.QueryParams);
            }

            Entity.ResetQuery();
            return list;
        }

        public PagedResult<T> GetPagedResult<T>(T entity, int pageIndex = 1, int pageSize = 20, string sort = "") where T : DTOBase, new()
        {
            var sql = CreateSql.Build((DTOBase)entity);

            var list = this.IRepository.PagedQuery<T>(sql, entity.QueryParams, pageIndex, pageSize, sort);

            list.PageData.ForEach(o =>
            {
                o.SetState(DTOState.UnChange);
            });

            entity.ResetQuery();
            return list as PagedResult<T>;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameties"></param>
        /// <returns></returns>
        internal List<T> GetList<T>(string sql, Dictionary<string, object> parameties = null) where T : DTOBase, new()
        {
            var list = this.IRepository.SqlQuery<T>(sql, parameties).ToList();

            list.ForEach(o =>
            {
                o.SetState(DTOState.UnChange);
            });

            // 查询的数据超过一定的数量记录到日志中，防止由于查询的数据量过大也导致服务器内存暴涨
            if (list.Count > 5000)
            {
                StringBuilder sBuilder = new StringBuilder();
                sBuilder.Append("SQL语句：");
                sBuilder.AppendLine(sql);

                if (parameties != null)
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("参数列表：");

                    parameties.ToList().ForEach(o =>
                    {
                        sBuilder.AppendLine(o.Key + "=" + o.Value);
                    });
                }

                LogHelper.Intance.Info("查询数据量过大", list.Count.ToString(), sBuilder.ToString());
            }

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
                if (value is string s)
                {
                    return Enum.Parse(targetType, s);
                }

                return Enum.ToObject(targetType, value);
            }
            if (!targetType.IsInterface && targetType.IsGenericType)
            {
                Type innerType = targetType.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(targetType, new object[] { innerValue });
            }
            if (value is string o && targetType == typeof(Guid))
            {
                return new Guid(o);
            }
            if (value is string o2 && targetType == typeof(Version))
            {
                return new Version(o2);
            }
            if (!(value is IConvertible))
            {
                return value;
            }
            return Convert.ChangeType(value, targetType);
        }

        /// <summary>
        /// 查询datatable
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        public DataTable Get(Parm<string> Sql, bool Cache = false)
        {
            if (Cache)
            {
                var table = Level2CacheRegister.Get(Sql.Entity, GetTable);
                return table;
            }
            else
            {
                var table = GetTable(Sql.Entity);
                return table;
            }
        }

        /// <summary>
        /// 查询datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private DataTable GetTable(string sql)
        {
            return this.IRepository.GetDataTable(sql);
        }

        public PagedTableResult GetPagedTable(string sql, Dictionary<string, object> parameters, int pageIndex = 1, int pageSize = 20, string sort = "")
        {
            return this.IRepository.GetPagedDataTable(sql, parameters, pageIndex, pageSize, sort = "") as PagedTableResult;
        }
    }
}
