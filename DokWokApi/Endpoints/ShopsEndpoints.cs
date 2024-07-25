using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;

namespace DokWokApi.Endpoints;

public static class ShopsEndpoints
{
    private const string GetByIdRouteName = nameof(GetShopById);

    public static void MapShopsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Shops.Group);
        group.MapGet("/", GetAllShops);
        group.MapGet(ApiRoutes.Shops.GetById, GetShopById).WithName(GetByIdRouteName);
        group.MapGet(ApiRoutes.Shops.GetByAddress, GetShopByAddress);
        group.MapPost("/", AddShop).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapPut("/", UpdateShop).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.Shops.DeleteById, DeleteShop).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.Shops.IsAddressTaken, IsShopAddressTaken);
    }

    public static async Task<IResult> GetAllShops(IShopService shopService)
    {
        var shops = await shopService.GetAllAsync();
        return Results.Ok(shops);
    }

    public static async Task<IResult> GetShopById(IShopService shopService, long id)
    {
        var shop = await shopService.GetByIdAsync(id);
        if (shop is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(shop);
    }

    public static async Task<IResult> GetShopByAddress(IShopService shopService, string street, string building)
    {
        var shop = await shopService.GetByAddressAsync(street, building);
        if (shop is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(shop);
    }

    public static async Task<IResult> AddShop(IShopService shopService, ShopPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await shopService.AddAsync(model);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateShop(IShopService shopService, ShopPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await shopService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteShop(IShopService shopService, long id)
    {
        var result = await shopService.DeleteAsync(id);
        if (result is null)
        {
            return Results.NotFound();
        }
        else if (result.Value)
        {
            return Results.Ok();
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> IsShopAddressTaken(IShopService shopService, string street, string building)
    {
        var result = await shopService.IsAddressTaken(street, building);
        return result.ToOkIsTakenResult();
    }
}
