using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;

public sealed record IsProductCategoryNameTakenQuery(string Name) : IQuery<Result<IsTakenResponse>>;
