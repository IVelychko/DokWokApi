using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.Shops;

public sealed record IsShopAddressTakenQuery(string Street, string Building) : IQuery<IsTakenResponse>;
