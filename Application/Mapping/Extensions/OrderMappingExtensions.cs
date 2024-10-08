﻿using Application.Operations.Order;
using Application.Operations.Order.Commands.AddDeliveryOrder;
using Application.Operations.Order.Commands.AddTakeawayOrder;
using Application.Operations.Order.Commands.UpdateOrder;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class OrderMappingExtensions
{
    public static AddDeliveryOrderCommand ToCommand(this AddDeliveryOrderRequest request) =>
        new(
            request.CustomerName,
            request.PhoneNumber,
            request.Email,
            request.DeliveryAddress,
            request.PaymentType,
            request.UserId,
            request.OrderLines);

    public static AddTakeawayOrderCommand ToCommand(this AddTakeawayOrderRequest request) =>
        new(
            request.CustomerName,
            request.PhoneNumber,
            request.Email,
            request.PaymentType,
            request.UserId,
            request.ShopId,
            request.OrderLines);

    public static UpdateOrderCommand ToCommand(this UpdateOrderRequest request) =>
        new(
            request.Id,
            request.CustomerName,
            request.PhoneNumber,
            request.Email,
            request.DeliveryAddress,
            request.PaymentType,
            request.TotalOrderPrice,
            request.CreationDate,
            request.Status,
            request.UserId,
            request.ShopId);

    public static OrderModel ToModel(this AddDeliveryOrderCommand command)
    {
        return new()
        {
            DeliveryAddress = command.DeliveryAddress,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            OrderLines = command.OrderLines.Select(r => r.ToModel()).ToList()
        };
    }

    public static OrderModel ToModel(this AddTakeawayOrderCommand command)
    {
        return new()
        {
            ShopId = command.ShopId,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            OrderLines = command.OrderLines.Select(r => r.ToModel()).ToList()
        };
    }

    public static OrderModel ToModel(this UpdateOrderCommand command)
    {
        return new()
        {
            ShopId = command.ShopId,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            DeliveryAddress = command.DeliveryAddress,
            CreationDate = command.CreationDate,
            Id = command.Id,
            TotalOrderPrice = command.TotalOrderPrice,
            Status = command.Status
        };
    }

    public static OrderResponse ToResponse(this OrderModel model)
    {
        return new()
        {
            Id = model.Id,
            CreationDate = model.CreationDate,
            CustomerName = model.CustomerName,
            Email = model.Email,
            PaymentType = model.PaymentType,
            Status = model.Status,
            TotalOrderPrice = model.TotalOrderPrice,
            PhoneNumber = model.PhoneNumber,
            DeliveryAddress = model.DeliveryAddress,
            ShopId = model.ShopId,
            UserId = model.UserId,
            OrderLines = model.OrderLines,
        };
    }
}
