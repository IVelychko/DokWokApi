﻿using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;

public class GetAllOrderLinesByOrderIdQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        return await orderLineService.GetAllByOrderIdAsync(request.OrderId);
    }
}
