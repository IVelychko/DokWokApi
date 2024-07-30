using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Order.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderService.GetAllAsync();
        return orders.Select(o => o.ToResponse());
    }
}
