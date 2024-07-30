using Application.Mapping.Extensions;
using Application.Operations.OrderLine;
using Application.Operations.OrderLine.Commands.AddOrderLine;
using Application.Operations.OrderLine.Commands.DeleteOrderLine;
using Application.Operations.OrderLine.Commands.UpdateOrderLine;
using Application.Operations.OrderLine.Queries.GetAllOrderLines;
using Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;
using Application.Operations.OrderLine.Queries.GetOrderLineById;
using Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using MediatR;

namespace DokWokApi.Endpoints;

public static class OrderLinesEndpoints
{
    private const string GetByIdRouteName = nameof(GetOrderLineById);

    public static void MapOrderLinesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.OrderLines.Group);
        group.MapGet("/", GetAllOrderLines)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.OrderLines.GetById, GetOrderLineById)
            .WithName(GetByIdRouteName)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapGet(ApiRoutes.OrderLines.GetByOrderAndProductIds, GetOrderLineByOrderAndProductIds)
            .RequireAuthorization(AuthorizationPolicyNames.AdminAndCustomer);
        group.MapPost("/", AddOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapPut("/", UpdateOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.OrderLines.DeleteById, DeleteOrderLine)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
    }

    public static async Task<IResult> GetAllOrderLines(ISender sender, long? orderId)
    {
        var orderLines = orderId.HasValue ?
                await sender.Send(new GetAllOrderLinesByOrderIdQuery(orderId.Value)) :
                await sender.Send(new GetAllOrderLinesQuery());

        return Results.Ok(orderLines);
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
        return result.ToCreatedAtRouteResult<OrderLineResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrderLine(ISender sender, UpdateOrderLineRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrderLine(ISender sender, long id)
    {
        var result = await sender.Send(new DeleteOrderLineCommand(id));
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
