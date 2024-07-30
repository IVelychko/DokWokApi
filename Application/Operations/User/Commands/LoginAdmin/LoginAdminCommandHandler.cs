using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginAdmin;

public class LoginAdminCommandHandler(IUserService userService)
    : ICommandHandler<LoginAdminCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.AdminLoginAsync(request.UserName, request.Password);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
