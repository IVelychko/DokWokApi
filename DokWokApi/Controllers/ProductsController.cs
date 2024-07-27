using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.Products.Group)]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
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
        return result.ToCreatedAtActionResult(nameof(GetProductById), "Products");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateProduct(ProductPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _productService.UpdateAsync(model);
        return result.ToOkActionResult();
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
        return result.ToOkIsTakenActionResult();
    }
}
