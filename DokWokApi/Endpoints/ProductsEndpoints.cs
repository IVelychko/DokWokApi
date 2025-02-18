using Application.Mapping.Extensions;
using DokWokApi.Helpers;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Products;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DokWokApi.Endpoints;

public static class ProductsEndpoints
{
    private const string GetByIdRouteName = nameof(GetProductById);

    public static void AddProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Products.Group).WithTags("Products");

        group.MapGet("/", GetAllProducts);

        group.MapGet(ApiRoutes.Products.GetById, GetProductById)
            .WithName(GetByIdRouteName)
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/", UpdateProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces<ProductResponse>()
            .Produces<ProblemDetailsModel>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetailsModel>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete(ApiRoutes.Products.DeleteById, DeleteProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet(ApiRoutes.Products.IsNameTaken, IsProductNameTaken)
            .Produces<IsTakenResponse>()
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<Ok<IEnumerable<ProductResponse>>> GetAllProducts(ISender sender,
        long? categoryId, int? pageNumber, int? pageSize)
    {
        IEnumerable<ProductResponse> products;
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            products = categoryId.HasValue ?
                await sender.Send(new GetAllProductsByCategoryIdAndPageQuery(categoryId.Value, pageNumber.Value, pageSize.Value)) :
                await sender.Send(new GetAllProductsByPageQuery(pageNumber.Value, pageSize.Value));
        }
        else
        {
            products = categoryId.HasValue ?
                await sender.Send(new GetAllProductsByCategoryIdQuery(categoryId.Value)) :
                await sender.Send(new GetAllProductsQuery());
        }

        return TypedResults.Ok(products);
    }

    public static async Task<IResult> GetProductById(ISender sender, long id)
    {
        var product = await sender.Send(new GetProductByIdQuery(id));
        if (product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(product);
    }

    public static async Task<IResult> AddProduct(ISender sender, AddProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateProduct(ISender sender, UpdateProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteProduct(ISender sender, long id)
    {
        await sender.Send(new DeleteProductCommand(id));
        return Results.Ok();
    }

    public static async Task<IResult> IsProductNameTaken(ISender sender, string name)
    {
        var result = await sender.Send(new IsProductNameTakenQuery(name));
        return result.ToOkResult();
    }
}
