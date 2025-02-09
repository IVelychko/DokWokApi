using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByUserIdQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderService.GetAllByUserIdAsync(request.UserId);
        return orders.Select(o => o.ToResponse());
    }
}
