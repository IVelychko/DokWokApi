using Domain.Entities;
using Domain.Specifications.OrderLines;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specifications.OrderLines;

public static class OrderLineSpecificationEvaluator
{
    public static IQueryable<OrderLine> ApplySpecification(
        IQueryable<OrderLine> initialQuery, OrderLineSpecification specification)
    {
        var query = initialQuery;
        query = Include(query, specification);
        query = ApplyOrderId(query, specification.OrderId);
        return query;
    }
    
    private static IQueryable<OrderLine> Include(IQueryable<OrderLine> query, OrderLineSpecification specification)
    {
        if (specification.IncludeProduct)
        {
            var productIncludableQueryable = query.Include(ol => ol.Product);
            if (specification.IncludeCategory)
            {
                return productIncludableQueryable.ThenInclude(p => p!.Category);
            }

            return productIncludableQueryable;
        }

        return query;
    }
    
    private static IQueryable<OrderLine> ApplyOrderId(IQueryable<OrderLine> query, long? orderId)
    {
        return orderId.HasValue ?
            query.Where(ol => ol.OrderId == orderId.Value) :
            query;
    }
}