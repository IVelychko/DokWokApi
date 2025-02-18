using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrdersByPage;

public sealed class GetAllOrdersByPageQueryHandler(IOrderService orderService)
    : IQueryHandler<GetAllOrdersByPageQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await orderService.GetAllAsync(pageInfo);
    }
}
