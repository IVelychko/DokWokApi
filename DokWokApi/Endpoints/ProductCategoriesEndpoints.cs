using Application.Mapping.Extensions;
using Application.Operations.ProductCategory;
using Application.Operations.ProductCategory.Commands.AddProductCategory;
using Application.Operations.ProductCategory.Commands.DeleteProductCategory;
using Application.Operations.ProductCategory.Commands.UpdateProductCategory;
using Application.Operations.ProductCategory.Queries.GetAllProductCategories;
using Application.Operations.ProductCategory.Queries.GetProductCategoryById;
using Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using MediatR;

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

    public static async Task<IResult> GetAllCategories(ISender sender)
    {
        var categories = await sender.Send(new GetAllProductCategoriesQuery());
        return Results.Ok(categories);
    }

    public static async Task<IResult> GetCategoryById(ISender sender, long id)
    {
        var category = await sender.Send(new GetProductCategoryByIdQuery(id));
        if (category is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(category);
    }

    public static async Task<IResult> AddCategory(ISender sender, AddProductCategoryRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult<ProductCategoryResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateCategory(ISender sender, UpdateProductCategoryRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteCategory(ISender sender, long id)
    {
        var result = await sender.Send(new DeleteProductCategoryCommand(id));
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

    public static async Task<IResult> IsCategoryNameTaken(ISender sender, string name)
    {
        var result = await sender.Send(new IsProductCategoryNameTakenQuery(name));
        return result.ToOkIsTakenResult();
    }
}
