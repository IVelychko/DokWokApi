using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByUserIdQuery, IEnumerable<OrderModel>>
{
    public async Task<IEnumerable<OrderModel>> Handle(GetAllOrdersByUserIdQuery request, CancellationToken cancellationToken) =>
        await orderService.GetAllByUserIdAsync(request.UserId);
}
