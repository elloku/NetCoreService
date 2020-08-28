using Mediinfo.Utility;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.Config
{
    /// <summary>
    /// RabbitMQ配置
    /// </summary>
    public class RabbitMQConfig
    {
        private static readonly RabbitMQConfig inst = new RabbitMQConfig();
        private List<string> hostnames = new List<string>();
        private string username;
        private string password;

        private RabbitMQConfig()
        {
            string json = IOHelper.Read(AppDomain.CurrentDomain.BaseDirectory + "RabbitMQConfig.json");
            JObject jObject = JsonConvert.DeserializeObject(json) as JObject;
            var namesObjs = jObject["hostnames"].Values();
            foreach (var item in namesObjs)
            {
                hostnames.Add(item.ToString());
            }

            username = jObject["username"].ToString();
            password = jObject["password"].ToString();
        }

        public static RabbitMQConfig Instance
        {
            get
            {
                return inst;
            }
        }

        public List<string> GetHostnames()
        {
            return hostnames;
        }

        public string GetUsername()
        {
            return username;
        }

        public string GetPassword()
        {
            return password;
        }
    }
}
