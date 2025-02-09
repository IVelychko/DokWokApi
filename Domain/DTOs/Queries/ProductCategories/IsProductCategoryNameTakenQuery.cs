using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Domain.DTOs.Queries.ProductCategories;

public sealed record IsProductCategoryNameTakenQuery(string Name) : IQuery<Result<IsTakenResponse>>;
