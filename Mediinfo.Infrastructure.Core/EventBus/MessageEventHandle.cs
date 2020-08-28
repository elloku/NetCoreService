namespace Mediinfo.Infrastructure.Core.EventBus
{
    public abstract class MessageEventHandle<T> where T : MessageEventArgs, new()
    {
        public abstract void Handle(T MessageArgs);
    }
}
