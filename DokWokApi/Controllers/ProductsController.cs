using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Products;
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
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet(ApiRoutes.Products.GetById)]
    public async Task<IActionResult> GetProductById(long id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddProduct(AddProductRequest request)
    {
        var product = await _productService.AddAsync(request);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }
    
    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateProduct(UpdateProductRequest request)
    {
        var product = await _productService.UpdateAsync(request);
        return Ok(product);
    }
    
    [HttpDelete(ApiRoutes.Products.DeleteById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        await _productService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpGet(ApiRoutes.Products.IsNameTaken)]
    public async Task<IActionResult> IsProductNameTaken(string name)
    {
        var response = await _productService.IsNameTakenAsync(name);
        return Ok(response);
    }
}