using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL.Extensions;

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

    public static ProductCategoryModel ToModel(this ProductCategoryPostModel model)
    {
        return new()
        {
            Name = model.Name!
        };
    }

    public static ProductCategoryModel ToModel(this ProductCategoryPutModel model)
    {
        return new()
        {
            Id = model.Id!.Value,
            Name = model.Name!
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
