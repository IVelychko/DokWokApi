using DokWokApi.BLL.Interfaces;
using DokWokApi.Models.ShoppingCart;
using DokWokApi.DAL.Exceptions;
using DokWokApi.Extensions;
using DokWokApi.DAL.ResultType;

namespace DokWokApi.Services;

public class SessionCartService : ICartService
{
    private readonly IProductService _productService;
    private readonly ISession? _session;

    public SessionCartService(IProductService productService, IHttpContextAccessor httpContextAccessor)
    {
        _productService = productService;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<Cart?> GetCart()
    {
        if (_session is null)
        {
            return null;
        }

        return await _session.GetJsonAsync<Cart>("Cart") ?? new Cart();
    }

    public async Task<Result<Cart?>> AddItem(long productId, int quantity)
    {
        if (_session is null)
        {
            return null;
        }

        if (quantity <= 0)
        {
            var exception = new ValidationException("The quantity value must be greater than 0");
            return new Result<Cart?>(exception);
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            var exception = new NotFoundException("There is no entity with this ID in the database.");
            return new Result<Cart?>(exception);
        }

        var cart = await _session.GetJsonAsync<Cart>("Cart") ?? new Cart();

        var cartLine = cart.Lines.Find(cl => cl.Product.Id == productId);
        if (cartLine is null)
        {
            cart.Lines.Add(new CartLine
            {
                Product = product,
                Quantity = quantity,
                TotalLinePrice = product.Price * quantity
            });
        }
        else
        {
            cartLine.Quantity += quantity;
            cartLine.CalculateTotalLinePrice();
        }

        cart.CalculateTotalCartPrice();

        await _session.SetJsonAsync("Cart", cart);
        return cart;
    }

    public async Task<Result<Cart?>> RemoveItem(long productId, int quantity)
    {
        if (_session is null)
        {
            return null;
        }

        if (quantity <= 0)
        {
            var exception = new ValidationException("The quantity value must be greater than 0");
            return new Result<Cart?>(exception);
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            var exception = new NotFoundException("There is no entity with this ID in the database.");
            return new Result<Cart?>(exception);
        }

        var cart = await _session.GetJsonAsync<Cart>("Cart");
        if (cart is null)
        {
            var exception = new NotFoundException("There is no existing cart object to remove the line from.");
            return new Result<Cart?>(exception);
        }

        var cartLine = cart.Lines.Find(cl => cl.Product.Id == productId);
        if (cartLine is null)
        {
            var exception = new NotFoundException(nameof(cartLine), "There is no cart line object to remove items from.");
            return new Result<Cart?>(exception);
        }

        cartLine.Quantity -= quantity;
        cartLine.Quantity = cartLine.Quantity <= 0 ? 1 : cartLine.Quantity;
        cartLine.CalculateTotalLinePrice();
        cart.CalculateTotalCartPrice();

        await _session.SetJsonAsync("Cart", cart);
        return cart;
    }

    public async Task<Result<Cart?>> RemoveLine(long productId)
    {
        if (_session is null)
        {
            return null;
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            var exception = new NotFoundException("There is no entity with this ID in the database.");
            return new Result<Cart?>(exception);
        }

        var cart = await _session.GetJsonAsync<Cart>("Cart");
        if (cart is null)
        {
            var exception = new NotFoundException("There is no existing cart object to remove the line from.");
            return new Result<Cart?>(exception);
        }

        cart.Lines.RemoveAll(cl => cl.Product.Id == productId);
        cart.CalculateTotalCartPrice();
        if (cart.Lines.Count > 0)
        {
            await _session.SetJsonAsync("Cart", cart);
            return cart;
        }
        else
        {
            await _session.RemoveAsync("Cart");
            return new();
        }
    }

    public async Task<bool> ClearCart()
    {
        if (_session is null)
        {
            return false;
        }

        await _session.RemoveAsync("Cart");
        return true;
    }
}
