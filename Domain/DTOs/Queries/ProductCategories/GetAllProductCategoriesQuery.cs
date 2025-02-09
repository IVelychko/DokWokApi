using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.DTOs.Queries.ProductCategories;

public class GetAllProductCategoriesQuery() : IQuery<IEnumerable<ProductCategoryResponse>>;
