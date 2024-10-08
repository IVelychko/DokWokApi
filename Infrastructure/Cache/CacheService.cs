using Domain.Abstractions.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Infrastructure.Cache;

public class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> _cacheKeys = [];
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<TValue?> GetAsync<TValue>(string key) where TValue : class
    {
        var serializedValue = await _distributedCache.GetStringAsync(key);
        return serializedValue is not null ? JsonConvert.DeserializeObject<TValue>(serializedValue) : null;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
        _cacheKeys.TryRemove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        IEnumerable<Task> tasks = _cacheKeys.Keys
            .Where(x => x.StartsWith(prefixKey))
            .Select(RemoveAsync);

        await Task.WhenAll(tasks);
    }

    public async Task SetAsync<TValue>(string key, TValue value, JsonSerializerSettings? jsonSerializerSettings = null) where TValue : class
    {
        string serializedValue;
        if (jsonSerializerSettings is not null)
        {
            serializedValue = JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }
        else
        {
            serializedValue = JsonConvert.SerializeObject(value);
        }

        await _distributedCache.SetStringAsync(key, serializedValue);
        _cacheKeys.TryAdd(key, false);
    }
}
