using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;

public sealed record IsProductCategoryNameTakenQuery(string Name) : IQuery<Result<bool>>;
