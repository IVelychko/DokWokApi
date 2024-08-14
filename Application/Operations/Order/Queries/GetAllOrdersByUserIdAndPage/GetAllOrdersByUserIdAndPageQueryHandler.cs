using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserIdAndPage;

public sealed class GetAllOrdersByUserIdAndPageQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByUserIdAndPageQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByUserIdAndPageQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderService.GetAllByUserIdAndPageAsync(request.UserId, request.PageNumber, request.PageSize);
        return orders.Select(o => o.ToResponse());
    }
}
