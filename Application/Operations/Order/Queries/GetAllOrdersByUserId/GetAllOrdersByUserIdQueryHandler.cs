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
        return await orderService.GetAllByUserIdAsync(request.UserId);
    }
}
