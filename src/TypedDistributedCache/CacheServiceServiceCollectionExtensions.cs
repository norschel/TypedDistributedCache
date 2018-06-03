using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace TypedDistributedCache
{
    public static class CacheServiceServiceCollectionExtensions
    {
        /// <summary>
        /// Extension method for IServiceCollection. Registers ICacheService with an in memory cache provider.
        /// </summary>
        /// <param name="services">The IServiceCollection in which you want to register the ICacheService configuration</param>
        /// <returns>The updated IServiceCollection</returns>
        public static IServiceCollection AddMemoryCache(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IDistributedCache, MemoryDistributedCache>());

            RegisterCacheService(services);

            return services;
        }

        /// <summary>
        /// Extension method for IServiceCollection. Registers ICacheService with a Distributed Redis connection
        /// </summary>
        /// <param name="services">The IServiceCollection in which you want to register the ICacheService configuration</param>
        /// <param name="connectionString">Redis connection string</param>
        /// <returns>The updated IServiceCollection</returns>
        public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = connectionString;
            });

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
