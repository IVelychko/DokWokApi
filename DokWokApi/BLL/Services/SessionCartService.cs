﻿using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.Exceptions;
using DokWokApi.Extensions;

namespace DokWokApi.BLL.Services;

public class SessionCartService : ICartService
{
    private readonly IProductService productService;

    private readonly ISession? session;

    public SessionCartService(IProductService productService, IHttpContextAccessor httpContextAccessor)
    {
        this.productService = productService;
        session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<Cart> GetCart()
    {
        if (session is null)
        {
            throw new CartException(nameof(session), "There is no session available.");
        }

        return await session.GetJsonAsync<Cart>("Cart") ?? new Cart();
    }

    public async Task<Cart> AddItem(long productId, int quantity)
    {
        if (session is null)
        {
            throw new CartException(nameof(session), "There is no session available.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("The quantity value must be greater than 0", nameof(quantity));
        }

        var product = await productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
        }

        var cart = await session.GetJsonAsync<Cart>("Cart") ?? new Cart();

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

        await session.SetJsonAsync("Cart", cart);
        return cart;
    }

    public async Task<Cart> RemoveItem(long productId, int quantity)
    {
        if (session is null)
        {
            throw new CartException(nameof(session), "There is no session available.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("The quantity value must be greater than 0", nameof(quantity));
        }

        var product = await productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
        }

        var cart = await session.GetJsonAsync<Cart>("Cart");
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

        await session.SetJsonAsync("Cart", cart);
        return cart;
    }

    public async Task<Cart> RemoveLine(long productId)
    {
        if (session is null)
        {
            throw new CartException(nameof(session), "There is no session available.");
        }

        var product = await productService.GetByIdAsync(productId);
        if (product is null)
        {
            throw new EntityNotFoundException(nameof(product), "There is no entity with this ID in the database.");
        }

        var cart = await session.GetJsonAsync<Cart>("Cart");
        if (cart is null)
        {
            throw new CartNotFoundException(nameof(cart), "There is no existing cart object to remove the line from.");
        }

        cart.Lines.RemoveAll(cl => cl.Product.Id == productId);
        cart.CalculateTotalCartPrice();
        if (cart.Lines.Count > 0)
        {
            await session.SetJsonAsync("Cart", cart);
            return cart;
        }
        else
        {
            await session.RemoveAsync("Cart");
            return new();
        }
    }

    public async Task ClearCart()
    {
        if (session is null)
        {
            throw new CartException(nameof(session), "There is no session available.");
        }

        await session.RemoveAsync("Cart");
    }
}
