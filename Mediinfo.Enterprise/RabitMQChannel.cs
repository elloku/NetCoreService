using Mediinfo.Enterprise.Config;
using Mediinfo.Enterprise.Log;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;

namespace Mediinfo.Enterprise
{
    /// <summary>
    /// RabitMQ 通道
    /// </summary>
    public class RabitMQChannel : IDisposable
    {
        private IModel channel = null;
        private IConnection connection = null;

        private RabitMQChannel(string vHost)
        {
            try
            {
                var uname = MediinfoConfig.GetValue("RabbitMQConfig.xml", "username");
                var pwd = MediinfoConfig.GetValue("RabbitMQConfig.xml", "password");
                var hosts = MediinfoConfig.GetValue("RabbitMQConfig.xml", "hostnames");

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
                    HostName = hosts.Split(',')[0],
                    VirtualHost = vHost
                };

                connection = factory.CreateConnection();

                channel = connection.CreateModel();

                channel.ExchangeDeclare("HIS_USERMSG_EXCHANGE", ExchangeType.Direct, true, false, null);
            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "消息初始化失败",
                    "异常原因: " + ex.ToString());
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static RabitMQChannel Channel
        {
            get
            {
                return new RabitMQChannel("/");
            }
        }

        public static RabitMQChannel UserChannel
        {
            get
            {
                return new RabitMQChannel("/user");
            }
        }

        public static RabitMQChannel DbChannel
        {
            get
            {
                return new RabitMQChannel("/db");
            }
        }

        /// <summary>
        /// 新增队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        public void CreateQueue(string queueName, bool durable, bool exclusive, bool autoDelete, string routingKey)
        {
            Dictionary<string, object> aHeader = new Dictionary<string, object>();
            aHeader.Add("x-ha-policy", "all");

            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, aHeader);

            channel.QueueBind(queueName.Trim(), "HIS_USERMSG_EXCHANGE", routingKey);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="eventArgs"></param>
        public EventingBasicConsumer HandlerMessage(string queueName)
        {
            try
            {
                if (channel == null) return null;

                CreateQueue("USERMSG-" + queueName.Trim(), true, true, true, queueName);

                // 在处理完消息的数量
                channel.BasicQos(0, 5, false);

                var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += eventArgs;
                String consumerTag = channel.BasicConsume("USERMSG-" + queueName.Trim(), true, consumer);
                return consumer;
            }
            catch (Exception ex)
            {
                LogHelper.Intance.Error("消息队列", "消息HandlerMessage失败",
                    "异常原因: " + ex.ToString());
                return null;
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
        /// 断开连接
        /// </summary>
        public void Close()
        {
            channel.Close();
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
