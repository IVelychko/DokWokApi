using Application.Abstractions.Messaging;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategories;

public class GetAllProductCategoriesQuery() : IQuery<IEnumerable<ProductCategoryResponse>>;
