using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Shops.Controller)]
public class ShopsController : ControllerBase
{
    private readonly IShopService _shopService;
    private readonly ILogger<ShopsController> _logger;

    public ShopsController(IShopService shopService, ILogger<ShopsController> logger)
    {
        _shopService = shopService;
        _logger = logger;
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
            _logger.LogInformation("The shop was not found.");
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
            _logger.LogInformation("The shop was not found.");
            return NotFound();
        }

        return Ok(shop);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> AddShop(ShopPostModel postModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<ShopModel>(postModel);
        var result = await _shopService.AddAsync(model);
        return result.ToCreatedAtAction(_logger, nameof(GetShopById), nameof(ShopsController));
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateShop(ShopPutModel putModel, [FromServices] IMapper mapper)
    {
        var model = mapper.Map<ShopModel>(putModel);
        var result = await _shopService.UpdateAsync(model);
        return result.ToOk(_logger);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Shops.DeleteById)]
    public async Task<IActionResult> DeleteShop(long id)
    {
        var result = await _shopService.DeleteAsync(id);
        if (result is null)
        {
            _logger.LogInformation("Not found");
            return NotFound();
        }
        else if (result.Value)
        {
            return Ok();
        }

        _logger.LogError("Server error");
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet(ApiRoutes.Shops.IsAddressTaken)]
    public async Task<IActionResult> IsShopAddressTaken(string street, string building)
    {
        var result = await _shopService.IsAddressTaken(street, building);
        return result.ToOkIsTaken(_logger);
    }
}
