﻿using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Shared;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public class AddDeliveryOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<AddDeliveryOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(AddDeliveryOrderCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderService.AddAsync(model);
        return result.Match(o => o.ToResponse(), Result<OrderResponse>.Failure);
    }
}
