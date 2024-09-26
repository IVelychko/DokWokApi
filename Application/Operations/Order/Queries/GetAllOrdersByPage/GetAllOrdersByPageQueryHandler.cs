using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrdersByPage;

public sealed class GetAllOrdersByPageQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByPageQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var orders = await orderService.GetAllAsync(pageInfo);
        return orders.Select(o => o.ToResponse());
    }
}
