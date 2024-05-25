using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Services;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService orderService;

    public OrdersController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderModel>> AddOrder(OrderForm model)
    {
        try
        {
            var addedModel = await orderService.AddAsync(model);
            return Ok(addedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (OrderException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
