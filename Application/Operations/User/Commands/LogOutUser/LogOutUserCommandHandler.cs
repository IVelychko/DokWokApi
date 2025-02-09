using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;

namespace Application.Operations.User.Commands.LogOutUser;

public sealed class LogOutUserCommandHandler(IUserService userService) : ICommandHandler<LogOutUserCommand, bool>
{
    public async Task<bool> Handle(LogOutUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.LogOutAsync(request.RefreshToken);
    }
}
