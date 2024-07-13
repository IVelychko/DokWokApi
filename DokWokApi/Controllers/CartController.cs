using DokWokApi.BLL.Interfaces;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Cart.Controller)]
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
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCart();
        if (cart is null)
        {
            _logger.LogError("Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(cart);
    }

    [HttpPost(ApiRoutes.Cart.AddProduct)]
    public async Task<IActionResult> AddProductToCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            _logger.LogInformation("Bad request: Wrong product quantity.");
            return BadRequest("The quantity value must be greater than 0");
        }

        var result = await _cartService.AddItem(productId, quantity);
        return result.ToOkCart(_logger);
    }

    [HttpDelete(ApiRoutes.Cart.RemoveProduct)]
    public async Task<IActionResult> RemoveProductFromCart(long productId, int quantity)
    {
        if (quantity <= 0)
        {
            _logger.LogInformation("Bad request: Wrong product quantity.");
            return BadRequest("The quantity value must be greater than 0");
        }

        var result = await _cartService.RemoveItem(productId, quantity);
        return result.ToOkCart(_logger);
    }

    [HttpDelete(ApiRoutes.Cart.RemoveLine)]
    public async Task<IActionResult> RemoveLineFromCart(long productId)
    {
        var result = await _cartService.RemoveLine(productId);
        return result.ToOkCart(_logger);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var result = await _cartService.ClearCart();
        if (!result)
        {
            _logger.LogError("Cart error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}
