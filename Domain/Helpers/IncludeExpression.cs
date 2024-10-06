using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Helpers;

public class IncludeExpression<TEntity> where TEntity : BaseEntity
{
    public IncludeExpression(Expression<Func<TEntity, object?>> include)
    {
        Include = include;
    }

    public IncludeExpression(Expression<Func<TEntity, object?>> include, List<Expression<Func<object?, object?>>> thenIncludes)
        : this(include)
    {
        ThenIncludes = thenIncludes;
    }

    public Expression<Func<TEntity, object?>> Include { get; init; }

    public List<Expression<Func<object?, object?>>> ThenIncludes { get; init; } = [];
}
