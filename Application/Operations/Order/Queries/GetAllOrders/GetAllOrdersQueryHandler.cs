using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersQuery, IEnumerable<OrderModel>>
{
    public async Task<IEnumerable<OrderModel>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken) =>
        await orderService.GetAllAsync();
}
