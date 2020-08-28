using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mediinfo.Infrastructure.Core.EventBus
{
    public class RabbitMQHelper : MQHelper, IMQHelper
    {
        static List<string> ExchangeNames = new List<string>();
        static List<KeyValuePair<string, Func<object, object>>> Messages = new List<KeyValuePair<string, Func<object, object>>>();
        IConnection conn;

        static RabbitMQHelper()
        {
            typeof(MQMessage).GetProperties().ToList().ForEach(o =>
            {
                if (o.PropertyType.IsValueType || o.PropertyType == typeof(string))
                {
                    Messages.Add(new KeyValuePair<string, Func<object, object>>(o.Name, o.GetValue));
                }
            });
        }

        public RabbitMQHelper(MQConnect connect)
        {
            conn = new ConnectionFactory()
            {
                HostName = connect.HostName,
                Port = connect.Port,
                AutomaticRecoveryEnabled = true,
                UserName = connect.UserName,
                Password = connect.Password,
            }.CreateConnection(connect.ConnectionName);
        }

        public override bool SendMessage(MQMessage message, MQConnParms parms)
        {
            using (var channel = conn.CreateModel())
            {
                if (!ExchangeNames.Contains(parms.MQExchange))
                {
                    ExchangeNames.Add(parms.MQExchange);
                    channel.ExchangeDeclare(parms.MQExchange, "topic", true, false, null);
                    channel.BasicQos(0, 1, false);  // 不要在同一时间给一个工作者发送多于1个的消息
                }
                JsonSerializer serializer = new JsonSerializer();
                var sw = new StringWriter();
                serializer.Serialize(new JsonTextWriter(sw), message.Messages);

                IBasicProperties propertie;
                propertie = channel.CreateBasicProperties();
                propertie.Persistent = true;    // 消息需要落地
                propertie.Headers = new Dictionary<string, object>();
                Messages.ForEach(o =>
                {
                    propertie.Headers.Add(o.Key, o.Value(message));
                });

                channel.BasicPublish(parms.MQExchange, parms.MQRoutingKey, propertie, Encoding.UTF8.GetBytes(sw.GetStringBuilder().ToString()));
                channel.Close();
            }

            return true;
        }
    }
}
