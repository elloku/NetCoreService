using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediinfo.APIGateway.Core.Services
{
    /// <summary>
    /// consul KV
    /// </summary>
    public class ConsulKVManager:IDisposable
    {
        private ConsulClient _consul;
        private static readonly ConsulKVManager _instance = new ConsulKVManager();

        /// <summary>
        /// 单例
        /// </summary>
        public static ConsulKVManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        private ConsulKVManager(string host = "127.0.0.1", int port = 8500)
        {
            try
            {
                _consul = new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{host}:{port}");
                });
            }
            catch (Exception ex)
            {
                _consul.Dispose();
                _consul = null;

                _consul = new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{host}:{port}");
                });
            }
        }
        
        /// <summary>
        /// 字典是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> IsExits(string path, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            bool result = false;
            var kv = await _consul.KV.Get(path,ct);
            if (kv != null)
            {
                if (kv.Response != null)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 列出字典列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> ListValue(string path, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var kv = await _consul.KV.List(path, ct);
            if (kv != null)
            {
                if (kv.Response != null)
                {

                    KVPair[] kVPairs = kv.Response;
                    foreach (var item in kVPairs)
                    {
                        string value = (item.Value!=null&&item.Value.Any()) ? System.Text.Encoding.UTF8.GetString(item.Value) : "";
                        result.Add(item.Key, value);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<string> GetValue(string path, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            var kv = await _consul.KV.Get(path, ct);
            if (kv != null)
            {
                if (kv.Response != null)
                {
                    
                    KVPair kVPair = kv.Response;
                    if(kVPair==null || kVPair.Value == null)
                    {
                        return string.Empty;
                    }
                    return System.Text.Encoding.UTF8.GetString(kVPair.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 设置字典值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> SetValue(string path,string value, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            KVPair kVPair = new KVPair(path);
            kVPair.Value = System.Text.Encoding.UTF8.GetBytes(value);

            var kv = await _consul.KV.Put(kVPair, ct);
            if (kv != null)
            {
                return kv.Response;
            }
            return false;
        }

        /// <summary>
        /// 删除路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> DeletePath(string path, System.Threading.CancellationToken ct = default(System.Threading.CancellationToken))
        {
            var kv = await _consul.KV.Delete(path, ct);
            if (kv != null)
            {
                return kv.Response;
            }
            return false;
        }

        /// <summary>
        /// 析构
        /// </summary>
        public void Dispose()
        {
            _consul.Dispose();
        }
    }
}
