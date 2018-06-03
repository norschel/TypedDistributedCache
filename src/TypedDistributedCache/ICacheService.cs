﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("TypedDistributedCache.Tests")]

namespace TypedDistributedCache
{
    internal interface ICacheService
    {
        Task<T> Get<T>(string key);
        Task<string> Get(string key);

        Task Set(string key, string value, TimeSpan? ttl = null);
        Task Set<T>(string key, T value, TimeSpan? ttl = null);

        Task Unset(string key);

        Task<bool> IsSet(string key);
    }
}
