using Domain.Entities;
using Domain.Specifications.Orders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specifications.Orders;

public static class OrderSpecificationEvaluator
{
    public static IQueryable<Order> ApplySpecification(
        IQueryable<Order> initialQuery, OrderSpecification specification)
    {
        var query = initialQuery;
        query = Include(query, specification);
        query = ApplyId(query, specification.Id);
        query = ApplyUserId(query, specification.UserId);
        return query;
    }

    private static IQueryable<Order> ApplyId(IQueryable<Order> query, long? id)
    {
        return id.HasValue ? query.Where(o => o.Id == id.Value) : query;
    }
    
    private static IQueryable<Order> ApplyUserId(IQueryable<Order> query, long? userId)
    {
        return userId.HasValue ? query.Where(o => o.UserId == userId.Value) : query;
    }
    
    private static IQueryable<Order> Include(IQueryable<Order> query, OrderSpecification specification)
    {
        if (specification.IncludeOrderLines)
        {
            var orderLineIncludableQueryable = query.Include(o => o.OrderLines);
            if (specification.IncludeProduct)
            {
                var productIncludableQueryable = orderLineIncludableQueryable.ThenInclude(ol => ol.Product);
                if (specification.IncludeCategory)
                {
                    return productIncludableQueryable.ThenInclude(p => p!.Category);
                }

                return productIncludableQueryable;
            }

            return orderLineIncludableQueryable;
        }

        return query;
    }
}