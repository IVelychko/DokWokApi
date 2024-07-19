using DokWokApi.BLL;
using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Products.Controller)]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductCategoryService _categoryService;

    public ProductsController(IProductService productService, IProductCategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(long? categoryId)
    {
        var products = categoryId.HasValue ?
                await _productService.GetAllByCategoryIdAsync(categoryId.Value) :
                await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet(ApiRoutes.Products.GetById)]
    public async Task<IActionResult> GetProductById(long id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _productService.AddAsync(model);
        return result.ToCreatedAtActionResult(nameof(GetProductById), nameof(ProductsController));
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateProduct(ProductPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _productService.UpdateAsync(model);
        return result.ToOkResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.Products.DeleteById)]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var result = await _productService.DeleteAsync(id);
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


    [HttpGet(ApiRoutes.Products.IsNameTaken)]
    public async Task<IActionResult> IsProductNameTaken(string name)
    {
        var result = await _productService.IsNameTaken(name);
        return result.ToOkIsTakenResult();
    }

    [HttpGet(ApiRoutes.ProductCategories.GetAll)]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet(ApiRoutes.ProductCategories.GetById)]
    public async Task<IActionResult> GetCategoryById(long id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPost(ApiRoutes.ProductCategories.Add)]
    public async Task<IActionResult> AddCategory(ProductCategoryPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _categoryService.AddAsync(model);
        return result.ToCreatedAtActionResult(nameof(GetCategoryById), nameof(ProductsController));
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut(ApiRoutes.ProductCategories.Update)]
    public async Task<IActionResult> UpdateCategory(ProductCategoryPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _categoryService.UpdateAsync(model);
        return result.ToOkResult();
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpDelete(ApiRoutes.ProductCategories.DeleteById)]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        var result = await _categoryService.DeleteAsync(id);
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

    [HttpGet(ApiRoutes.ProductCategories.IsNameTaken)]
    public async Task<IActionResult> IsCategoryNameTaken(string name)
    {
        var result = await _categoryService.IsNameTaken(name);
        return result.ToOkIsTakenResult();
    }
}
