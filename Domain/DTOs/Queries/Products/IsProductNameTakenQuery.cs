using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Domain.DTOs.Queries.Products;

public sealed record IsProductNameTakenQuery(string Name) : IQuery<Result<IsTakenResponse>>;
