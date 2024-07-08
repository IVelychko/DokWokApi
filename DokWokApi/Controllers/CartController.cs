using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<Cart>> GetCart()
    {
        try
        {
            var cart = await _cartService.GetCart();
            return Ok(cart);
        }
        catch (CartException ex)
        {
            _logger.LogError(ex, "Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("item")]
    public async Task<ActionResult<Cart>> AddProductToCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            _logger.LogInformation("Bad request: Wrong product quantity.");
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            var modifiedCart = await _cartService.AddItem(productId, quantity);
            return Ok(modifiedCart);
        }
        catch (CartException ex)
        {
            _logger.LogError(ex, "Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
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

    [HttpDelete("item")]
    public async Task<ActionResult<Cart>> RemoveProductFromCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            _logger.LogInformation("Bad request: Wrong product quantity.");
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            var modifiedCart = await _cartService.RemoveItem(productId, quantity);
            return Ok(modifiedCart);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (CartNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (CartException ex)
        {
            _logger.LogError(ex, "Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("line")]
    public async Task<ActionResult<Cart>> RemoveLineFromCart(long productId)
    {
        try
        {
            var modifiedCart = await _cartService.RemoveLine(productId);
            return Ok(modifiedCart);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (CartNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (CartException ex)
        {
            _logger.LogError(ex, "Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        try
        {
            await _cartService.ClearCart();
            return Ok();
        }
        catch (CartException ex)
        {
            _logger.LogError(ex, "Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
