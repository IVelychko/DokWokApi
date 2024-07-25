using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.Services;

namespace DokWokApi.Endpoints;

public static class OrdersEndpoints
{
    private const string GetByIdRouteName = nameof(GetOrderById);

    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Orders.Group);
        group.MapGet("/", GetAllOrders)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.Orders.GetById, GetOrderById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapPost(ApiRoutes.Orders.AddDelivery, AddDeliveryOrder);
        group.MapPost(ApiRoutes.Orders.AddTakeaway, AddTakeawayOrder);
        group.MapPut("/", UpdateOrder)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.Orders.DeleteById, DeleteOrder)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
    }

    public static async Task<IResult> GetAllOrders(IOrderService orderService, string? userId)
    {
        var orders = userId is null ?
                await orderService.GetAllAsync() :
                await orderService.GetAllByUserIdAsync(userId);

        return Results.Ok(orders);
    }

    public static async Task<IResult> GetOrderById(IOrderService orderService, long id)
    {
        var order = await orderService.GetByIdAsync(id);
        if (order is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(order);
    }

    public static async Task<IResult> AddDeliveryOrder(IOrderService orderService, ICartService cartService, DeliveryOrderModel form)
    {
        var model = form.ToModel();
        var result = await orderService.AddOrderFromCartAsync(model, cartService);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> AddTakeawayOrder(IOrderService orderService, ICartService cartService, TakeawayOrderModel form)
    {
        var model = form.ToModel();
        var result = await orderService.AddOrderFromCartAsync(model, cartService);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrder(IOrderService orderService, OrderPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await orderService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrder(IOrderService orderService, long id)
    {
        var result = await orderService.DeleteAsync(id);
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
}
