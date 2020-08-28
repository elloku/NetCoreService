using Mediinfo.Enterprise.Config;
using Mediinfo.Utility.Extensions;
using Mediinfo.Utility.Util;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediinfo.Enterprise.Log
{
    /// <summary>
    /// 基于ES的日志
    /// </summary>
    public class ESLog
    {
        private string address = MediinfoConfig.GetValue("ElasticsearchConfig.xml", "address");//地址
        private string username = MediinfoConfig.GetValue("ElasticsearchConfig.xml", "username");//用户名
        private string pwd = MediinfoConfig.GetValue("ElasticsearchConfig.xml", "password");//密码

        public ESLog()
        {
        }

        /// <summary>
        /// 提交自定义日志
        /// </summary>
        /// <param name="logEntity"></param>
        /// <param name="id">若id为空，则ES会自动生成一个唯一的ID</param>
        public void PutLog(LogEntity logEntity, string id = "")
        {
            if (string.IsNullOrEmpty(address)) return;
            RestClient.HttpRequestNoResult("http://" + address + "/his_cus_" + logEntity.SuoYin.ToLower() + "/_doc/" + id, HttpType.POST, JsonConvert.SerializeObject(logEntity),username, pwd);
        }

        /// <summary>
        /// 提交系统日志
        /// </summary>
        /// <param name="logEntity"></param>
        public void PutLog(SysLogEntity logEntity)
        {
            if (string.IsNullOrEmpty(address)) return;
            RestClient.HttpRequestNoResult("http://" + address + "/his_sys/_doc/", HttpType.POST, JsonConvert.SerializeObject(logEntity), username, pwd);
        }
        /// <summary>
        /// 全文检索，多字段 并关系
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="searchDict">查询文档</param>
        /// <param name="pageIndex">起始页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public SysLogEntityList SearchFullFiledss(DateTime startDate, DateTime endDate, Dictionary<string, string> searchDict, int pageIndex, int pageSize, string order = "desc")
        {
            bool isHaoShi = false;
            if (searchDict.Count > 0)
            {
                foreach (var item in searchDict)
                {
                    if (item.Key == "RiZhiLx" && item.Value == "6")
                    {
                        isHaoShi = true;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(address)) return null;
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("\"query\": {");
            json.AppendLine("\"bool\": {");
            json.AppendLine("\"must\": [");
            json.AppendLine("{");
            json.AppendLine("\"bool\":{");
            json.AppendLine("\"should\":{\"range\": { \"ChuangJianSj\" : { \"gt\" : \"" + startDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\",\"lt\":\"" + endDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\" }}}");
            json.AppendLine("}");
            json.AppendLine("}");

            foreach (var item in searchDict)
            {
                json.AppendLine(",{\"bool\":{\"should\":{\"match_phrase\": { \"" + item.Key + "\":\"" + item.Value + "\"}}}}");
            }

            json.AppendLine("]");
            json.AppendLine("}},");

            json.AppendLine("\"from\": " + pageIndex * pageSize + ",");
            json.AppendLine("\"size\": " + pageSize + ",");

            //json.AppendLine("\"sort\": [{	\"ChuangJianSj\": {\"order\": \"" + order + "\", \"unmapped_type\": \"string\" }}]}");

            if (isHaoShi)
            {
                json.AppendLine("\"sort\": [{\"FuWuHs\": {\"order\": \"" + order + "\", \"missing\": \"_last\" }},{\"ChuangJianSj\":{\"order\":\"" + order + "\",\"unmapped_type\":\"string\"}}]}");
            }
            else
            {
                json.AppendLine("\"sort\": [{	\"ChuangJianSj\": {\"order\": \"" + order + "\", \"unmapped_type\": \"string\" }}]}");
            }


            string result = RestClient.HttpRequest("http://" + address + "/his_sys/_doc/_search", HttpType.POST, json.ToString(),username, pwd);

            SysRootLogEntity list = JsonUtil.DeserializeToObject<SysRootLogEntity>(result);
            SysLogEntityList datalist = new SysLogEntityList();
            if (list != null && list.hits != null)
            {
                datalist.Hits = list.hits.total.value;
                datalist.Took = list.took;
                var sList = list.hits.hits.Select(c => c._source);
                if (isHaoShi)
                {
                    var l = sList.OrderByDescending(c => c.FuWuHs);
                    datalist.AddRange(l);
                }
                else
                {
                    datalist.AddRange(sList);
                }
            }
            return datalist;
        }

        /// <summary>
        /// 搜索异常日志
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="searchDict"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public SysLogEntityList SearchYiChang(DateTime startDate, DateTime endDate, Dictionary<string, string> searchDict, int haoshics,int pageIndex, int pageSize, string order = "desc")
        {
            if (string.IsNullOrEmpty(address)) return null;
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("\"query\": {");
            json.AppendLine("\"bool\": {");
            json.AppendLine("\"must\": [");
            json.AppendLine("{");
            //json.AppendLine("\"should\":{\"range\": { \"ChuangJianSj\" : { \"gt\" : \"" + startDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\",\"lt\":\"" + endDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\" }}}");
            json.AppendLine("\"range\": { \"ChuangJianSj\" : { \"gt\" : \"" + startDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\",\"lt\":\"" + endDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\" }}}");
            json.AppendLine(",{\"range\":{\"FuWuHs\":{\"gte\":\""+ haoshics + "\"}}}");

            //json.AppendLine("}");
            //json.AppendLine("}");

            foreach (var item in searchDict)
            {
                json.AppendLine(",{\"bool\":{\"should\":{\"match_phrase\": { \"" + item.Key + "\":\"" + item.Value + "\"}}}}");
            }

            json.AppendLine("]");
            json.AppendLine("}},");

            json.AppendLine("\"from\": " + pageIndex * pageSize + ",");
            json.AppendLine("\"size\": " + pageSize + ",");

            json.AppendLine("\"sort\": [{	\"ChuangJianSj\": {\"order\": \"" + order + "\", \"unmapped_type\": \"string\" }}]}");

            string result = RestClient.HttpRequest("http://" + address + "/his_sys/_doc/_search", HttpType.POST, json.ToString(), username, pwd);

            SysRootLogEntity list = JsonUtil.DeserializeToObject<SysRootLogEntity>(result);
            SysLogEntityList datalist = new SysLogEntityList();
            if (list != null && list.hits != null)
            {
                datalist.Hits = list.hits.total.value;
                datalist.Took = list.took;
                var sList = list.hits.hits.Select(c => c._source);
                var l = sList.OrderByDescending(c => c.FuWuHs);
                datalist.AddRange(l);
            }
            return datalist;
        }

        /// <summary>
        /// 查询自定义日志
        /// </summary>
        /// <param name="suoyin">索引</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="searchDict">查询文档</param>
        /// <param name="pageIndex">起始页</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public LogEntityList SearchZdyFullFiledss(string suoyin, DateTime startDate, DateTime endDate, Dictionary<string, string> searchDict, int pageIndex, int pageSize)
        {
            if (string.IsNullOrEmpty(address)) return null;
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("\"query\": {");
            json.AppendLine("\"bool\": {");
            json.AppendLine("\"must\": [");
            json.AppendLine("{");
            json.AppendLine("\"bool\":{");
            json.AppendLine("\"should\":{\"range\": { \"ChuangJianSj\" : { \"gt\" : \"" + startDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\",\"lt\":\"" + endDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\" }}}");
            json.AppendLine("}");
            json.AppendLine("}");

            foreach (var item in searchDict)
            {
                json.AppendLine(",{\"bool\":{\"should\":{\"match_phrase\": { \"" + item.Key + "\":\"" + item.Value + "\"}}}}");
            }

            json.AppendLine("]");
            json.AppendLine("}},");

            json.AppendLine("\"from\": " + pageIndex * pageSize + ",");
            json.AppendLine("\"size\": " + pageSize + ",");

            json.AppendLine("\"sort\": [{	\"ChuangJianSj\": {\"order\": \"desc\"}}]}");

            string result = RestClient.HttpRequest("http://" + address + "/" + suoyin + "/_doc/_search", HttpType.POST, json.ToString(), username, pwd);

            CustRootLogEntity list = JsonUtil.DeserializeToObject<CustRootLogEntity>(result);
            LogEntityList datalist = new LogEntityList();
            if (list != null && list.hits != null)
            {
                datalist.Hits = list.hits.total.value;
                datalist.Took = list.took;
                var sList = list.hits.hits.Select(c => c._source);
                datalist.AddRange(sList);
            }
            return datalist;
        }

        public SysLogEntityList SearchHaoShi(DateTime startDate, DateTime endDate, Dictionary<string, string> searchDict, int pageIndex, int pageSize, string order = "desc")
        {
            if (string.IsNullOrEmpty(address)) return null;
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("\"query\": {");
            json.AppendLine("\"bool\": {");
            json.AppendLine("\"must\": [");
            json.AppendLine("{");
            json.AppendLine("\"bool\":{");
            json.AppendLine("\"should\":{\"range\": { \"ChuangJianSj\" : { \"gt\" : \"" + startDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\",\"lt\":\"" + endDate.ToInvariantString("yyyy/MM/dd HH:mm:ss") + "\" }}}");
            json.AppendLine("}");
            json.AppendLine("}");

            foreach (var item in searchDict)
            {
                json.AppendLine(",{\"bool\":{\"should\":{\"match_phrase\": { \"" + item.Key + "\":\"" + item.Value + "\"}}}}");
            }

            json.AppendLine("]");
            json.AppendLine("}},");

            json.AppendLine("\"from\": " + pageIndex * pageSize + ",");
            json.AppendLine("\"size\": " + pageSize + ",");

            //json.AppendLine("\"sort\": [{	\"ChuangJianSj\": {\"order\": \"" + order + "\", \"unmapped_type\": \"string\" }}]}");
            json.AppendLine("\"sort\": [{\"FuWuHs\": {\"order\": \"" + order + "\", \"missing\": \"_last\" }},{\"ChuangJianSj\":{\"order\":\"" + order + "\",\"unmapped_type\":\"string\"}}]}");

            string result = RestClient.HttpRequest("http://" + address + "/his_sys/_doc/_search", HttpType.POST, json.ToString(), username, pwd);

            SysRootLogEntity list = JsonUtil.DeserializeToObject<SysRootLogEntity>(result);
            SysLogEntityList datalist = new SysLogEntityList();
            if (list != null && list.hits != null)
            {
                datalist.Hits = list.hits.total.value;
                datalist.Took = list.took;
                var sList = list.hits.hits.Select(c => c._source);
                var l = sList.OrderByDescending(c => c.FuWuHs);
                datalist.AddRange(l);
                //datalist.AddRange(sList);
            }
            return datalist;
        }

        /// <summary>
        /// 获取自定义索引列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetZdySyList()
        {
            List<string> list = new List<string>();
            string result = RestClient.HttpRequest("http://" + address + "/_cat/indices", HttpType.GET, "", username, pwd);
            foreach (var item in result.Split('\n'))
            {
                foreach (var sy in item.Split(' '))
                {
                    if (sy.StartsWith("his_cus_"))
                    {
                        list.Add(sy.Replace("his_cus_", ""));
                    }
                }
            }

            return list;
        }

        public string[] ReadAllIndex()
        {
            List<string> indexs = new List<string>();
            string result = RestClient.HttpRequest("http://" + address + "/_cat/indices", HttpType.GET, "", username, pwd);
            string[] re = result.Split(' ');
            return re;
        }
        /// <summary>
        /// 按照时间删除
        /// </summary>
        public void DeleteByTime(DateTime? kssj, DateTime? jssj)
        {
            string ks = kssj.ToInvariantString("yyyy/MM/dd HH:mm:ss");
            string js = jssj.ToInvariantString("yyyy/MM/dd HH:mm:ss");

            if (string.IsNullOrEmpty(address)) return;
            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("\"query\":{");
            json.AppendLine("\"range\":{");
            json.AppendLine("\"ChuangJianSj\":{\"from\":\"" + ks + "\",\"to\":\"" + js + "\"}}}}");

            string result = RestClient.HttpRequest("http://" + address + "/his_sys/_delete_by_query", HttpType.POST, json.ToString(), username, pwd);

        }
        public void DeleteSpecific(double beforedays)
        {
            DateTime dt = DateTime.Now;
            string beforday = dt.AddDays(-beforedays).ToString("yyyy-MM-dd").Replace("-", "");
            int beforeday = int.Parse(beforday);
            string tillDay = dt.AddDays(-beforedays * 3).ToString("yyyy-MM-dd").Replace("-", "");
            int tillDays = int.Parse(tillDay);
            for (int i = beforeday; i > tillDays; i--)
            {
                string indexName = "his_sys-" + i.ToString();
                string[] indexs = ReadAllIndex();
                if (indexs.Contains(indexName))
                {
                    RestClient.HttpRequestNoResult("http://" + address + "/" + indexName, HttpType.DELETE, "", username, pwd);
                }
            }

        }
    }
}
