using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Domain.DTOs.Queries.Shops;

public sealed record IsShopAddressTakenQuery(string Street, string Building) : IQuery<Result<IsTakenResponse>>;
