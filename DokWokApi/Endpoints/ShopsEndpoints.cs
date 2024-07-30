using Application.Mapping.Extensions;
using Application.Operations.Shop;
using Application.Operations.Shop.Commands.AddShop;
using Application.Operations.Shop.Commands.DeleteShop;
using Application.Operations.Shop.Commands.UpdateShop;
using Application.Operations.Shop.Queries.GetAllShops;
using Application.Operations.Shop.Queries.GetShopByAddress;
using Application.Operations.Shop.Queries.GetShopById;
using Application.Operations.Shop.Queries.IsShopAddressTaken;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using MediatR;

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

    public static async Task<IResult> GetAllShops(ISender sender)
    {
        var shops = await sender.Send(new GetAllShopsQuery());
        return Results.Ok(shops);
    }

    public static async Task<IResult> GetShopById(ISender sender, long id)
    {
        var shop = await sender.Send(new GetShopByIdQuery(id));
        if (shop is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(shop);
    }

    public static async Task<IResult> GetShopByAddress(ISender sender, string street, string building)
    {
        var shop = await sender.Send(new GetShopByAddressQuery(street, building));
        if (shop is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(shop);
    }

    public static async Task<IResult> AddShop(ISender sender, AddShopRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult<ShopResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateShop(ISender sender, UpdateShopRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteShop(ISender sender, long id)
    {
        var result = await sender.Send(new DeleteShopCommand(id));
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

    public static async Task<IResult> IsShopAddressTaken(ISender sender, string street, string building)
    {
        var result = await sender.Send(new IsShopAddressTakenQuery(street, building));
        return result.ToOkIsTakenResult();
    }
}
