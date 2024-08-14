using Domain.Entities;
using Infrastructure.Validation.Products.Add;
using Infrastructure.Validation.Products.Update;

namespace Infrastructure.Mapping.Extensions;

public static class ProductMappingExtensions
{
    public static AddProductValidationModel ToAddValidationModel(this Product product) =>
        new(product.Name, product.Price, product.Weight, product.MeasurementUnit, product.Description, product.CategoryId);

    public static UpdateProductValidationModel ToUpdateValidationModel(this Product product) =>
        new(product.Id, product.Name, product.Price, product.Weight, product.MeasurementUnit, product.Description, product.CategoryId);
}
