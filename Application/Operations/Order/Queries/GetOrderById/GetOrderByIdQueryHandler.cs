using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderService orderService)
    : IQueryHandler<GetOrderByIdQuery, OrderModel?>
{
    public async Task<OrderModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken) =>
        await orderService.GetByIdAsync(request.Id);
}
