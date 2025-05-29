using Domain.DTOs.Requests.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class ShopMappingExtensions
{
    public static Shop ToEntity(this AddShopRequest command)
    {
        return new Shop
        {
            Building = command.Building,
            ClosingTime = command.ClosingTime,
            OpeningTime = command.OpeningTime,
            Street = command.Street
        };
    }
    
    public static Shop ToEntity(this UpdateShopRequest command)
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
