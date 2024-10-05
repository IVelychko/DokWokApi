using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Shop.Queries.IsShopAddressTaken;

public sealed record IsShopAddressTakenQuery(string Street, string Building) : IQuery<Result<IsTakenResponse>>;
