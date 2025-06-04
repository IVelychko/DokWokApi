using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.OrderLines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.OrderLines.Group)]
public class OrderLinesController : ControllerBase
{
    private readonly IOrderLineService _orderLineService;

    public OrderLinesController(IOrderLineService orderLineService)
    {
        _orderLineService = orderLineService;
    }
    
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllOrderLines()
    {
        var orderLines = await _orderLineService.GetAllAsync();
        return Ok(orderLines);
    }
    
    [HttpGet(ApiRoutes.OrderLines.GetById)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    public async Task<IActionResult> GetOrderLineById(long id)
    {
        var orderLine = await _orderLineService.GetByIdAsync(id);
        return Ok(orderLine);
    }
    
    [HttpGet(ApiRoutes.OrderLines.GetByOrderAndProductIds)]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    public async Task<IActionResult> GetOrderLineByOrderAndProductIds(long orderId, long productId)
    {
        var orderLine = await _orderLineService.GetByOrderAndProductIdsAsync(orderId, productId);
        return Ok(orderLine);
    }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddOrderLine(AddOrderLineRequest request)
    {
        var orderLine = await _orderLineService.AddAsync(request);
        return CreatedAtAction(nameof(GetOrderLineById), new { id = orderLine.Id }, orderLine);
    }
    
    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateOrderLine(UpdateOrderLineRequest request)
    {
        var orderLine = await _orderLineService.UpdateAsync(request);
        return Ok(orderLine);
    }
    
    [HttpDelete(ApiRoutes.OrderLines.DeleteById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteOrderLine(long id)
    {
        await _orderLineService.DeleteAsync(id);
        return Ok();
    }
}