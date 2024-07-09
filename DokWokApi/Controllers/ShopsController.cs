using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/shops")]
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
    public async Task<ActionResult<IEnumerable<ShopModel>>> GetAllShops()
    {
        try
        {
            var shops = await _shopService.GetAllAsync();
            return Ok(shops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShopModel>> GetShopById(long id)
    {
        try
        {
            var shop = await _shopService.GetByIdAsync(id);
            if (shop is null)
            {
                _logger.LogInformation("The shop was not found.");
                return NotFound();
            }

            return Ok(shop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{street}/{building}")]
    public async Task<ActionResult<ShopModel>> GetShopByAddress(string street, string building)
    {
        try
        {
            var shop = await _shopService.GetByAddressAsync(street, building);
            if (shop is null)
            {
                _logger.LogInformation("The shop was not found.");
                return NotFound();
            }

            return Ok(shop);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<ActionResult<ShopModel>> AddShop(ShopPostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ShopModel>(postModel);
            var addedModel = await _shopService.AddAsync(model);
            return CreatedAtAction(nameof(GetShopById), new { id = addedModel.Id }, addedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest("The passed data is null");
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<ActionResult<ShopModel>> UpdateShop(ShopPutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ShopModel>(putModel);
            var updatedModel = await _shopService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest("The passed data is null");
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShop(long id)
    {
        try
        {
            await _shopService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("isAddressTaken/{street}/{building}")]
    public async Task<ActionResult> IsShopAddressTaken(string street, string building)
    {
        try
        {
            var isTaken = await _shopService.IsAddressTaken(street, building);
            return new JsonResult(new { isTaken }) { StatusCode = StatusCodes.Status200OK };
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest("The passed data is null");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
