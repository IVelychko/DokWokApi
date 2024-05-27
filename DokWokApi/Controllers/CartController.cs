using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Cart>> GetCart()
    {
        try
        {
            var cart = await _cartService.GetCart();
            return Ok(cart);
        }
        catch (CartException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("item")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Cart>> AddProductToCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            var modifiedCart = await _cartService.AddItem(productId, quantity);
            return Ok(modifiedCart);
        }
        catch (CartException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("item")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Cart>> RemoveProductFromCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            var modifiedCart = await _cartService.RemoveItem(productId, quantity);
            return Ok(modifiedCart);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CartException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("line")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Cart>> RemoveLineFromCart(long productId)
    {
        try
        {
            var modifiedCart = await _cartService.RemoveLine(productId);
            return Ok(modifiedCart);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CartException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ClearCart()
    {
        try
        {
            await _cartService.ClearCart();
            return Ok("The cart was cleared successfully.");
        }
        catch (CartException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
