using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Product.Queries.IsProductNameTaken;

public sealed record IsProductNameTakenQuery(string Name) : IQuery<Result<IsTakenResponse>>;
