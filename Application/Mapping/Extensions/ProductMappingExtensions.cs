using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses.Products;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class ProductMappingExtensions
{
    public static Product ToEntity(this AddProductRequest command)
    {
        return new Product
        {
            Name = command.Name,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Price = command.Price,
            Weight = command.Weight,
            MeasurementUnit = command.MeasurementUnit,
        };
    }
    
    public static Product ToEntity(this UpdateProductRequest command)
    {
        return new Product
        {
            Id = command.Id,
            Name = command.Name,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Price = command.Price,
            Weight = command.Weight,
            MeasurementUnit = command.MeasurementUnit
        };
    }
    
    public static ProductResponse ToResponse(this Product entity)
    {
        return new ProductResponse
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category?.Name ?? string.Empty,
            Description = entity.Description,
            MeasurementUnit = entity.MeasurementUnit,
            Name = entity.Name,
            Price = entity.Price,
            Weight = entity.Weight
        };
    }
}
