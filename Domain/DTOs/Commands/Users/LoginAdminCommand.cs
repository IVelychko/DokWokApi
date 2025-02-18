using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Commands.Users;

public sealed record LoginAdminCommand(string UserName, string Password) 
    : ICommand<AuthorizedUserResponse>;
