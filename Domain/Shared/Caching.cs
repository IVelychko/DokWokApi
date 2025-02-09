using Domain.Abstractions.Services;
using Domain.Entities;
using Newtonsoft.Json;

namespace Domain.Shared;

public static class Caching
{
    private static readonly JsonSerializerSettings _settings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

    public static async Task<IList<TEntity>> GetCollectionFromCache<TEntity>(ICacheService cacheService,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IList<TEntity>>> fetch)
        where TEntity : BaseEntity
    {
        IList<TEntity>? entities = await cacheService.GetAsync<IList<TEntity>>(key);
        if (entities is null)
        {
            entities = await fetch(specification);
            if (entities.Any())
            {
                await cacheService.SetAsync(key, entities, _settings);
                return entities;
            }

            return entities;
        }

        return entities;
    }

    public static async Task<TEntity?> GetEntityFromCache<TEntity>(ICacheService cacheService,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IList<TEntity>>> fetch)
        where TEntity : BaseEntity
    {
        TEntity? entity = await cacheService.GetAsync<TEntity>(key);
        if (entity is null)
        {
            var fetchedResult = await fetch(specification);
            entity = fetchedResult.FirstOrDefault();
            if (entity is not null)
            {
                await cacheService.SetAsync(key, entity, _settings);
                return entity;
            }

            return entity;
        }

        return entity;
    }
}
