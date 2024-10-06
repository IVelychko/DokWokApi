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

        foreach (var includeExpression in specification.IncludeExpressions)
        {
            var includableQueryable = queryable.Include(includeExpression.Include);
            foreach (var thenIncludeExpression in includeExpression.ThenIncludes)
            {
                includableQueryable = includableQueryable.ThenInclude(thenIncludeExpression);
            }
            queryable = includableQueryable;
        }

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
            queryable = ApplyPagination(queryable, specification.PageInfo);
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
