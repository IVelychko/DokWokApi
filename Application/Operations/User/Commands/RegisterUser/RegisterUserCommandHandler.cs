using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.RegisterUser;

public class RegisterUserCommandHandler(IUserService userService) : ICommandHandler<RegisterUserCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.RegisterAsync(model, request.Password);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
