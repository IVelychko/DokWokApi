using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginAdmin;

public class LoginAdminCommandHandler(IUserService userService) : ICommandHandler<LoginAdminCommand, Result<AuthorizedUserModel>>
{
    public async Task<Result<AuthorizedUserModel>> Handle(LoginAdminCommand request, CancellationToken cancellationToken) =>
        await userService.AdminLoginAsync(request.UserName, request.Password);
}
