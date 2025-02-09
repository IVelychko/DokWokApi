﻿using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Shops;
using Domain.Shared;

namespace Domain.DTOs.Commands.Shops;

public sealed record AddShopCommand(
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
) : ICommand<Result<ShopResponse>>;
