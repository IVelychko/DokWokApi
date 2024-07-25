using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.Services;

namespace DokWokApi.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Cart.Group);
        group.MapGet("/", GetCart);
        group.MapPost(ApiRoutes.Cart.AddProduct, AddProductToCart);
        group.MapDelete(ApiRoutes.Cart.RemoveProduct, RemoveProductFromCart);
        group.MapDelete(ApiRoutes.Cart.RemoveLine, RemoveLineFromCart);
        group.MapDelete("/", ClearCart);
    }

    public static async Task<IResult> GetCart(ICartService cartService)
    {
        var cart = await cartService.GetCart();
        if (cart is null)
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Results.Ok(cart);
    }

    public static async Task<IResult> AddProductToCart(ICartService cartService, long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return Results.BadRequest("The quantity value must be greater than 0");
        }

        var result = await cartService.AddItem(productId, quantity);
        return result.ToOkResult();
    }

    public static async Task<IResult> RemoveProductFromCart(ICartService cartService, long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return Results.BadRequest("The quantity value must be greater than 0");
        }

        var result = await cartService.RemoveItem(productId, quantity);
        return result.ToOkResult();
    }

    public static async Task<IResult> RemoveLineFromCart(ICartService cartService, long productId)
    {
        var result = await cartService.RemoveLine(productId);
        return result.ToOkResult();
    }

    public static async Task<IResult> ClearCart(ICartService cartService)
    {
        var result = await cartService.ClearCart();
        if (!result)
        {
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Results.Ok();
    }
}
