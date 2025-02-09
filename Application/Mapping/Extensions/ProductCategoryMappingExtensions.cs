using Application.Operations.ProductCategory;
using Application.Operations.ProductCategory.Commands.AddProductCategory;
using Application.Operations.ProductCategory.Commands.UpdateProductCategory;
using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Requests.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Entities;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class ProductCategoryMappingExtensions
{
    public static AddProductCategoryCommand ToCommand(this AddProductCategoryRequest request) =>
        new(request.Name);

    public static UpdateProductCategoryCommand ToCommand(this UpdateProductCategoryRequest request) =>
        new(request.Id, request.Name);

    // public static ProductCategoryModel ToModel(this AddProductCategoryCommand command)
    // {
    //     return new()
    //     {
    //         Name = command.Name
    //     };
    // }
    
    public static ProductCategory ToEntity(this AddProductCategoryCommand command)
    {
        return new ProductCategory
        {
            Name = command.Name
        };
    }

    // public static ProductCategoryModel ToModel(this UpdateProductCategoryCommand command)
    // {
    //     return new()
    //     {
    //         Id = command.Id,
    //         Name = command.Name
    //     };
    // }
    
    public static ProductCategory ToEntity(this UpdateProductCategoryCommand command)
    {
        return new ProductCategory
        {
            Id = command.Id,
            Name = command.Name
        };
    }

    // public static ProductCategoryResponse ToResponse(this ProductCategoryModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         Name = model.Name
    //     };
    // }
    
    public static ProductCategoryResponse ToResponse(this ProductCategory entity)
    {
        return new ProductCategoryResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}
