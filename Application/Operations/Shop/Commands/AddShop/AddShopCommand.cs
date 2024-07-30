﻿using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.Shop.Commands.AddShop;

public sealed record AddShopCommand(
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
) : ICommand<Result<ShopResponse>>;
