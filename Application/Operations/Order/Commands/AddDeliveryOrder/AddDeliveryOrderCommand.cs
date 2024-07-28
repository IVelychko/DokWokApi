﻿using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed record AddDeliveryOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    string? UserId
) : ICommand<Result<OrderModel>>;
