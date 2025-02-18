using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Requests.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class ShopMappingExtensions
{
    public static AddShopCommand ToCommand(this AddShopRequest request) =>
        new(request.Street, request.Building, request.OpeningTime, request.ClosingTime);

    public static UpdateShopCommand ToCommand(this UpdateShopRequest request) =>
        new(request.Id, request.Street, request.Building, request.OpeningTime, request.ClosingTime);

    // public static ShopModel ToModel(this AddShopCommand command)
    // {
    //     return new()
    //     {
    //         Building = command.Building,
    //         ClosingTime = command.ClosingTime,
    //         OpeningTime = command.OpeningTime,
    //         Street = command.Street
    //     };
    // }
    
    public static Shop ToEntity(this AddShopCommand command)
    {
        return new Shop
        {
            Building = command.Building,
            ClosingTime = command.ClosingTime,
            OpeningTime = command.OpeningTime,
            Street = command.Street
        };
    }

    // public static ShopModel ToModel(this UpdateShopCommand command)
    // {
    //     return new()
    //     {
    //         Id = command.Id,
    //         Building = command.Building,
    //         ClosingTime = command.ClosingTime,
    //         OpeningTime = command.OpeningTime,
    //         Street = command.Street
    //     };
    // }
    
    public static Shop ToEntity(this UpdateShopCommand command)
    {
        return new Shop
        {
            Id = command.Id,
            Building = command.Building,
            ClosingTime = command.ClosingTime,
            OpeningTime = command.OpeningTime,
            Street = command.Street
        };
    }

    // public static ShopResponse ToResponse(this ShopModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         Building = model.Building,
    //         ClosingTime = model.ClosingTime,
    //         OpeningTime = model.OpeningTime,
    //         Street = model.Street
    //     };
    // }
    
    public static ShopResponse ToResponse(this Shop entity)
    {
        return new ShopResponse
        {
            Id = entity.Id,
            Building = entity.Building,
            ClosingTime = entity.ClosingTime,
            OpeningTime = entity.OpeningTime,
            Street = entity.Street
        };
    }
}
