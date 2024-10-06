using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Domain.Helpers;

public static class Caching
{
    private static readonly JsonSerializerSettings _settings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

    public async static Task<IEnumerable<TEntity>> GetCollectionFromCache<TEntity>(IDistributedCache distributedCache,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IEnumerable<TEntity>>> fetch)
        where TEntity : BaseEntity
    {
        IEnumerable<TEntity> entities;
        string? cachedSerializedEntities = await distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedSerializedEntities))
        {
            entities = await fetch(specification);
            if (entities.Any())
            {
                var serializedEntities = JsonConvert.SerializeObject(entities, _settings);
                await distributedCache.SetStringAsync(key, serializedEntities);
                return entities;
            }

            return entities;
        }

        entities = JsonConvert.DeserializeObject<IEnumerable<TEntity>>(cachedSerializedEntities)!;
        return entities;
    }

    public async static Task<TEntity?> GetEntityFromCache<TEntity>(IDistributedCache distributedCache,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IEnumerable<TEntity>>> fetch)
        where TEntity : BaseEntity
    {
        TEntity? entity;
        string? cachedSerializedEntity = await distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedSerializedEntity))
        {
            var fetchedResult = await fetch(specification);
            entity = fetchedResult.FirstOrDefault();
            if (entity is not null)
            {
                var serializedEntity = JsonConvert.SerializeObject(entity, _settings);
                await distributedCache.SetStringAsync(key, serializedEntity);
                return entity;
            }

            return entity;
        }

        entity = JsonConvert.DeserializeObject<TEntity>(cachedSerializedEntity);
        return entity;
    }
}
