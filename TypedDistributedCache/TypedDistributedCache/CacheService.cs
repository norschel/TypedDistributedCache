using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TypedDistributedCache.ContractResolvers;

namespace TypedDistributedCache
{
    /// <summary>
    /// This service is an abstraction, another layer on top of the IDistributedCache
    /// 
    /// Contains wrapper methods for easier, typed operations
    /// </summary>
    internal class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly TimeSpan DefaultTTL = TimeSpan.FromHours(1);

        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new PrivateSetterContractResolver()
        };

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> Get<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            return await DeserializeJson<T>(data);
        }

        public async Task<string> Get(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }

        public async Task<bool> IsSet(string key)
        {
            var data = await Get(key);
            return data != null;
        }

        public async Task Set(string key, string value, TimeSpan? ttl = null)
        {
            await _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? DefaultTTL
            });
        }

        public async Task Set<T>(string key, T value, TimeSpan? ttl = null)
        {
            var serialized = await SerializeToJson(value);
            await Set(key, serialized, ttl);
        }

        public async Task Unset(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        private async Task<string> SerializeToJson(object o)
        {
            var json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(o, _serializerSettings));
            return json;
        }

        public async Task<T> DeserializeJson<T>(string json)
        {
            if (json == null)
                return default(T);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(json, typeof(T));
            }

            var obj = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json, _serializerSettings));
            return obj;
        }
    }
}
