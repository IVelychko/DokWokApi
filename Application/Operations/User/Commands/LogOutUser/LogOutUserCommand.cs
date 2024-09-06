using Application.Abstractions.Messaging;

namespace Application.Operations.User.Commands.LogOutUser;

public sealed record LogOutUserCommand(string RefreshToken) : ICommand<bool>;
