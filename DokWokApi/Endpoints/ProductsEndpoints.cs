using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Product;
using DokWokApi.Extensions;
using DokWokApi.Infrastructure;

namespace DokWokApi.Endpoints;

public static class ProductsEndpoints
{
    private const string GetByIdRouteName = nameof(GetProductById);

    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Products.Group);
        group.MapGet("/", GetAllProducts);
        group.MapGet(ApiRoutes.Products.GetById, GetProductById).WithName(GetByIdRouteName);
        group.MapPost("/", AddProduct).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapPut("/", UpdateProduct).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapDelete(ApiRoutes.Products.DeleteById, DeleteProduct).RequireAuthorization(AuthorizationPolicyNames.Admin);
        group.MapGet(ApiRoutes.Products.IsNameTaken, IsProductNameTaken);
    }

    public static async Task<IResult> GetAllProducts(IProductService productService, long? categoryId)
    {
        var products = categoryId.HasValue ?
                await productService.GetAllByCategoryIdAsync(categoryId.Value) :
                await productService.GetAllAsync();

        return Results.Ok(products);
    }

    public static async Task<IResult> GetProductById(IProductService productService, long id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(product);
    }

    public static async Task<IResult> AddProduct(IProductService productService, ProductPostModel postModel)
    {
        var model = postModel.ToModel();
        var result = await productService.AddAsync(model);
        return result.ToCreatedAtRouteResult(GetByIdRouteName);
    }

    public static async Task<IResult> UpdateProduct(IProductService productService, ProductPutModel putModel)
    {
        var model = putModel.ToModel();
        var result = await productService.UpdateAsync(model);
        return result.ToOkResult();
    }

    public static async Task<IResult> DeleteProduct(IProductService productService, long id)
    {
        var result = await productService.DeleteAsync(id);
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

    public static async Task<IResult> IsProductNameTaken(IProductService productService, string name)
    {
        var result = await productService.IsNameTaken(name);
        return result.ToOkIsTakenResult();
    }
}
