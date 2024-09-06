using Application.Mapping.Extensions;
using Application.Operations;
using Application.Operations.ProductCategory;
using Application.Operations.ProductCategory.Commands.AddProductCategory;
using Application.Operations.ProductCategory.Commands.DeleteProductCategory;
using Application.Operations.ProductCategory.Commands.UpdateProductCategory;
using Application.Operations.ProductCategory.Queries.GetAllProductCategories;
using Application.Operations.ProductCategory.Queries.GetAllProductCategoriesByPage;
using Application.Operations.ProductCategory.Queries.GetProductCategoryById;
using Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DokWokApi.Endpoints;

public static class ProductCategoriesEndpoints
{
    private const string GetByIdRouteName = nameof(GetCategoryById);

    public static void AddCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.ProductCategories.Group).WithTags("ProductCategories");

        group.MapGet("/", GetAllCategories);

        group.MapGet(ApiRoutes.ProductCategories.GetById, GetCategoryById)
            .WithName(GetByIdRouteName)
            .Produces<ProductCategoryResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddCategory)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ProductCategoryResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/", UpdateCategory)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ProductCategoryResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete(ApiRoutes.ProductCategories.DeleteById, DeleteCategory)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.ProductCategories.IsNameTaken, IsCategoryNameTaken)
            .Produces<IsTakenResponse>()
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<Ok<IEnumerable<ProductCategoryResponse>>> GetAllCategories(ISender sender,
        int? pageNumber, int? pageSize)
    {
        var categories = pageNumber.HasValue && pageSize.HasValue ?
            await sender.Send(new GetAllProductCategoriesByPageQuery(pageNumber.Value, pageSize.Value)) :
            await sender.Send(new GetAllProductCategoriesQuery());

        return TypedResults.Ok(categories);
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
