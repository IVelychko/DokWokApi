using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserNameTaken;

public sealed record IsUserNameTakenQuery(string UserName) : IQuery<Result<bool>>;
