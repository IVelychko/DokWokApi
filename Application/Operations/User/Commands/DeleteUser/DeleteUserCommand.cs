using Application.Abstractions.Messaging;

namespace Application.Operations.User.Commands.DeleteUser;

public sealed record DeleteUserCommand(long Id) : ICommand<bool?>;
