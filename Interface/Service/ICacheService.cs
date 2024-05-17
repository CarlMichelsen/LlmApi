using Microsoft.Extensions.Caching.Distributed;

namespace Interface.Service;

public interface ICacheService
{
    Task<string?> Get(string key);

    Task Set(string key, string value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null);

    Task<T?> GetJson<T>(string key);

    Task SetJson<T>(string key, T value, DistributedCacheEntryOptions? distributedCacheEntryOptions = null);

    Task Remove(string key);
}