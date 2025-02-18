using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByPage;

public sealed class GetAllOrderLinesByPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await orderLineService.GetAllAsync(pageInfo);
    }
}
