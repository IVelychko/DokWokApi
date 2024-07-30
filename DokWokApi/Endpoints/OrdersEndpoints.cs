using Application.Mapping.Extensions;
using Application.Operations.Order;
using Application.Operations.Order.Commands.AddDeliveryOrder;
using Application.Operations.Order.Commands.AddTakeawayOrder;
using Application.Operations.Order.Commands.DeleteOrder;
using Application.Operations.Order.Commands.UpdateOrder;
using Application.Operations.Order.Queries.GetAllOrders;
using Application.Operations.Order.Queries.GetAllOrdersByUserId;
using Application.Operations.Order.Queries.GetOrderById;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using MediatR;

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

    public static async Task<IResult> GetAllOrders(ISender sender, string? userId)
    {
        var orders = userId is null ?
                await sender.Send(new GetAllOrdersQuery()) :
                await sender.Send(new GetAllOrdersByUserIdQuery(userId));

        return Results.Ok(orders);
    }

    public static async Task<IResult> GetOrderById(ISender sender, long id)
    {
        var order = await sender.Send(new GetOrderByIdQuery(id));
        if (order is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(order);
    }

    public static async Task<IResult> AddDeliveryOrder(ISender sender, AddDeliveryOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult<OrderResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> AddTakeawayOrder(ISender sender, AddTakeawayOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult<OrderResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrder(ISender sender, UpdateOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrder(ISender sender, long id)
    {
        var result = await sender.Send(new DeleteOrderCommand(id));
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
