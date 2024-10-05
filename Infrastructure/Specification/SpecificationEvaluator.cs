using Domain.Entities;
using Domain.Helpers;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> ApplySpecification<TEntity>(IQueryable<TEntity> initialQuery, Specification<TEntity> specification)
        where TEntity : BaseEntity
    {
        IQueryable<TEntity> queryable = initialQuery;
        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        queryable = specification.IncludeExpressions.Aggregate(queryable,
            (current, includeExpression) => current.Include(includeExpression));

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        }

        if (specification.PageInfo is not null)
        {
            var pageInfo = specification.PageInfo;
            var itemsToSkip = (pageInfo.Number - 1) * pageInfo.Size;
            queryable = queryable.Skip(itemsToSkip).Take(pageInfo.Size);
        }

        return queryable;
    }

    public static IQueryable<TEntity> ApplyPagination<TEntity>(IQueryable<TEntity> initialQuery, PageInfo pageInfo)
        where TEntity : BaseEntity
    {
        IQueryable<TEntity> queryable = initialQuery;
        var itemsToSkip = (pageInfo.Number - 1) * pageInfo.Size;
        queryable = queryable.Skip(itemsToSkip).Take(pageInfo.Size);
        return queryable;
    }
}
