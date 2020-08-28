namespace Mediinfo.Infrastructure.Core.EventBus
{
    public class MQConnParms
    {
        public string MessageType { get; set; }
        public string MQExchange { get; set; }
        public string MQRoutingKey { get; set; }
    }
}
