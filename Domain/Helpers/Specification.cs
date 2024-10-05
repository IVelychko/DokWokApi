using Domain.Entities;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Helpers;

public class Specification<TEntity> where TEntity : BaseEntity
{
    public Expression<Func<TEntity, bool>>? Criteria { get; init; }

    public List<Expression<Func<TEntity, object?>>> IncludeExpressions { get; } = [];

    public Expression<Func<TEntity, object>>? OrderByExpression { get; init; }

    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; init; }

    public PageInfo? PageInfo { get; init; }
}
