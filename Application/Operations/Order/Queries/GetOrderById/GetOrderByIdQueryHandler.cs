using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderService orderService)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse?>
{
    public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderService.GetByIdAsync(request.Id);
        return order?.ToResponse();
    }
}
