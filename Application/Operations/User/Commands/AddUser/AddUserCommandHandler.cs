using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.AddUser;

public class AddUserCommandHandler(IUserService userService) : ICommandHandler<AddUserCommand, Result<UserModel>>
{
    public async Task<Result<UserModel>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.AddAsync(model, request.Password);
        return result;
    }
}
