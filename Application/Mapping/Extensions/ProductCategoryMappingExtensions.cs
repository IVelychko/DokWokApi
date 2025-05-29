using Domain.DTOs.Requests.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class ProductCategoryMappingExtensions
{
    public static ProductCategory ToEntity(this AddProductCategoryRequest request)
    {
        return new ProductCategory
        {
            Name = request.Name
        };
    }
    
    public static ProductCategory ToEntity(this UpdateProductCategoryRequest command)
    {
        return new ProductCategory
        {
            Id = command.Id,
            Name = command.Name
        };
    }
    
    public static ProductCategoryResponse ToResponse(this ProductCategory entity)
    {
        return new ProductCategoryResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}
