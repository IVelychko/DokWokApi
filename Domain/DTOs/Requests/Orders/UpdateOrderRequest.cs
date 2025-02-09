﻿namespace Domain.DTOs.Requests.Orders;

public sealed record UpdateOrderRequest(
    long Id,
    string CustomerName,
    string PhoneNumber,
    string Email,
    string? DeliveryAddress,
    string PaymentType,
    decimal TotalOrderPrice,
    DateTime CreationDate,
    string Status,
    long? UserId,
    long? ShopId
);
