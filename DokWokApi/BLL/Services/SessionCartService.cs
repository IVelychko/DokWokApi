using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.Exceptions;
using DokWokApi.Extensions;

namespace DokWokApi.BLL.Services;

public class SessionCartService : ICartService
{
    private readonly IProductService _productService;

    private readonly ISession? _session;

    public SessionCartService(IProductService productService, IHttpContextAccessor httpContextAccessor)
    {
        _productService = productService;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<Cart> GetCart()
    {
        if (_session is null)
        {
            throw new CartException(nameof(_session), "There is no session available.");
        }

        return await _session.GetJsonAsync<Cart>("Cart") ?? new Cart();
    }

    public async Task<Cart> AddItem(long productId, int quantity)
    {
        if (_session is null)
        {
            throw new CartException(nameof(_session), "There is no session available.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("The quantity value must be greater than 0", nameof(quantity));
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
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

    public async Task<Cart> RemoveItem(long productId, int quantity)
    {
        if (_session is null)
        {
            throw new CartException(nameof(_session), "There is no session available.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("The quantity value must be greater than 0", nameof(quantity));
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
        }

        var cart = await _session.GetJsonAsync<Cart>("Cart");
        if (cart is null)
        {
            throw new CartNotFoundException(nameof(cart), "There is no existing cart object to remove the line from.");
        }

        var cartLine = cart.Lines.Find(cl => cl.Product.Id == productId);
        if (cartLine is null)
        {
            throw new CartNotFoundException(nameof(cartLine), "There is no cart line object to remove items from.");
        }

        cartLine.Quantity -= quantity;
        cartLine.Quantity = cartLine.Quantity <= 0 ? 1 : cartLine.Quantity;
        cartLine.CalculateTotalLinePrice();
        cart.CalculateTotalCartPrice();

        await _session.SetJsonAsync("Cart", cart);
        return cart;
    }

    public async Task<Cart> RemoveLine(long productId)
    {
        if (_session is null)
        {
            throw new CartException(nameof(_session), "There is no session available.");
        }

        var product = await _productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
        }

        var cart = await _session.GetJsonAsync<Cart>("Cart");
        if (cart is null)
        {
            throw new CartNotFoundException(nameof(cart), "There is no existing cart object to remove the line from.");
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

    public async Task ClearCart()
    {
        if (_session is null)
        {
            throw new CartException(nameof(_session), "There is no session available.");
        }

        await _session.RemoveAsync("Cart");
    }
}
