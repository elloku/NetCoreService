using System;

namespace Mediinfo.Infrastructure.Core.EventBus
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs()
        {
            ID = Guid.NewGuid();
            Time = DateTime.Now;
        }

        public Guid ID { get; set; }
        public DateTime Time { get; set; }
        public string IP { get; set; }
        public string UserName { get; set; }
        public string YingYongID { get; set; }
        public string XiTongID { get; set; }
    }
}
