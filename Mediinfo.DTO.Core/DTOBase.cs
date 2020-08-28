using Mediinfo.Enterprise.Exceptions;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;


namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// DTO对象基类
    /// </summary>
    [Serializable]
    public abstract class DTOBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 默认Sql语句
        /// </summary>
        //[NotMapped]
        private string _DefaultSQL { get; set; }

        /// <summary>
        /// 获取默认sql
        /// </summary>
        /// <returns></returns>
        public string GetDefaultSQL()
        {
            if (_DefaultSQL == null)
            {
                var type = this.GetType();
                var atts = type.GetCustomAttributes(true);
                if (atts.Any())
                {
                    var obj = atts.FirstOrDefault(o => o is DefaultSQLAttribute);
                    if (obj != null)
                    {
                        var att = (DefaultSQLAttribute)obj;
                        return att.SQL;
                    }
                }
            }
            return _DefaultSQL;
        }

        /// <summary>
        /// 设置默认sql
        /// </summary>
        /// <param name="defaultSQL"></param>
        public void SetDefaultSQL(string defaultSQL)
        {
            _DefaultSQL = defaultSQL;
        }

        /// <summary>
        /// 自定义列开关
        /// </summary>
        private bool _EnableSelectColumn = false;
        /// <summary>
        /// 启用自定义查询列
        /// </summary>
        public void EnableSelectColumn()
        {
            _EnableSelectColumn = true;
        }

        /// <summary>
        /// 禁用自定义查询列
        /// </summary>
        public void DisableSelectColumn()
        {
            _EnableSelectColumn = false;
        }

        /// <summary>
        /// 获取当前自定义查询列的状态
        /// </summary>
        /// <returns></returns>
        public bool GetSelectColumnStatus()
        {
            return _EnableSelectColumn;
        }

        /// <summary>
        /// DTO对象属性的原始值，属性名区分大小写
        /// </summary>
        [JsonProperty]
        public Dictionary<string, object> OriginalValues;

        /// <summary>
        /// DTO状态
        /// </summary>
        [JsonProperty]
        private DTOState _State;
        /// <summary>
        /// 获取DTO状态
        /// </summary>
        /// <returns></returns>
        public DTOState GetState()
        {
            return _State;
        }

        /// <summary>
        /// 设置DTO状态
        /// </summary>
        /// <param name="state"></param>
        public void SetState(DTOState state)
        {
            _State = state;

            if (_State == DTOState.UnChange || _State == DTOState.New)
                OriginalValues.Clear();
        }

       [JsonProperty]
        private bool _traceChange;
        /// <summary>
        /// 获取自定跟踪的状态
        /// </summary>
        /// <returns></returns>
        public bool GetTraceChange()
        {
            return _traceChange;
        }

        /// <summary>
        /// 设置DTO变化自动跟踪
        /// </summary>
        /// <param name="traceChange"></param>
        public void SetTraceChange(bool traceChange)
        {
            if (traceChange == _traceChange)
                return;

            _traceChange = traceChange;
            OriginalValues.Clear();
        }

    
        #region 查询相关 ,每次查询完成后需要清除

        /// <summary>
        /// 用户输入的SQL，多为查询条件
        /// </summary>
        protected internal string QuerySql = string.Empty;

        /// <summary>
        /// 用户自定义Select的列
        /// </summary>
        internal List<string> SelectedColumns;

        /// <summary>
        /// Sql查询参数
        /// </summary>
        public Dictionary<string, object> QueryParams;

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        protected DTOBase()
        {
            // 初始化查询条件
            SelectedColumns = new List<string>();
            QueryParams = new Dictionary<string, object>();
            QuerySql = string.Empty;

            // 原始值
            OriginalValues = new Dictionary<string, object>();

            // 行状态
            SetState(DTOState.New);

            // 默认不跟踪状态
            _traceChange = false;

        }

        /// <summary>
        /// 初始化查询状态
        /// </summary>
        public void ResetQuery()
        {
            QuerySql = string.Empty;
            SelectedColumns.Clear();
            QueryParams.Clear();
        }

        #region 变化跟踪

        /// <summary>
        /// 属性变化通知事件
        /// 这个报错，加下面属性这个博客上有讲，先做个标记吧（https://blog.darkthread.net/blog/field-nonserializable/）
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性发生变化（执行变更记录）
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="propertyName"></param>
        protected void Changing(object oldValue, String propertyName = null)
        {
            if (this.GetState() == DTOState.UnChange && _traceChange)
            {
                this.SetState(DTOState.Update);
            }

            // 记录变化列的原始值
            if (GetTraceChange() && !OriginalValues.ContainsKey(propertyName == null ? "" : propertyName) && !string.IsNullOrWhiteSpace(propertyName == null ? "" : propertyName))
                OriginalValues.Add(propertyName, oldValue);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region DTO查询实现部分

        /// <summary>
        /// 设置需要查询的列
        /// </summary>
        /// <param name="propertyName"></param
        protected void Select(string propertyName = null)
        {
            if (_EnableSelectColumn && SelectedColumns.All(o => o != propertyName))
            {
                SelectedColumns.Add(propertyName);
            }
        }

        /// <summary>
        /// 设置需要查询的列
        /// </summary>
        public void Select(params object[] Columns)
        {
            List<string> columns = new List<string>();
            if (Columns.Length > 0)
            {
                for (int i = 0; i < Columns.Length; i++)
                {
                    columns.Add(SelectedColumns[Columns.Length - i - 1]);
                }
            }
            SelectedColumns = columns;
        }

        /// <summary>
        /// 根据列名设置需要查询的列
        /// </summary>
        public void SelectByColumnName(params string[] Columns)
        {
            foreach (var item in Columns)
            {
                if (_EnableSelectColumn && SelectedColumns.All(o => o != item))
                {
                    SelectedColumns.Add(item);
                }
            }
        }

        /// <summary>
        /// 附加查询条件
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="Parameters"></param>
        public void WhereAppend(string sql, params object[] Parameters)
        {
            QuerySql += sql;
            var items = sql.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> PNames = new List<string>();
            items.ToList().ForEach(o =>
            {
                if (o.IndexOf(":", StringComparison.Ordinal) >= 0)
                {
                    PNames.Add(o.Substring(o.IndexOf(":", StringComparison.Ordinal) + 1));
                }
            });
            if (Parameters != null)
            {
                int index = 0;
                Parameters.ToList().ForEach(o =>
                {
                    if (PNames.Count > index)
                    {
                        if (!QueryParams.ContainsKey(PNames[index]))
                            QueryParams.Add(PNames[index], o);
                    }
                    index++;
                });
            }
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void Where(string sql, params object[] parameters)
        {
            QuerySql = sql;
            var items = sql.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> PNames = new List<string>();
            items.ToList().ForEach(o =>
            {
                if (o.IndexOf(":", StringComparison.Ordinal) >= 0)
                {
                    PNames.Add(o.Substring(o.IndexOf(":", StringComparison.Ordinal) + 1));
                }
            });

            if (parameters != null)
            {
                int index = 0;

                parameters.ToList().ForEach(o =>
                {
                    if (PNames.Count > index)
                    {
                        if (!QueryParams.ContainsKey(PNames[index]))
                            QueryParams.Add(PNames[index], o);
                    }
                    index++;
                });
            }
        }

        /// <summary>
        /// 添加查询的参数
        /// </summary>
        /// <param name="Name">参数名称</param>
        /// <param name="Value">参数值</param>
        public void AddParameter(string Name, object Value)
        {
            QueryParams.Add(Name, Value);
        }

        #endregion

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            try
            {
                var deserializeSettings = new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this), this.GetType(), deserializeSettings);
            }
            catch (Exception)
            {
                BinaryFormatter BF2 = new BinaryFormatter();
                using MemoryStream stream = new MemoryStream();
                BF2.Serialize(stream, this);
                stream.Position = 0;
                return BF2.Deserialize(stream);
            }
        }

        public T CloneJson<T>()
        {
            if (ReferenceEquals(this, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings
            { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this), deserializeSettings);
        }

      /// <summary>
        /// dto字段赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ColumnName"></param>
        /// <param name="Value"></param>
        public void Set<T>(string ColumnName, T Value)
        {
            var same = false;
            this.GetType().GetProperties().ToList().ForEach(o =>
            {
                if (o.Name == ColumnName)
                {
                    same = true;
                    o.SetValue(this, Value, null);
                    return;
                }
            });
            if (!same)
            {
                throw new DTOException(ColumnName + "字段未找到");
            }
        }
        
        /// <summary>
        /// 复制DTO属性到另一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MapTo<T>() where T : DTOBase, new()
        {
            T t = new T();
            var propers = t.GetType().GetProperties().ToList();
            this.GetType().GetProperties().ToList().ForEach(o =>
            {
                var pro = propers.FirstOrDefault(p => p.Name == o.Name);
                if (pro != null && o.PropertyType == pro.PropertyType)
                {
                    var value = o.GetValue(this, null);
                    if (value != null)
                    {
                        pro.SetValue(t, value, null);
                    }
                }
            });
            //---把状态也复制过去---
            t.SetState(this.GetState());
            t.SetTraceChange(this.GetTraceChange());

            return t;
        }

        /// <summary>
        /// 将当前对象转换成特定的对象
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <returns>目标对象</returns>
        public T ConvertObject<T>()
        {
            // 创建实体对象实例
            var t = Activator.CreateInstance<T>();
            Type type = this.GetType();
            // 遍历实体对象属性
            foreach (var info in typeof(T).GetProperties())
            {
                // 取得object对象中此属性的值
                var val = type.GetProperty(info.Name)?.GetValue(this, null);
                if (val != null)
                {
                    // 非泛型
                    object obj = null;
                    if (!info.PropertyType.IsGenericType)
                        obj = Convert.ChangeType(val, info.PropertyType);
                    else // 泛型Nullable<>
                    {
                        Type genericTypeDefinition = info.PropertyType.GetGenericTypeDefinition();
                        obj = genericTypeDefinition == typeof(Nullable<>)
                            ? Convert.ChangeType(val, Nullable.GetUnderlyingType(info.PropertyType)
                                                      ?? throw new InvalidOperationException())
                            : Convert.ChangeType(val, info.PropertyType);
                    }

                    info.SetValue(t, obj, null);
                }
            }

            return t;
        }
    }

    /// <summary>
    /// DTO扩展方法
    /// </summary>
    public static partial class DTOExtesion
    {
        /// <summary>
        /// 获取变化的DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> GetChanges<T>(this List<T> entitys) where T : DTOBase
        {
            return entitys.Where(o => o.GetState() != DTOState.UnChange && o.GetState() != DTOState.Cancel).ToList();
        }

        /// <summary>
        /// 获取新增状态的DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> GetNews<T>(this List<T> entitys) where T : DTOBase
        {
            return entitys.Where(o => o.GetState() == DTOState.New).ToList();
        }

        /// <summary>
        /// 获取更新状态的DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> GetUpdates<T>(this List<T> entitys) where T : DTOBase
        {
            return entitys.Where(o => o.GetState() == DTOState.Update).ToList();
        }

        /// <summary>
        /// 获取删除状态的DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static List<T> GetDeletes<T>(this List<T> entitys) where T : DTOBase
        {
            return entitys.Where(o => o.GetState() == DTOState.Delete).ToList();
        }

        /// <summary>
        /// 设置为删除状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static void Delete<T>(this T entity) where T : DTOBase
        {
            entity.SetState(entity.GetState() == DTOState.New ? DTOState.Cancel : DTOState.Delete);
        }
    }
}
