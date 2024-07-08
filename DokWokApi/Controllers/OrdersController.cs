using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/orders")]
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
    public async Task<ActionResult<IEnumerable<OrderModel>>> GetAllOrders(string? userId)
    {
        try
        {
            var orders = userId is null ? 
                await _orderService.GetAllAsync() : 
                await _orderService.GetAllByUserIdAsync(userId);

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderModel>> GetOrderById(long id)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order is null)
            {
                _logger.LogInformation("The order was not found.");
                return NotFound("The order was not found.");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("delivery")]
    public async Task<ActionResult<OrderModel>> AddDeliveryOrder(DeliveryOrderForm form, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<OrderModel>(form);
            var addedModel = await _orderService.AddOrderFromCartAsync(model);
            return CreatedAtAction(nameof(GetOrderById), new { id = addedModel.Id }, addedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (OrderException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("takeaway")]
    public async Task<ActionResult<OrderModel>> AddTakeawayOrder(TakeawayOrderForm form, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<OrderModel>(form);
            var addedModel = await _orderService.AddOrderFromCartAsync(model);
            return CreatedAtAction(nameof(GetOrderById), new { id = addedModel.Id }, addedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (OrderException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<ActionResult<OrderModel>> UpdateOrder(OrderPutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<OrderModel>(putModel);
            var updatedModel = await _orderService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(long id)
    {
        try
        {
            await _orderService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("lines")]
    public async Task<ActionResult<IEnumerable<OrderLineModel>>> GetAllOrderLines(long? orderId)
    {
        try
        {
            var orderLines = orderId.HasValue ?
                await _orderLineService.GetAllByOrderIdAsync(orderId.Value) :
                await _orderLineService.GetAllAsync();

            return Ok(orderLines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("lines/{id}")]
    public async Task<ActionResult<OrderLineModel>> GetOrderLineById(long id)
    {
        try
        {
            var orderLine = await _orderLineService.GetByIdAsync(id);
            if (orderLine is null)
            {
                _logger.LogInformation("The order line was not found.");
                return NotFound("The order line was not found.");
            }

            return Ok(orderLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet("lines/{orderId}/{productId}")]
    public async Task<ActionResult<OrderLineModel>> GetOrderLineByOrderAndProductIds(long orderId, long productId)
    {
        try
        {
            var orderLine = await _orderLineService.GetByOrderAndProductIdsAsync(orderId, productId);
            if (orderLine is null)
            {
                _logger.LogInformation("The order line was not found.");
                return NotFound("The order line was not found.");
            }

            return Ok(orderLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost("lines")]
    public async Task<ActionResult<OrderLineModel>> AddOrderLine(OrderLinePostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<OrderLineModel>(postModel);
            var addedModel = await _orderLineService.AddAsync(model);
            return CreatedAtAction(nameof(GetOrderLineById), new { id = addedModel.Id }, addedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut("lines")]
    public async Task<ActionResult<OrderLineModel>> UpdateOrderLine(OrderLinePutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<OrderLineModel>(putModel);
            var updatedModel = await _orderLineService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("lines/{id}")]
    public async Task<ActionResult> DeleteOrderLine(long id)
    {
        try
        {
            await _orderLineService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
