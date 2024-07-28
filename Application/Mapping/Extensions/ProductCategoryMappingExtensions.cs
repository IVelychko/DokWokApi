using Application.Operations.ProductCategory.Commands.AddProductCategory;
using Application.Operations.ProductCategory.Commands.UpdateProductCategory;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class ProductCategoryMappingExtensions
{
    public static AddProductCategoryCommand ToCommand(this AddProductCategoryRequest request) =>
        new(request.Name);

    public static UpdateProductCategoryCommand ToCommand(this UpdateProductCategoryRequest request) =>
        new(request.Id, request.Name);

    public static ProductCategoryModel ToModel(this AddProductCategoryCommand command)
    {
        return new()
        {
            Name = command.Name
        };
    }

    public static ProductCategoryModel ToModel(this UpdateProductCategoryCommand command)
    {
        return new()
        {
            Id = command.Id,
            Name = command.Name
        };
    }
}
