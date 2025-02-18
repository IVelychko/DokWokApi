using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.Products;

public sealed record IsProductNameTakenQuery(string Name) : IQuery<IsTakenResponse>;
