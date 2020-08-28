using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Mediinfo.DTO.Core
{
    /// <summary>
    /// DataTable工具类
    /// </summary>
    public class DataTableHelper
    {
        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable FillDataTable<T>(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    if(dt.Columns.Contains(propertyInfo.Name))
                    {
                        dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);

                    }
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateData<T>(T model) 
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.Name != "IDs" && propertyInfo.Name != "DefaultSQL" && propertyInfo.Name != "State") 
                {
                    dataTable.Columns.Add(new DataColumn(propertyInfo.Name));
                }
            }
            return dataTable;
        }
    }
}
