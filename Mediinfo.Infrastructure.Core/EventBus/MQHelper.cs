namespace Mediinfo.Infrastructure.Core.EventBus
{

    public interface IMQHelper
    {
        bool SendMessage(MQMessage message, MQConnParms parms);
    }

    public abstract class MQHelper : IMQHelper
    {
        public abstract bool SendMessage(MQMessage message, MQConnParms parms);

        public static IMQHelper Create(MQConnect connect)
        {
            // 根据配置切换MQ
            RabbitMQHelper Helper = new RabbitMQHelper(connect);
            return Helper;
        } 
    }
}
