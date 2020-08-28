using System.Collections.Generic;
using System.Data;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 适应于与服务端传递的Dataset
    /// </summary>
    public class JsonDataSet
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public JsonDataSet()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataSet"></param>
        public JsonDataSet(DataSet dataSet)
        {
            // 对DataSet里面的Table循环处理
            foreach (DataTable table in dataSet.Tables)
            {
                List<string> columnList = new List<string>();
                foreach (DataColumn item in table.Columns)
                {
                    columnList.Add(item.ColumnName);
                }
                TableList.Add(table.TableName, columnList);
            }
            _dataSet = dataSet;
        }

        /// <summary>
        /// 定义table列表
        /// </summary>
        public Dictionary<string, List<string>> TableList { get; set; } = new Dictionary<string, List<string>>();
        private DataSet _dataSet;

        /// <summary>
        /// 获取dataset内容
        /// </summary>
        public DataSet DataSetContent
        {
            get
            {
                if (_dataSet != null)
                {
                    foreach (DataTable table in _dataSet.Tables)
                    {
                        if (table != null && table.Rows.Count <= 0 && table.Columns.Count == 0)
                        {
                            List<string> columnList = TableList[table.TableName];
                            foreach (var item in columnList)
                            {
                                table.Columns.Add(item);
                            }
                        }
                    }
                }
                return _dataSet;
            }
            set
            {
                _dataSet = value;
            }
        }
    }
}
