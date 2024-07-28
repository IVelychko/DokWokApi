using Domain.Entities;
using Domain.Models;

namespace Domain.Mapping.Extensions;

public static class ProductCategoryMappingExtensions
{
    public static ProductCategoryModel ToModel(this ProductCategory entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public static ProductCategory ToEntity(this ProductCategoryModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name
        };
    }
}
