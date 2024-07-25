using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;

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

    public static async Task<IResult> GetAllOrderLines(IOrderLineService orderLineService, long? orderId)
    {
        var orderLines = orderId.HasValue ?
                await orderLineService.GetAllByOrderIdAsync(orderId.Value) :
                await orderLineService.GetAllAsync();

        return Results.Ok(orderLines);
    }

    public static async Task<IResult> GetOrderLineById(IOrderLineService orderLineService, long id)
    {
        var orderLine = await orderLineService.GetByIdAsync(id);
        if (orderLine is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(orderLine);
    }

    public static async Task<IResult> GetOrderLineByOrderAndProductIds(IOrderLineService orderLineService, long orderId, long productId)
    {
        var orderLine = await orderLineService.GetByOrderAndProductIdsAsync(orderId, productId);
        if (orderLine is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(orderLine);
    }

    public static async Task<IResult> AddOrderLine(IOrderLineService orderLineService, OrderLinePostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await orderLineService.AddAsync(model);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateOrderLine(IOrderLineService orderLineService, OrderLinePutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await orderLineService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteOrderLine(IOrderLineService orderLineService, long id)
    {
        var result = await orderLineService.DeleteAsync(id);
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
