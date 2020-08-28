using System.Collections.Generic;
using System.Data;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// JsonDataTable用于与服务端的数据发送
    /// </summary>
    public class JsonDataTable
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public JsonDataTable()
        {

        }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="dataTable"></param>
        public JsonDataTable(DataTable dataTable)
        {
            if (dataTable != null && dataTable.Rows.Count <= 0)
            {
                foreach (DataColumn item in dataTable.Columns)
                {
                    ColumnList.Add(item.ColumnName);
                }
            }
            _dataTable = dataTable;
        }

        /// <summary>
        /// 定义列
        /// </summary>
        public List<string> ColumnList { get; set; } = new List<string>();
        private DataTable _dataTable;

        /// <summary>
        /// 获取Datatable内容
        /// </summary>
        public DataTable TableContent
        {
            get
            {
                if (_dataTable != null && _dataTable.Rows.Count <= 0 && _dataTable.Columns.Count == 0)
                {
                    foreach (var item in ColumnList)
                    {
                        _dataTable.Columns.Add(item);
                    }
                }
                return _dataTable;
            }
            set
            {
                _dataTable = value;
            }
        }
    }
}
