using Domain.Entities;
using Domain.Models;

namespace Domain.Mapping.Extensions;

public static class ProductMappingExtensions
{
    public static ProductModel ToModel(this Product entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CategoryId = entity.CategoryId,
            Description = entity.Description,
            Price = entity.Price,
            Weight = entity.Weight,
            MeasurementUnit = entity.MeasurementUnit,
            CategoryName = entity.Category is not null ? entity.Category.Name : string.Empty
        };
    }

    public static Product ToEntity(this ProductModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            CategoryId = model.CategoryId,
            Description = model.Description,
            Price = model.Price,
            Weight = model.Weight,
            MeasurementUnit = model.MeasurementUnit
        };
    }
}
