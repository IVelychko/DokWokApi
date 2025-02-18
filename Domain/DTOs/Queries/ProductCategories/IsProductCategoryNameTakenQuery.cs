using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.ProductCategories;

public sealed record IsProductCategoryNameTakenQuery(string Name) : IQuery<IsTakenResponse>;
