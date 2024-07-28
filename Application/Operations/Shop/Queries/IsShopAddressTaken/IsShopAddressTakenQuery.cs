using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.Shop.Queries.IsShopAddressTaken;

public sealed record IsShopAddressTakenQuery(string Street, string Building) : IQuery<Result<bool>>;
