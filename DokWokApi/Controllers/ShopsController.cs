using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Shops;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Shops.Group)]
public class ShopsController : ControllerBase
{
    private readonly IShopService _shopService;

    public ShopsController(IShopService shopService)
    {
        _shopService = shopService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllShops()
    {
        var shops = await _shopService.GetAllAsync();
        return Ok(shops);
    }
    
    [HttpGet(ApiRoutes.Shops.GetById)]
    public async Task<IActionResult> GetShopById(long id)
    {
        var shop = await _shopService.GetByIdAsync(id);
        return Ok(shop);
    }
    
    [HttpGet(ApiRoutes.Shops.GetByAddress)]
    public async Task<IActionResult> GetShopByAddress(string street, string building)
    {
        var shop = await _shopService.GetByAddressAsync(street, building);
        return Ok(shop);
    }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddShop(AddShopRequest request)
    {
        var shop = await _shopService.AddAsync(request);
        return CreatedAtAction(nameof(GetShopById), new { id = shop.Id }, shop);
    }
    
    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateShop(UpdateShopRequest request)
    {
        var shop = await _shopService.UpdateAsync(request);
        return Ok(shop);
    }
    
    [HttpDelete(ApiRoutes.Shops.DeleteById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteShop(long id)
    {
        await _shopService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpGet(ApiRoutes.Shops.IsAddressTaken)]
    public async Task<IActionResult> IsShopAddressTaken(string street, string building)
    {
        var response = await _shopService.IsAddressTakenAsync(street, building);
        return Ok(response);
    }
}