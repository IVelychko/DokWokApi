using Domain.Abstractions.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

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
        string? serializedValue = await _distributedCache.GetStringAsync(key);
        return serializedValue is not null ? JsonConvert.DeserializeObject<TValue>(serializedValue) : null;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
        CacheKeys.TryRemove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        IEnumerable<Task> tasks = CacheKeys.Keys
            .Where(x => x.StartsWith(prefixKey))
            .Select(RemoveAsync);

        await Task.WhenAll(tasks);
    }

    public async Task SetAsync<TValue>(string key, TValue value, JsonSerializerSettings? jsonSerializerSettings = null) where TValue : class
    {
        string serializedValue;
        DistributedCacheEntryOptions cacheOptions = new();
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        if (jsonSerializerSettings is not null)
        {
            serializedValue = JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }
        else
        {
            serializedValue = JsonConvert.SerializeObject(value);
        }

        await _distributedCache.SetStringAsync(key, serializedValue, cacheOptions);
        CacheKeys.TryAdd(key, false);
    }
}
