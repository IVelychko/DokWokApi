using Domain.Abstractions.Services;
using Domain.Entities;
using Newtonsoft.Json;

namespace Domain.Helpers;

public static class Caching
{
    private static readonly JsonSerializerSettings _settings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

    public async static Task<IEnumerable<TEntity>> GetCollectionFromCache<TEntity>(ICacheService cacheService,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IEnumerable<TEntity>>> fetch)
        where TEntity : BaseEntity
    {
        IEnumerable<TEntity>? entities = await cacheService.GetAsync<IEnumerable<TEntity>>(key);
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

    public async static Task<TEntity?> GetEntityFromCache<TEntity>(ICacheService cacheService,
        string key, Specification<TEntity> specification, Func<Specification<TEntity>, Task<IEnumerable<TEntity>>> fetch)
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
