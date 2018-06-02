using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace TypedDistributedCache.Tests
{
    [TestClass]
    public class InMemoryTests
    {
        private readonly CacheService _cacheService;

        public InMemoryTests()
        {
            var inMemoryCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            _cacheService = new CacheService(inMemoryCache);
        }

        [TestMethod]
        public async Task Memory_GetSet_String()
        {
            var key = nameof(Memory_GetSet_String);
            var expected = "test";

            await _cacheService.Set(key, expected);
            var get = await _cacheService.Get(key);

            get.Should().Be(expected);
        }

        [TestMethod]
        public async Task Memory_GetSet_Int()
        {
            var key = nameof(Memory_GetSet_Int);
            var expected = 123456789;

            await _cacheService.Set(key, expected);
            var get = await _cacheService.Get<int>(key);

            get.Should().Be(expected);
        }

        [TestMethod]
        public async Task Memory_GetSet_Null()
        {
            var key = nameof(Memory_GetSet_Null);
            object expected = null;

            await _cacheService.Set(key, expected);
            var get = await _cacheService.Get<object>(key);

            get.Should().Be(expected);
        }

        [TestMethod]
        public async Task Memory_GetSet_Object()
        {
            var key = nameof(Memory_GetSet_Object);
            TestObject expected = TestObjectFactory.Create();

            await _cacheService.Set(key, expected);
            var get = await _cacheService.Get<TestObject>(key);

            get.Should().NotBeNull();
            get.Name.Should().Be(expected.Name);
        }
    }
}
