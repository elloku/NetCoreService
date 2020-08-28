using System.Collections.Generic;

namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    public interface IUserMessageClient : IMessageClient
    {
        bool PublishUserMsg(IEnumerable<string> receivers, object XiaoXiNR);
    }
}
