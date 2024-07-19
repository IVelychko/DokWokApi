using DokWokApi.BLL.Models.Shop;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL.Extensions;

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

    public static ShopModel ToModel(this ShopPostModel model)
    {
        return new()
        {
            Building = model.Building!,
            ClosingTime = model.ClosingTime!,
            OpeningTime = model.OpeningTime!,
            Street = model.Street!
        };
    }

    public static ShopModel ToModel(this ShopPutModel model)
    {
        return new()
        {
            Id = model.Id!.Value,
            Building = model.Building!,
            ClosingTime = model.ClosingTime!,
            OpeningTime = model.OpeningTime!,
            Street = model.Street!
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
