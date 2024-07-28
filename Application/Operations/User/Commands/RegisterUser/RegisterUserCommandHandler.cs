using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.RegisterUser;

public class RegisterUserCommandHandler(IUserService userService) : ICommandHandler<RegisterUserCommand, Result<AuthorizedUserModel>>
{
    public async Task<Result<AuthorizedUserModel>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.RegisterAsync(model, request.Password);
        return result;
    }
}
