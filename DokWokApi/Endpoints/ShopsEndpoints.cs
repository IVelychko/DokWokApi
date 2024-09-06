using Application.Mapping.Extensions;
using Application.Operations;
using Application.Operations.Shop;
using Application.Operations.Shop.Commands.AddShop;
using Application.Operations.Shop.Commands.DeleteShop;
using Application.Operations.Shop.Commands.UpdateShop;
using Application.Operations.Shop.Queries.GetAllShops;
using Application.Operations.Shop.Queries.GetAllShopsByPage;
using Application.Operations.Shop.Queries.GetShopByAddress;
using Application.Operations.Shop.Queries.GetShopById;
using Application.Operations.Shop.Queries.IsShopAddressTaken;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DokWokApi.Endpoints;

public static class ShopsEndpoints
{
    private const string GetByIdRouteName = nameof(GetShopById);

    public static void AddShopsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Shops.Group).WithTags("Shops");

        group.MapGet("/", GetAllShops);

        group.MapGet(ApiRoutes.Shops.GetById, GetShopById)
            .WithName(GetByIdRouteName)
            .Produces<ShopResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(ApiRoutes.Shops.GetByAddress, GetShopByAddress)
            .Produces<ShopResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddShop)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ShopResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/", UpdateShop)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ShopResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete(ApiRoutes.Shops.DeleteById, DeleteShop)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.Shops.IsAddressTaken, IsShopAddressTaken)
            .Produces<IsTakenResponse>()
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<Ok<IEnumerable<ShopResponse>>> GetAllShops(ISender sender,
        int? pageNumber, int? pageSize)
    {
        var shops = pageNumber.HasValue && pageSize.HasValue ?
            await sender.Send(new GetAllShopsByPageQuery(pageNumber.Value, pageSize.Value)) :
            await sender.Send(new GetAllShopsQuery());

        return TypedResults.Ok(shops);
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
