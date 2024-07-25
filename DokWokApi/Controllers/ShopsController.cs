using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
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
        if (shop is null)
        {
            return NotFound();
        }

        return Ok(shop);
    }

    [HttpGet(ApiRoutes.Shops.GetByAddress)]
    public async Task<IActionResult> GetShopByAddress(string street, string building)
    {
        var shop = await _shopService.GetByAddressAsync(street, building);
        if (shop is null)
        {
            return NotFound();
        }

        return Ok(shop);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> AddShop(ShopPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _shopService.AddAsync(model);
        return result.ToCreatedAtActionActionResult(nameof(GetShopById), "Shops");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateShop(ShopPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _shopService.UpdateAsync(model);
        return result.ToOkActionResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Shops.DeleteById)]
    public async Task<IActionResult> DeleteShop(long id)
    {
        var result = await _shopService.DeleteAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet(ApiRoutes.Shops.IsAddressTaken)]
    public async Task<IActionResult> IsShopAddressTaken(string street, string building)
    {
        var result = await _shopService.IsAddressTaken(street, building);
        return result.ToOkIsTakenActionResult();
    }
}
