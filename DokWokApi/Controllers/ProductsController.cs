using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    private readonly IProductCategoryService _categoryService;

    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, IProductCategoryService categoryService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllProducts(long? categoryId)
    {
        try
        {
            var products = categoryId.HasValue ? 
                await _productService.GetAllByCategoryIdAsync(categoryId.Value) : 
                await _productService.GetAllAsync();

            return Ok(products);
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
    public async Task<ActionResult<ProductModel>> GetProductById(long id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null)
            {
                _logger.LogInformation("The product was not found.");
                return NotFound("The product was not found.");
            }

            return Ok(product);
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
    public async Task<ActionResult<ProductModel>> AddProduct(ProductPostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductModel>(postModel);
            var addedModel = await _productService.AddAsync(model);
            return CreatedAtAction(nameof(GetProductById), new { id = addedModel.Id }, addedModel);
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
    public async Task<ActionResult<ProductModel>> UpdateProduct(ProductPutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductModel>(putModel);
            var updatedModel = await _productService.UpdateAsync(model);
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
    public async Task<ActionResult> DeleteProduct(long id)
    {
        try
        {
            await _productService.DeleteAsync(id);
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


    [HttpGet("isNameTaken/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IsProductNameTaken(string name)
    {
        try
        {
            var isTaken = await _productService.IsNameTaken(name);
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

    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetAllCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCategoryModel>> GetCategoryById(long id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
            {
                _logger.LogInformation("The product category was not found.");
                return NotFound("The product category was not found.");
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost("categories")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCategoryModel>> AddCategory(ProductCategoryPostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductCategoryModel>(postModel);
            var addedModel = await _categoryService.AddAsync(model);
            return CreatedAtAction(nameof(GetCategoryById), new { id = addedModel.Id }, addedModel);
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
    [HttpPut("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCategoryModel>> UpdateCategory(ProductCategoryPutModel putModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductCategoryModel>(putModel);
            var updatedModel = await _categoryService.UpdateAsync(model);
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
    [HttpDelete("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCategory(long id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
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

    [HttpGet("categories/isNameTaken/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> IsCategoryNameTaken(string name)
    {
        try
        {
            var isTaken = await _categoryService.IsNameTaken(name);
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
