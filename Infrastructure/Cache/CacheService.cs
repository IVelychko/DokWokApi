using Domain.Abstractions.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Infrastructure.Cache;

public class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = [];
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<TValue?> GetAsync<TValue>(string key) where TValue : class
    {
        var serializedValue = await _distributedCache.GetStringAsync(key);
        // return serializedValue is not null ? JsonConvert.DeserializeObject<TValue>(serializedValue) : null;
        return serializedValue is not null ? JsonSerializer.Deserialize<TValue>(serializedValue) : null;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
        CacheKeys.TryRemove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        var tasks = CacheKeys.Keys
            .Where(x => x.StartsWith(prefixKey))
            .Select(RemoveAsync);

        await Task.WhenAll(tasks);
    }

    public async Task SetAsync<TValue>(string key, TValue value) where TValue : class
    {
        DistributedCacheEntryOptions cacheOptions = new();
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        // var serializedValue = JsonConvert.SerializeObject(value);
        var serializedValue = JsonSerializer.Serialize(value);

        await _distributedCache.SetStringAsync(key, serializedValue, cacheOptions);
        CacheKeys.TryAdd(key, false);
    }
}
