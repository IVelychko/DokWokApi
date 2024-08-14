using Application.Mapping.Extensions;
using Application.Operations.Order;
using Application.Operations.Order.Commands.AddDeliveryOrder;
using Application.Operations.Order.Commands.AddTakeawayOrder;
using Application.Operations.Order.Commands.DeleteOrder;
using Application.Operations.Order.Commands.UpdateOrder;
using Application.Operations.Order.Queries.GetAllOrders;
using Application.Operations.Order.Queries.GetAllOrdersByPage;
using Application.Operations.Order.Queries.GetAllOrdersByUserId;
using Application.Operations.Order.Queries.GetAllOrdersByUserIdAndPage;
using Application.Operations.Order.Queries.GetOrderById;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DokWokApi.Endpoints;

public static class OrdersEndpoints
{
    private const string GetByIdRouteName = nameof(GetOrderById);

    public static void AddOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Orders.Group).WithTags("Orders");

        group.MapGet("/", GetAllOrders)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);

        group.MapGet(ApiRoutes.Orders.GetById, GetOrderById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<OrderResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost(ApiRoutes.Orders.AddDelivery, AddDeliveryOrder)
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPost(ApiRoutes.Orders.AddTakeaway, AddTakeawayOrder)
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapPut("/", UpdateOrder)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<OrderResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest);

        group.MapDelete(ApiRoutes.Orders.DeleteById, DeleteOrder)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Ok<IEnumerable<OrderResponse>>> GetAllOrders(ISender sender,
        string? userId, int? pageNumber, int? pageSize)
    {
        IEnumerable<OrderResponse> orders;
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            orders = userId is null ?
                await sender.Send(new GetAllOrdersByPageQuery(pageNumber.Value, pageSize.Value)) :
                await sender.Send(new GetAllOrdersByUserIdAndPageQuery(userId, pageNumber.Value, pageSize.Value));
        }
        else
        {
            orders = userId is null ?
                await sender.Send(new GetAllOrdersQuery()) :
                await sender.Send(new GetAllOrdersByUserIdQuery(userId));
        }

        return TypedResults.Ok(orders);
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
