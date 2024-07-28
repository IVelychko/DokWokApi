using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Queries.IsUserPhoneNumberTaken;

public sealed record IsUserPhoneNumberTakenQuery(string PhoneNumber) : IQuery<Result<bool>>;
