using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    public interface IMessageClient:IDisposable
    {
        /// <summary>
        /// 绑定队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="moKuaiMc"></param>
        /// <param name="yeWuMc"></param>
        /// <param name="caoZuoMc"></param>
        void BindQueue(string queueName, params string[] entityNames);

        /// <summary>
        /// 新增队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        void CreateQueue(string queueName, bool durable, bool exclusive, bool autoDelete, params string[] entityNames);

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queueName"></param>
        void DeleteQueue(string queueName);

        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        void UnbindQueue(string queueName, params string[] entityNames);

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="eventArgs"></param>
        void HandlerMessage(string queueName, EventHandler<BasicDeliverEventArgs> eventArgs);

        /// <summary>
        /// 确认接收消息
        /// </summary>
        /// <param name="deliveryTag"></param>
        /// <param name="multiple"></param>
        void BasicAck(ulong deliveryTag, bool multiple);

        /// <summary>
        /// 获取队列
        /// </summary>
        /// <returns></returns>
        List<QueueEntity.QueueModel> GetQueueList();

        List<ExchangeEntity.ExchangeModel> GetExchangeList();

        List<BindingsEntity.BindingsModel> GetBindingsList();
    }
}
