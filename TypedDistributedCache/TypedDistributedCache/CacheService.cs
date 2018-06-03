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

        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new PrivateSetterContractResolver()
        };

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Gets a value from the cache for a given key.
        /// </summary>
        /// <typeparam name="T">Expected type of value in cache</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Value for given key in cache</returns>
        public async Task<T> Get<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            return await DeserializeJson<T>(data);
        }

        /// <summary>
        /// Gets a string from the cache for a given key.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Value for given key in cache</returns>
        public async Task<string> Get(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }

        /// <summary>
        /// Checks if a given key is present in the cache.
        /// </summary>
        /// <param name="key">Key to evaluate</param>
        /// <returns>Returns true if the key is present in the cache. Otherwise, false.</returns>
        public async Task<bool> IsSet(string key)
        {
            var data = await Get(key);
            return data != null;
        }

        /// <summary>
        /// Stores a string value for a given key in the cache.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="value">String value to be stored</param>
        /// <param name="ttl">Time to live for the cache key. Optional. Will default to "Never expire".</param>
        /// <returns></returns>
        public async Task Set(string key, string value, TimeSpan? ttl = null)
        {
            await _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });
        }

        /// <summary>
        /// Stores a value of type T for a given key in the cache.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Actual value to be stored</param>
        /// <param name="ttl">Time to live for the cache key. Optional. Will default to "Never expire".</param>
        /// <returns></returns>
        public async Task Set<T>(string key, T value, TimeSpan? ttl = null)
        {
            var serialized = await SerializeToJson(value);
            await Set(key, serialized, ttl);
        }

        /// <summary>
        /// Removes a key from the cache.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>A Task.</returns>
        public async Task Unset(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        private async Task<string> SerializeToJson(object o)
        {
            var json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(o, _serializerSettings));
            return json;
        }

        private async Task<T> DeserializeJson<T>(string json)
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
