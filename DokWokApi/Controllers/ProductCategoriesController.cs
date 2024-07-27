using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Controllers;

[ApiController]
[Route(ApiRoutes.ProductCategories.Group)]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoriesController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
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
    [HttpPost]
    public async Task<IActionResult> AddCategory(ProductCategoryPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await _categoryService.AddAsync(model);
        return result.ToCreatedAtActionResult(nameof(GetCategoryById), "Products");
    }

    [Authorize(Roles = $"{UserRoles.Admin}")]
    [HttpPut]
    public async Task<IActionResult> UpdateCategory(ProductCategoryPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await _categoryService.UpdateAsync(model);
        return result.ToOkActionResult();
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
        return result.ToOkIsTakenActionResult();
    }
}
