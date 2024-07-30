using Application.Mapping.Extensions;
using Application.Operations.Product;
using Application.Operations.Product.Commands.AddProduct;
using Application.Operations.Product.Commands.DeleteProduct;
using Application.Operations.Product.Commands.UpdateProduct;
using Application.Operations.Product.Queries.GetAllProducts;
using Application.Operations.Product.Queries.GetAllProductsByCategoryId;
using Application.Operations.Product.Queries.GetProductById;
using Application.Operations.Product.Queries.IsProductNameTaken;
using DokWokApi.Extensions;
using DokWokApi.Helpers;
using MediatR;

namespace DokWokApi.Endpoints;

public static class ProductsEndpoints
{
    private const string GetByIdRouteName = nameof(GetProductById);

    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Products.Group);
        group.MapGet("/", GetAllProducts);
        group.MapGet(ApiRoutes.Products.GetById, GetProductById).WithName(GetByIdRouteName);
        group.MapPost("/", AddProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapPut("/", UpdateProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.Products.DeleteById, DeleteProduct)
            .RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.Products.IsNameTaken, IsProductNameTaken);
    }

    public static async Task<IResult> GetAllProducts(ISender sender, long? categoryId)
    {
        var products = categoryId.HasValue ?
                await sender.Send(new GetAllProductsByCategoryIdQuery(categoryId.Value)) :
                await sender.Send(new GetAllProductsQuery());

        return Results.Ok(products);
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
        return result.ToCreatedAtRouteResult<ProductResponse, long>(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateProduct(ISender sender, UpdateProductRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteProduct(ISender sender, long id)
    {
        var result = await sender.Send(new DeleteProductCommand(id));
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

    public static async Task<IResult> IsProductNameTaken(ISender sender, string name)
    {
        var result = await sender.Send(new IsProductNameTakenQuery(name));
        return result.ToOkIsTakenResult();
    }
}
