namespace Mediinfo.Infrastructure.Core.EventBus
{
    public interface IMQMessageEventArgs
    {
        MessageChannel Channel { get; set; }
        MessageTarget Target { get; set; }
        string HospitalID { get; set; }
        string JiuZhenKH { get; set; } 
        object Messages { get; set; }
    }

    public enum MessageTarget
    {
        Add,
        Modify,
        Remove,
    }

    public enum MessageChannel
    {
        MenZhen,
        ZhuYuan,
        JiZhen,
        QiTa,
    }
}
