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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShopModel>> GetShopById(long id)
    {
        try
        {
            var shop = await _shopService.GetByIdAsync(id);
            if (shop is null)
            {
                _logger.LogInformation("The shop was not found.");
                return NotFound("The shop was not found.");
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShopModel>> GetShopByAddress(string street, string building)
    {
        try
        {
            var shop = await _shopService.GetByAddressAsync(street, building);
            if (shop is null)
            {
                _logger.LogInformation("The shop was not found.");
                return NotFound("The shop was not found.");
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
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
            return BadRequest();
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
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
            return BadRequest();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(ex, "Not found.");
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation(ex, "Bad request.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
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
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
