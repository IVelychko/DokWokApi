using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Queries.IsUserEmailTaken;

public sealed record IsUserEmailTakenQuery(string Email) : IQuery<Result<IsTakenResponse>>;
