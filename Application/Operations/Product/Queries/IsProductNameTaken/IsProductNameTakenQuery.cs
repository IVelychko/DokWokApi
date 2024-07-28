using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.Product.Queries.IsProductNameTaken;

public sealed record IsProductNameTakenQuery(string Name) : IQuery<Result<bool>>;
