using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using DokWokApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Cart.Controller)]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCart();
        if (cart is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(cart);
    }

    [HttpPost(ApiRoutes.Cart.AddProduct)]
    public async Task<IActionResult> AddProductToCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        var result = await _cartService.AddItem(productId, quantity);
        return result.ToOkCartResult();
    }

    [HttpDelete(ApiRoutes.Cart.RemoveProduct)]
    public async Task<IActionResult> RemoveProductFromCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("The quantity value must be greater than 0");
        }

        var result = await _cartService.RemoveItem(productId, quantity);
        return result.ToOkCartResult();
    }

    [HttpDelete(ApiRoutes.Cart.RemoveLine)]
    public async Task<IActionResult> RemoveLineFromCart(long productId)
    {
        var result = await _cartService.RemoveLine(productId);
        return result.ToOkCartResult();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var result = await _cartService.ClearCart();
        if (!result)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}
