using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Queries.IsUserPhoneNumberTaken;

public sealed record IsUserPhoneNumberTakenQuery(string PhoneNumber) : IQuery<Result<IsTakenResponse>>;
