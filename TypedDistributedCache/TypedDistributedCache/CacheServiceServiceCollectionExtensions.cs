using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace TypedDistributedCache
{
    public static class CacheServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddMemoryCache(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAdd(ServiceDescriptor.Singleton<IDistributedCache, MemoryDistributedCache>());

            RegisterCacheService(services);

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.TryAdd(ServiceDescriptor.Singleton<IDistributedCache, RedisCache>());

            RegisterCacheService(services);

            return services;
        }

        private static void RegisterCacheService(IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Transient<ICacheService, CacheService>());
        }
    }

    public enum CacheType
    {
        Memory = 0,
        Redis = 1
    }
}
