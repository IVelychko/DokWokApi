using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;

namespace DokWokApi.Endpoints;

public static class ProductCategoriesEndpoints
{
    private const string GetByIdRouteName = nameof(GetCategoryById);

    public static void MapProductCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.ProductCategories.Group);
        group.MapGet("/", GetAllCategories);
        group.MapGet(ApiRoutes.ProductCategories.GetById, GetCategoryById).WithName(GetByIdRouteName);
        group.MapPost("/", AddCategory).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapPut("/", UpdateCategory).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.ProductCategories.DeleteById, DeleteCategory).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.ProductCategories.IsNameTaken, IsCategoryNameTaken);
    }

    public static async Task<IResult> GetAllCategories(IProductCategoryService productCategoryService)
    {
        var categories = await productCategoryService.GetAllAsync();
        return Results.Ok(categories);
    }

    public static async Task<IResult> GetCategoryById(IProductCategoryService productCategoryService, long id)
    {
        var category = await productCategoryService.GetByIdAsync(id);
        if (category is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(category);
    }

    public static async Task<IResult> AddCategory(IProductCategoryService productCategoryService, ProductCategoryPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await productCategoryService.AddAsync(model);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateCategory(IProductCategoryService productCategoryService, ProductCategoryPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await productCategoryService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteCategory(IProductCategoryService productCategoryService, long id)
    {
        var result = await productCategoryService.DeleteAsync(id);
        if (result is null)
        {
            return Results.NotFound();
        }
        else if (result.Value)
        {
            return Results.Ok();
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> IsCategoryNameTaken(IProductCategoryService productCategoryService, string name)
    {
        var result = await productCategoryService.IsNameTaken(name);
        return result.ToOkIsTakenResult();
    }
}
