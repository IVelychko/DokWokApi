using Domain.Entities;
using Infrastructure.Validation.ProductCategories.Add;
using Infrastructure.Validation.ProductCategories.Update;

namespace Infrastructure.Mapping.Extensions;

public static class ProductCategoryMappingExtensions
{
    public static AddProductCategoryValidationModel ToAddValidationModel(this ProductCategory productCategory) =>
        new(productCategory.Name);

    public static UpdateProductCategoryValidationModel ToUpdateValidationModel(this ProductCategory productCategory) =>
        new(productCategory.Id, productCategory.Name);
}
