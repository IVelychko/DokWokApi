using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Orders.Controller)]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderLineService _orderLineService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, IOrderLineService orderLineService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _orderLineService = orderLineService;
        _logger = logger;
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
            _logger.LogInformation("The order was not found.");
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost(ApiRoutes.Orders.AddDelivery)]
    public async Task<IActionResult> AddDeliveryOrder(DeliveryOrderForm form, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<OrderModel>(form);
        var result = await _orderService.AddOrderFromCartAsync(model);
        return result.ToCreatedAtActionOrder(_logger, nameof(GetOrderById), nameof(OrdersController));
    }

    [HttpPost(ApiRoutes.Orders.AddTakeaway)]
    public async Task<IActionResult> AddTakeawayOrder(TakeawayOrderForm form, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<OrderModel>(form);
        var result = await _orderService.AddOrderFromCartAsync(model);
        return result.ToCreatedAtActionOrder(_logger, nameof(GetOrderById), nameof(OrdersController));
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateOrder(OrderPutModel putModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<OrderModel>(putModel);
        var result = await _orderService.UpdateAsync(model);
        return result.ToOk(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Orders.DeleteById)]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        var result = await _orderService.DeleteAsync(id);
        if (result is null)
        {
            _logger.LogInformation("Not found");
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        _logger.LogError("Server error");
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
            _logger.LogInformation("The order line was not found.");
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
            _logger.LogInformation("The order line was not found.");
            return NotFound();
        }

        return Ok(orderLine);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost(ApiRoutes.OrderLines.Add)]
    public async Task<IActionResult> AddOrderLine(OrderLinePostModel postModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<OrderLineModel>(postModel);
        var result = await _orderLineService.AddAsync(model);
        return result.ToCreatedAtAction(_logger, nameof(GetOrderLineById), nameof(OrdersController));
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut(ApiRoutes.OrderLines.Update)]
    public async Task<IActionResult> UpdateOrderLine(OrderLinePutModel putModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<OrderLineModel>(putModel);
        var result = await _orderLineService.UpdateAsync(model);
        return result.ToOk(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.OrderLines.DeleteById)]
    public async Task<IActionResult> DeleteOrderLine(long id)
    {
        var result = await _orderLineService.DeleteAsync(id);
        if (result is null)
        {
            _logger.LogInformation("Not found");
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        _logger.LogError("Server error");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
