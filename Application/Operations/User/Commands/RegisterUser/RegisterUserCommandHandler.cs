using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

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
