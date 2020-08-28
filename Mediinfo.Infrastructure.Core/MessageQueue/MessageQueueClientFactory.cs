namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    /// <summary>
    /// 消息队列客户端工厂
    /// </summary>
    public class MessageQueueClientFactory
    {
        /// <summary>
        /// 创建客户端对象
        /// </summary>
        /// <returns></returns>
        public static IMessageClient CreateClient()
        {
            IMessageClient messageQueueClient = new RabitMQChannel("/");
            return messageQueueClient;
        }

        /// <summary>
        /// 创建DB客户端
        /// </summary>
        /// <returns></returns>
        public static IDbMessageClient CreateDbClient()
        {
            IDbMessageClient messageQueueClient = new RabitMQChannel("/db");
            return messageQueueClient;
        }

        /// <summary>
        /// 创建用户客户端
        /// </summary>
        /// <returns></returns>
        public static IUserMessageClient CreateUserClient()
        {
            IUserMessageClient messageQueueClient = new RabitMQChannel("/user");
            return messageQueueClient;
        }
    }
}
