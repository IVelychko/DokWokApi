using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;

namespace Application.Operations.Users.Commands.LogOutUser;

public sealed class LogOutUserCommandHandler(IUserService userService) : ICommandHandler<LogOutUserCommand>
{
    public async Task Handle(LogOutUserCommand request, CancellationToken cancellationToken)
    {
        await userService.LogOutAsync(request.RefreshToken);
    }
}
