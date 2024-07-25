using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Orders.Group)]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderLineService _orderLineService;

    public OrdersController(IOrderService orderService, IOrderLineService orderLineService)
    {
        _orderService = orderService;
        _orderLineService = orderLineService;
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders(string? userId)
    {
        var orders = userId is null ?
                await _orderService.GetAllAsync() :
                await _orderService.GetAllByUserIdAsync(userId);

        return Ok(orders);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.Orders.GetById)]
    public async Task<IActionResult> GetOrderById(long id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost(ApiRoutes.Orders.AddDelivery)]
    public async Task<IActionResult> AddDeliveryOrder(DeliveryOrderModel form, [FromServices] ICartService cartService)
    {
        var model = form.ToModel();
        var result = await _orderService.AddOrderFromCartAsync(model, cartService);
        return result.ToCreatedAtActionActionResult(nameof(GetOrderById), "Orders");
    }

    [HttpPost(ApiRoutes.Orders.AddTakeaway)]
    public async Task<IActionResult> AddTakeawayOrder(TakeawayOrderModel form, [FromServices] ICartService cartService)
    {
        var model = form.ToModel();
        var result = await _orderService.AddOrderFromCartAsync(model, cartService);
        return result.ToCreatedAtActionActionResult(nameof(GetOrderById), "Orders");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateOrder(OrderPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _orderService.UpdateAsync(model);
        return result.ToOkActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Orders.DeleteById)]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        var result = await _orderService.DeleteAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.OrderLines.GetAll)]
    public async Task<IActionResult> GetAllOrderLines(long? orderId)
    {
        var orderLines = orderId.HasValue ?
                await _orderLineService.GetAllByOrderIdAsync(orderId.Value) :
                await _orderLineService.GetAllAsync();

        return Ok(orderLines);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.OrderLines.GetById)]
    public async Task<IActionResult> GetOrderLineById(long id)
    {
        var orderLine = await _orderLineService.GetByIdAsync(id);
        if (orderLine is null)
        {
            return NotFound();
        }

        return Ok(orderLine);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet(ApiRoutes.OrderLines.GetByOrderAndProductIds)]
    public async Task<IActionResult> GetOrderLineByOrderAndProductIds(long orderId, long productId)
    {
        var orderLine = await _orderLineService.GetByOrderAndProductIdsAsync(orderId, productId);
        if (orderLine is null)
        {
            return NotFound();
        }

        return Ok(orderLine);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost(ApiRoutes.OrderLines.Add)]
    public async Task<IActionResult> AddOrderLine(OrderLinePostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _orderLineService.AddAsync(model);
        return result.ToCreatedAtActionActionResult(nameof(GetOrderLineById), "Orders");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut(ApiRoutes.OrderLines.Update)]
    public async Task<IActionResult> UpdateOrderLine(OrderLinePutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _orderLineService.UpdateAsync(model);
        return result.ToOkActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.OrderLines.DeleteById)]
    public async Task<IActionResult> DeleteOrderLine(long id)
    {
        var result = await _orderLineService.DeleteAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
