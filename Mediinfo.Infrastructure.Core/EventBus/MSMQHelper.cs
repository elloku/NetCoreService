using System;

namespace Mediinfo.Infrastructure.Core.EventBus
{
    public class MSMQHelper : MQHelper, IMQHelper
    {
        public MSMQHelper(MQConnect connect)
        {

        }

        public override bool SendMessage(MQMessage message, MQConnParms parms)
        {
            throw new NotImplementedException();
        }
    }
}