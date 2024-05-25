using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;

    private readonly IProductCategoryService categoryService;

    public ProductsController(IProductService productService, IProductCategoryService categoryService)
    {
        this.productService = productService;
        this.categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllProducts(long? categoryId)
    {
        try
        {
            var products = categoryId.HasValue ? 
                await productService.GetAllByCategoryIdAsync(categoryId.Value) : 
                await productService.GetAllAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            var product = await productService.GetByIdAsync(id);
            if (product is null)
            {
                return NotFound("The product was not found.");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductModel>> AddProduct(ProductPostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductModel>(postModel);
            var addedModel = await productService.AddAsync(model);
            return Ok(addedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

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
            var updatedModel = await productService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteProduct(long id)
    {
        try
        {
            await productService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetAllCategories()
    {
        try
        {
            var categories = await categoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            var category = await categoryService.GetByIdAsync(id);
            if (category is null)
            {
                return NotFound("The product category was not found.");
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCategoryModel>> AddCategory(ProductCategoryPostModel postModel, [FromServices] IMapper mapper)
    {
        try
        {
            var model = mapper.Map<ProductCategoryModel>(postModel);
            var addedModel = await categoryService.AddAsync(model);
            return Ok(addedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

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
            var updatedModel = await categoryService.UpdateAsync(model);
            return Ok(updatedModel);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCategory(long id)
    {
        try
        {
            await categoryService.DeleteAsync(id);
            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
