using Application.Mapping.Extensions;
using Application.Operations.OrderLine;
using Application.Operations.OrderLine.Commands.AddOrderLine;
using Application.Operations.OrderLine.Commands.DeleteOrderLine;
using Application.Operations.OrderLine.Commands.UpdateOrderLine;
using Application.Operations.OrderLine.Queries.GetAllOrderLines;
using Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;
using Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderIdAndPage;
using Application.Operations.OrderLine.Queries.GetAllOrderLinesByPage;
using Application.Operations.OrderLine.Queries.GetOrderLineById;
using Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DokWokApi.Endpoints;

public static class OrderLinesEndpoints
{
    private const string GetByIdRouteName = nameof(GetOrderLineById);

    public static void AddOrderLinesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.OrderLines.Group).WithTags("OrderLines");

        group.MapGet("/", GetAllOrderLines)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.OrderLines.GetById, GetOrderLineById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<OrderLineResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(ApiRoutes.OrderLines.GetByOrderAndProductIds, GetOrderLineByOrderAndProductIds)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer)
            .Produces<OrderLineResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<OrderLineResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/", UpdateOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<OrderLineResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete(ApiRoutes.OrderLines.DeleteById, DeleteOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    public static async Task<Ok<IEnumerable<OrderLineResponse>>> GetAllOrderLines(ISender sender,
        long? orderId, int? pageNumber, int? pageSize)
    {
        IEnumerable<OrderLineResponse> orderLines;
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            orderLines = orderId.HasValue ?
                await sender.Send(new GetAllOrderLinesByOrderIdAndPageQuery(orderId.Value, pageNumber.Value, pageSize.Value)) :
                await sender.Send(new GetAllOrderLinesByPageQuery(pageNumber.Value, pageSize.Value));
        }
        else
        {
            orderLines = orderId.HasValue ?
                await sender.Send(new GetAllOrderLinesByOrderIdQuery(orderId.Value)) :
                await sender.Send(new GetAllOrderLinesQuery());
        }

        return TypedResults.Ok(orderLines);
    }

    public static async Task<IResult> GetOrderLineById(ISender sender, long id)
    {
        var orderLine = await sender.Send(new GetOrderLineByIdQuery(id));
        if (orderLine is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(orderLine);
    }

    public static async Task<IResult> GetOrderLineByOrderAndProductIds(ISender sender, long orderId, long productId)
    {
        var orderLine = await sender.Send(new GetOrderLineByOrderAndProductIdsQuery(orderId, productId));
        if (orderLine is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(orderLine);
    }

    public static async Task<IResult> AddOrderLine(ISender sender, AddOrderLineRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrderLine(ISender sender, UpdateOrderLineRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrderLine(ISender sender, long id)
    {
        await sender.Send(new DeleteOrderLineCommand(id));
        return Results.Ok();
    }
}
