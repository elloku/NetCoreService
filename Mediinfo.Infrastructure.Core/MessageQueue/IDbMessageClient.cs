namespace Mediinfo.Infrastructure.Core.MessageQueue
{
    public interface IDbMessageClient : IMessageClient
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="moKuaiMc"></param>
        /// <param name="yeWuMc"></param>
        /// <param name="caoZuoMc"></param>
        /// <param name="messageQueue"></param>
        void Publish(string moKuaiMc, string yeWuMc, string caoZuoMc, Messager messageQueue);
    }
}
