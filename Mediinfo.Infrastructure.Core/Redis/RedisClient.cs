namespace Mediinfo.Infrastructure.Core.Redis
{
    public class RedisClient : IRedisClient
    {
        private static readonly IRedisClient _Clinent = new RedisClient();

        private RedisClient()
        {

        }

        public static IRedisClient Clinent
        {
            get
            {
                return _Clinent;
            }
        }

        public long Decrement(string key, string value)
        {
            return StackRedis.Decrement(key,value);
        }

        public bool Exists(string key)
        {
            return StackRedis.Exists(key);
        }

        public T Get<T>(string key) where T : class
        {
            return StackRedis.Get<T>(key);
        }

        public long Increment(string key)
        {
            return StackRedis.Increment(key);
        }

        public bool Remove(string key)
        {
            return StackRedis.Remove(key);
        }

        public bool Set(string key, object value, int expireMinutes = 0)
        {
            return StackRedis.Set(key, value, expireMinutes);
        }

        public string Info()
        {
            return StackRedis.Info();
        }
    }
}
