using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Queries.IsUserNameTaken;

public sealed record IsUserNameTakenQuery(string UserName) : IQuery<Result<IsTakenResponse>>;
