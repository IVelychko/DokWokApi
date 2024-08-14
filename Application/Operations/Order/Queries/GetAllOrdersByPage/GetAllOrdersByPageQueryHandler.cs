using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Order.Queries.GetAllOrdersByPage;

public sealed class GetAllOrdersByPageQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByPageQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByPageQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderService.GetAllByPageAsync(request.PageNumber, request.PageSize);
        return orders.Select(o => o.ToResponse());
    }
}
