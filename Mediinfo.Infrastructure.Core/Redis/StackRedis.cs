using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Mediinfo.Enterprise.Config;
using Mediinfo.Utility.Util;
using StackExchange.Redis;

namespace Mediinfo.Infrastructure.Core.Redis
{
    public class StackRedis
    {

        private static object _locker = new Object();
        private static ConnectionMultiplexer _instance = null;

        /// <summary>
        /// 使用一个静态属性来返回已连接的实例
        /// </summary>
        private static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null || !_instance.IsConnected)
                {
                    lock (_locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = ConnectionMultiplexer.Connect(MediinfoConfig.GetValue("RedisConfig.xml", "constr"));
                            //注册如下事件
                            _instance.ConnectionFailed += MuxerConnectionFailed;
                            _instance.ConnectionRestored += MuxerConnectionRestored;
                            _instance.ErrorMessage += MuxerErrorMessage;
                            _instance.ConfigurationChanged += MuxerConfigurationChanged;
                            _instance.HashSlotMoved += MuxerHashSlotMoved;
                            _instance.InternalError += MuxerInternalError;

                        }
                    }
                }

                return _instance;
            }
        }

        static StackRedis()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IDatabase GetDatabase()
        {
            return Instance.GetDatabase();
        }


        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            return Deserialize<T>(GetDatabase().StringGet(key));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        public static bool Set(string key, object value, int expireMinutes = 0)
        {
            if (expireMinutes > 0)
            {
                return GetDatabase().StringSet(key, Serialize(value), TimeSpan.FromMinutes(expireMinutes), When.Always);
            }
            else
            {
                return GetDatabase().StringSet(key, Serialize(value));
            }
        }

        public static string Info()
        {
            return (string)GetDatabase().Execute("INFO");
        }

        public static int DbSize()
        {
            return (int)GetDatabase().Execute("DBSIZE");
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return GetDatabase().KeyExists(key); //可直接调用
        }

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            return GetDatabase().KeyDelete(key);
        }



        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Increment(string key)
        {
            return GetDatabase().StringIncrement(key, flags: CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Decrement(string key, string value)
        {
            return GetDatabase().HashDecrement(key, value, flags: CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string Serialize(object o)
        {
            return JsonUtil.SerializeObject(o);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string json) where T : class
        {
            return JsonUtil.DeserializeToObject<T>(json);
        }

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            //Mediinfo.Enterprise.Log.LogHelper.Intance.Info("Redis", "Redis配置更改", "Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            try
            {


                Mediinfo.Enterprise.Log.LogHelper.Intance.Error("Redis", "Redis发生错误", "MuxerErrorMessage:" + e.EndPoint + ">" + e.Message);
            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            try
            {
                Mediinfo.Enterprise.Log.LogHelper.Intance.Error("Redis", "重新建立连接发生错误", "MuxerConnectionRestored: " + e.EndPoint + ">" + e.Exception?.ToString());
            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            try
            {
                Mediinfo.Enterprise.Log.LogHelper.Intance.Error("Redis", "连接失败", "重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception?.Message));
            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            try
            {
                Mediinfo.Enterprise.Log.LogHelper.Intance.Info("Redis", "更改集群", "HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);

            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            try
            {
                Mediinfo.Enterprise.Log.LogHelper.Intance.Error("Redis", "内部错误", "InternalError:Message" + e.Exception.Message);
            }
            catch (Exception)
            {

                //throw;
            }
        }
    }
}
