using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplyPagination<TEntity>(this IQueryable<TEntity> initialQuery, PageInfo pageInfo)
        where TEntity : BaseEntity
    {
        IQueryable<TEntity> paginatedQuery = initialQuery;
        int itemsToSkip = (pageInfo.Number - 1) * pageInfo.Size;
        paginatedQuery = paginatedQuery
            .OrderBy(x => x.Id)
            .Skip(itemsToSkip)
            .Take(pageInfo.Size);

        return paginatedQuery;
    } 
}