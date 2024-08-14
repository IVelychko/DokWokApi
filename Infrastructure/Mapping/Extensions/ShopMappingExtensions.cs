using Domain.Entities;
using Infrastructure.Validation.Shops.Add;
using Infrastructure.Validation.Shops.Update;

namespace Infrastructure.Mapping.Extensions;

public static class ShopMappingExtensions
{
    public static AddShopValidationModel ToAddValidationModel(this Shop shop) =>
        new(shop.Street, shop.Building, shop.OpeningTime, shop.ClosingTime);

    public static UpdateShopValidationModel ToUpdateValidationModel(this Shop shop) =>
        new(shop.Id, shop.Street, shop.Building, shop.OpeningTime, shop.ClosingTime);
}
