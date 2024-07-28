using Application.Operations.Shop.Commands.AddShop;
using Application.Operations.Shop.Commands.UpdateShop;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class ShopMappingExtensions
{
    public static AddShopCommand ToCommand(this AddShopRequest request) =>
        new(request.Street, request.Building, request.OpeningTime, request.ClosingTime);

    public static UpdateShopCommand ToCommand(this UpdateShopRequest request) =>
        new(request.Id, request.Street, request.Building, request.OpeningTime, request.ClosingTime);

    public static ShopModel ToModel(this AddShopCommand command)
    {
        return new()
        {
            Building = command.Building,
            ClosingTime = command.ClosingTime,
            OpeningTime = command.OpeningTime,
            Street = command.Street
        };
    }

    public static ShopModel ToModel(this UpdateShopCommand command)
    {
        return new()
        {
            Id = command.Id,
            Building = command.Building,
            ClosingTime = command.ClosingTime,
            OpeningTime = command.OpeningTime,
            Street = command.Street
        };
    }
}
