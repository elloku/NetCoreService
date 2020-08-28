using Newtonsoft.Json;
using System;

namespace Mediinfo.Utility.Util
{
    /// <summary>
    /// Json工具类
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// 对象转Json
        /// </summary>
        public static string SerializeObject(object o,bool deleteNull = false)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                //NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            if (deleteNull) jsonSetting.NullValueHandling = NullValueHandling.Ignore;
            
            return JsonConvert.SerializeObject(o, jsonSetting);
        }

        public static string SerializeObject(object o, JsonSerializerSettings jsonSetting)
        {
            return JsonConvert.SerializeObject(o, jsonSetting);
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// 也可以直接用 JsonConvert.DeserializeObject<T>(json)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>对象实体</returns>
        public static T DeserializeToObject<T>(string json) where T : class
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                //NullValueHandling = NullValueHandling.Ignore,
                //DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            if(string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(json, jsonSetting);
        }

        public static object DeserializeToObject(string json, Type type)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                //NullValueHandling = NullValueHandling.Ignore,
                //DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject(json, type, jsonSetting);
        }

        public static object DeserializeToObject(string json, Type type, JsonConverter converter)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                //NullValueHandling = NullValueHandling.Ignore,
                //DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            jsonSetting.Converters.Add(converter);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject(json, type, jsonSetting);
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }
    }
}
