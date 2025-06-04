using DokWokApi.Constants;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.ProductCategories.Group)]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _productCategoryService;
    private readonly IProductService _productService;

    public ProductCategoriesController(
        IProductCategoryService productCategoryService,
        IProductService productService)
    {
        _productCategoryService = productCategoryService;
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _productCategoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet(ApiRoutes.ProductCategories.GetAllProductsByCategoryId)]
    public async Task<IActionResult> GetAllProductsByCategoryId(long categoryId)
    {
        var products = await _productService.GetAllByCategoryIdAsync(categoryId);
        return Ok(products);
    }
    
    [HttpGet(ApiRoutes.ProductCategories.GetById)]
    public async Task<IActionResult> GetCategoryById(long id)
    {
        var category = await _productCategoryService.GetByIdAsync(id);
        return Ok(category);
    }
    
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddCategory(AddProductCategoryRequest request)
    {
        var category = await _productCategoryService.AddAsync(request);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }
    
    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdateCategory(UpdateProductCategoryRequest request)
    {
        var category = await _productCategoryService.UpdateAsync(request);
        return Ok(category);
    }
    
    [HttpDelete(ApiRoutes.ProductCategories.DeleteById)]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        await _productCategoryService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpGet(ApiRoutes.ProductCategories.IsNameTaken)]
    public async Task<IActionResult> IsCategoryNameTaken(string name)
    {
        var response = await _productCategoryService.IsNameTakenAsync(name);
        return Ok(response);
    }
}