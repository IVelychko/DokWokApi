using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Shop.Commands.UpdateShop;

public sealed record UpdateShopCommand(
    long Id,
    string Street,
    string Building,
    string OpeningTime,
    string ClosingTime
) : ICommand<Result<ShopModel>>;
