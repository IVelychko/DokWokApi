using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await orderService.GetAllAsync();
    }
}
