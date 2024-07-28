using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategories;

public class GetAllProductCategoriesQuery() : IQuery<IEnumerable<ProductCategoryModel>>;
