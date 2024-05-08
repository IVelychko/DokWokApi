using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("ReactProjectCorsPolicy")]
public class CartController : ControllerBase
{
    private readonly ICartService cartService;

    public CartController(ICartService cartService)
    {
        this.cartService = cartService;
    }

    [HttpGet]
    public ActionResult<Cart> GetCart()
    {
        try
        {
            return Ok(cartService.GetCart());
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
    public async Task<ActionResult> AddProductToCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            await cartService.AddItem(productId, quantity);
            return Ok("The product was added to the cart successfully.");
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
    public async Task<ActionResult> RemoveProductFromCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        try
        {
            await cartService.RemoveItem(productId, quantity);
            return Ok("The product was removed from the cart successfully.");
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
    public async Task<ActionResult> RemoveLineFromCart(long productId)
    {
        try
        {
            await cartService.RemoveLine(productId);
            return Ok("The cart line was removed successfully.");
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
    public ActionResult ClearCart()
    {
        try
        {
            cartService.ClearCart();
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
