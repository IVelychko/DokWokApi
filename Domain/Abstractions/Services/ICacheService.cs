namespace Domain.Abstractions.Services;

public interface ICacheService
{
    public Task<TValue?> GetAsync<TValue>(string key) where TValue : class;

    public Task SetAsync<TValue>(string key, TValue value) where TValue : class;

    public Task RemoveAsync(string key);

    public Task RemoveByPrefixAsync(string prefixKey);
}
