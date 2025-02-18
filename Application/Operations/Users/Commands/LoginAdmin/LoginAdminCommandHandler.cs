using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.LoginAdmin;

public class LoginAdminCommandHandler(IUserService userService)
    : ICommandHandler<LoginAdminCommand, AuthorizedUserResponse>
{
    public async Task<AuthorizedUserResponse> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        return await userService.LoginAsync(request.UserName);
    }
}
