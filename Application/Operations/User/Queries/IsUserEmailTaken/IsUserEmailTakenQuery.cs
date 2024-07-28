using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserEmailTaken;

public sealed record IsUserEmailTakenQuery(string Email) : IQuery<Result<bool>>;
