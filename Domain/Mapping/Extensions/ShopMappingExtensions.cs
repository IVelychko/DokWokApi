using Domain.Entities;
using Domain.Models;

namespace Domain.Mapping.Extensions;

public static class ShopMappingExtensions
{
    public static ShopModel ToModel(this Shop entity)
    {
        return new()
        {
            Id = entity.Id,
            Building = entity.Building,
            ClosingTime = entity.ClosingTime,
            OpeningTime = entity.OpeningTime,
            Street = entity.Street
        };
    }

    public static Shop ToEntity(this ShopModel model)
    {
        return new()
        {
            Id = model.Id,
            Building = model.Building,
            ClosingTime = model.ClosingTime,
            OpeningTime = model.OpeningTime,
            Street = model.Street
        };
    }
}
