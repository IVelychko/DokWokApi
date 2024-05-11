using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Post;
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
    public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllProducts()
    {
        try
        {
            var products = await productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ProductModel>> UpdateProduct(ProductModel model)
    {
        try
        {
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("categories")]
    public async Task<ActionResult<ProductCategoryModel>> UpdateCategory(ProductCategoryModel model)
    {
        try
        {
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("categories/{id}")]
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
