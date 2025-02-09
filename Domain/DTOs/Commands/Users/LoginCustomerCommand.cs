using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record LoginCustomerCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserResponse>>;
