using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.Users;

public sealed record IsUserPhoneNumberTakenQuery(string PhoneNumber) : IQuery<IsTakenResponse>;
