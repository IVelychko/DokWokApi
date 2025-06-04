using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Orders.Group)]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderLineService _orderLineService;

    public OrdersController(
        IOrderService orderService,
        IOrderLineService orderLineService)
    {
        _orderService = orderService;
        _orderLineService = orderLineService;
    }

    [HttpGet(ApiRoutes.Orders.GetAllOrderLinesByOrderId)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    public async Task<IActionResult> GetAllOrderLinesByOrderId(long orderId)
    {
        var orderLines = await _orderLineService.GetAllByOrderIdAsync(orderId);
        return Ok(orderLines);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }
    
    [HttpGet(ApiRoutes.Orders.GetById)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    public async Task<IActionResult> GetOrderById(long id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order);
    }
    
    [HttpPost(ApiRoutes.Orders.AddDelivery)]
    public async Task<IActionResult> AddDeliveryOrder(AddDeliveryOrderRequest request)
    {
        var order = await _orderService.AddAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }
    
    [HttpPost(ApiRoutes.Orders.AddTakeaway)]
    public async Task<IActionResult> AddTakeawayOrder(AddTakeawayOrderRequest request)
    {
        var order = await _orderService.AddAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }
    
    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateOrder(UpdateOrderRequest request)
    {
        var order = await _orderService.UpdateAsync(request);
        return Ok(order);
    }
    
    [HttpDelete(ApiRoutes.Orders.DeleteById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        await _orderService.DeleteAsync(id);
        return Ok();
    }
}