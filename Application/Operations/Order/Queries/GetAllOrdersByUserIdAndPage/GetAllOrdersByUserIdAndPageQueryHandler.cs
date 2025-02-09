using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserIdAndPage;

public sealed class GetAllOrdersByUserIdAndPageQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByUserIdAndPageQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByUserIdAndPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var orders = await orderService.GetAllByUserIdAsync(request.UserId, pageInfo);
        return orders.Select(o => o.ToResponse());
    }
}
