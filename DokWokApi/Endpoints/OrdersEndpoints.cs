﻿using Application.Mapping.Extensions;
using DokWokApi.Helpers;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Queries.Orders;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;
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
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.Orders.GetById, GetOrderById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<OrderResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
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
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete(ApiRoutes.Orders.DeleteById, DeleteOrder)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    public static async Task<Ok<IEnumerable<OrderResponse>>> GetAllOrders(ISender sender,
        long? userId, int? pageNumber, int? pageSize)
    {
        IEnumerable<OrderResponse> orders;
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            orders = !userId.HasValue ?
                await sender.Send(new GetAllOrdersByPageQuery(pageNumber.Value, pageSize.Value)) :
                await sender.Send(new GetAllOrdersByUserIdAndPageQuery(userId.Value, pageNumber.Value, pageSize.Value));
        }
        else
        {
            orders = !userId.HasValue ?
                await sender.Send(new GetAllOrdersQuery()) :
                await sender.Send(new GetAllOrdersByUserIdQuery(userId.Value));
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
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> AddTakeawayOrder(ISender sender, AddTakeawayOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrder(ISender sender, UpdateOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrder(ISender sender, long id)
    {
        await sender.Send(new DeleteOrderCommand(id));
        return Results.Ok();
    }
}
