using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
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

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet]
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
    [HttpPost]
    public async Task<IActionResult> AddOrderLine(OrderLinePostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _orderLineService.AddAsync(model);
        return result.ToCreatedAtActionResult(nameof(GetOrderLineById), "Orders");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
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
