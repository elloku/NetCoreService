using Mediinfo.Enterprise.Config;
using Mediinfo.Enterprise.Log;

using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    /// <summary>
    /// RabitMQ 通道
    /// </summary>
    public class RabitMQChannel : IMessageClient, IDbMessageClient, IUserMessageClient
    {
        private IModel channel = null;
        private IConnection connection = null;

        private static string uname = MediinfoConfig.GetValue("RabbitMQConfig.xml", "username");
        private static string pwd = MediinfoConfig.GetValue("RabbitMQConfig.xml", "password");
        private static string hosts = MediinfoConfig.GetValue("RabbitMQConfig.xml", "hostnames");

        public RabitMQChannel(string vHost)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hosts) || string.IsNullOrWhiteSpace(uname) || string.IsNullOrWhiteSpace(pwd))
                {
                    LogHelper.Intance.Warn("消息队列", "消息队列未配置", "消息队列未配置,username:" + uname + ",password:" + pwd);
                    return;
                }

                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = uname,
                    Password = pwd,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    VirtualHost = vHost
                };
                
                connection = factory.CreateConnection(hosts.Split(','));
                
                channel = connection.CreateModel();

                channel.ExchangeDeclare("HIS_EXCHANGE", ExchangeType.Headers, true, false, null);
                channel.ExchangeDeclare("HIS_USERMSG_EXCHANGE", ExchangeType.Direct, true, false, null);
                
            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "消息初始化失败",
                    "异常原因: " + ex.ToString());
            }
        }

        /// <summary>
        /// 发送用户消息
        /// </summary>
        /// <param name="receivers"></param>
        /// <param name="XiaoXiNR"></param>
        /// <returns></returns>
        public bool PublishUserMsg(IEnumerable<string> receivers, object XiaoXiNR)
        {
            if(receivers == null)
            {
                LogHelper.Intance.Error("消息队列", "User消息发送主体不能为空",
                    "消息内容:" + XiaoXiNR?.ToString());
                return false;
            }

            try
            {
                if (channel == null) return false;

                foreach (var receiver in receivers)
                {
                    // 设置消息发送超时机制
                    var policy = Policy
                          .Handle<Exception>()
                          .WaitAndRetry(new[]
                          {
                        TimeSpan.FromSeconds(3),
                          }, (ex, time) =>
                          {
                              LogHelper.Intance.Error("超时预警", "User消息发送超时",
                    "发送列表:" + string.Join(",", receivers) + "，消息内容: " + XiaoXiNR?.ToString() + "，异常原因: " + ex.ToString());
                          });

                    // 执行发送消息
                    policy.Execute(() =>
                    {
                        channel.BasicPublish("HIS_USERMSG_EXCHANGE", receiver, null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(XiaoXiNR)));
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "User消息发送失败",
                    "发送列表:" + string.Join(",", receivers) + "，消息内容: " + XiaoXiNR?.ToString()+ "，异常原因: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="moKuaiMc"></param>
        /// <param name="yeWuMc"></param>
        /// <param name="caoZuoMc"></param>
        /// <param name="messageQueue"></param>
        public void Publish(string moKuaiMc, string yeWuMc, string caoZuoMc, Messager messageQueue)
        {
            try
            {
                if (channel == null) return;

                Dictionary<string, object> aHeader = new Dictionary<string, object>();
                aHeader.Add("moKuaiMc", moKuaiMc);
                aHeader.Add("yeWuMc", yeWuMc);
                aHeader.Add("caoZuoMc", caoZuoMc);
                foreach (var item in messageQueue.EntityNameList)
                {
                    if (string.IsNullOrEmpty(item.Trim()))
                        continue;
                    aHeader.Add(item.Trim(), "ENTITYNAME");
                }

                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;
                props.Expiration = "36000000";
                props.Headers = aHeader;
                messageQueue.FaSongSJ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 设置消息发送超时机制
                var policy = Policy
                      .Handle<Exception>()
                      .WaitAndRetry(new[]
                      {
                        TimeSpan.FromSeconds(3),
                      }, (ex, time) =>
                      {
                          Enterprise.Log.LogHelper.Intance.Warn("超时预警", "服务:"+ moKuaiMc + "，发送DB消息超时",
                              "服务:" + moKuaiMc + "，controller："+yeWuMc+"，action："+caoZuoMc+"，发送DB消息超时" + ex.ToString() + "");
                      });

                // 执行发送消息
                policy.Execute(() =>
                {
                    channel.BasicPublish("HIS_EXCHANGE", string.Empty, props, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageQueue)));
                });

            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "DB消息发送失败",
                    "服务:" + moKuaiMc + "，controller：" + yeWuMc + "，action：" + caoZuoMc + "，异常原因: " + ex.ToString());
            }
        }

        /// <summary>
        /// 绑定队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="moKuaiMc"></param>
        /// <param name="yeWuMc"></param>
        /// <param name="caoZuoMc"></param>
        public void BindQueue(string queueName, params string[] entityNames)
        {
            Dictionary<string, object> aHeader = new Dictionary<string, object>();
            foreach (var item in entityNames)
            {
                if (string.IsNullOrEmpty(item.Trim()))
                    continue;
                aHeader.Add(item.Trim(), "ENTITYNAME");
            }
            aHeader.Add("x-match", "any");
            aHeader.Add("x-ha-policy", "all");

            channel.QueueBind(queueName.Trim(), "HIS_EXCHANGE", string.Empty, aHeader);
        }

        /// <summary>
        /// 新增队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        public void CreateQueue(string queueName, bool durable, bool exclusive, bool autoDelete, params string[] entityNames)
        {
            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            Dictionary<string, object> aHeader = new Dictionary<string, object>();

            foreach (var item in entityNames)
            {
                if (string.IsNullOrEmpty(item.Trim()))
                    continue;
                aHeader.Add(item.Trim(), "ENTITYNAME");
            }

            aHeader.Add("x-match", "any");
            aHeader.Add("x-ha-policy", "all");

            channel.QueueBind(queueName.Trim(), "HIS_EXCHANGE", string.Empty, aHeader);
        }

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queueName"></param>
        public void DeleteQueue(string queueName)
        {
            channel.QueueDelete(queueName.Trim());
        }

        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        public void UnbindQueue(string queueName, params string[] entityNames)
        {
            Dictionary<string, object> aHeader = new Dictionary<string, object>();
            foreach (var item in entityNames)
            {
                if (string.IsNullOrEmpty(item.Trim()))
                    continue;
                aHeader.Add(item.Trim(), "ENTITYNAME");
            }
            aHeader.Add("x-match", "any");
            aHeader.Add("x-ha-policy", "all");

            channel.QueueUnbind(queueName.Trim(), "HIS_EXCHANGE", string.Empty, aHeader);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="eventArgs"></param>
        public void HandlerMessage(string queueName, EventHandler<BasicDeliverEventArgs> eventArgs)
        {
            try
            {
                if (channel == null) return;

                // 在处理完消息的数量
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += eventArgs;
                String consumerTag = channel.BasicConsume(queueName.Trim(), true, consumer);
            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "消息队列："+ queueName + "，HandlerMessage失败",
                    "异常原因: " + ex.ToString());
            }
        }

        /// <summary>
        /// 确认接收消息
        /// </summary>
        /// <param name="deliveryTag"></param>
        /// <param name="multiple"></param>
        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            channel.BasicAck(deliveryTag, false);
        }

        /// <summary>
        /// 自定义消息处理函数示例
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="ea"></param>
        private void MessageEventHandler(object ch, BasicDeliverEventArgs ea)
        {
            Messager messageBody = JsonConvert.DeserializeObject<Messager>(Encoding.UTF8.GetString(ea.Body));
        }

        /// <summary>
        /// 获取队列
        /// </summary>
        /// <returns></returns>
        public List<QueueEntity.QueueModel> GetQueueList()
        {

            var uname = MediinfoConfig.GetValue("RabbitMQConfig.xml", "username");
            var pwd = MediinfoConfig.GetValue("RabbitMQConfig.xml", "password");
            var hosts = MediinfoConfig.GetValue("RabbitMQConfig.xml", "hostnames");

            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(uname, pwd);
                string queue = webClient.DownloadString("http://" + hosts.Split(',')[0] + ":15672/api/queues");
                List<QueueEntity.QueueModel> list =
                    JsonConvert.DeserializeObject<List<QueueEntity.QueueModel>>(queue);
                return list;
            }
        }

        /// <summary>
        /// 获取路由列表
        /// </summary>
        /// <returns></returns>
        public List<ExchangeEntity.ExchangeModel> GetExchangeList()
        {
            var uname = MediinfoConfig.GetValue("RabbitMQConfig.xml", "username");
            var pwd = MediinfoConfig.GetValue("RabbitMQConfig.xml", "password");
            var hosts = MediinfoConfig.GetValue("RabbitMQConfig.xml", "hostnames");

            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(uname, pwd);
                string queue = webClient.DownloadString("http://" + hosts.Split(',')[0] + ":15672/api/exchanges");
                List<ExchangeEntity.ExchangeModel> list =
                    JsonConvert.DeserializeObject<List<ExchangeEntity.ExchangeModel>>(queue);

                return list;
            }
        }

        /// <summary>
        /// 获取绑定列表
        /// </summary>
        /// <returns></returns>
        public List<BindingsEntity.BindingsModel> GetBindingsList()
        {
            var uname = MediinfoConfig.GetValue("RabbitMQConfig.xml", "username");
            var pwd = MediinfoConfig.GetValue("RabbitMQConfig.xml", "password");
            var hosts = MediinfoConfig.GetValue("RabbitMQConfig.xml", "hostnames");

            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(uname, pwd);
                string queue = webClient.DownloadString("http://" + hosts.Split(',')[0] + ":15672/api/bindings");
                List<BindingsEntity.BindingsModel> list =
                    JsonConvert.DeserializeObject<List<BindingsEntity.BindingsModel>>(queue);

                return list;
            }
        }

        /// <summary>
        /// 释放channel
        /// </summary>
        public void Dispose()
        {
            // 释放通道
            channel.Dispose();
            if (!channel.IsClosed)
                channel.Close();
            // 释放连接
            connection.Dispose();
            if (connection.IsOpen)
                connection.Close();
        }
    }
}
