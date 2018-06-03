using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypedDistributedCache.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Configuration_Memory()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCacheInstance = serviceProvider.GetService<ICacheService>();
            memoryCacheInstance.Should().NotBeNull();
        }

        [TestMethod]
        public void Configuration_Redis()
        {
            var services = new ServiceCollection();

            services.AddRedisCache("test");

            var serviceProvider = services.BuildServiceProvider();

            var redisCacheService = serviceProvider.GetService<ICacheService>();
            redisCacheService.Should().NotBeNull();
        }
    }
}
