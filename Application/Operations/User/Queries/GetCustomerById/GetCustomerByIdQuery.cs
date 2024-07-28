using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(string Id) : IQuery<UserModel?>;
