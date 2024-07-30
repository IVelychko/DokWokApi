using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

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
