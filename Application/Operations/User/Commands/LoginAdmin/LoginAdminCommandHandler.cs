using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.LoginAdmin;

public class LoginAdminCommandHandler(IUserService userService)
    : ICommandHandler<LoginAdminCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(request.UserName, request.Password);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
