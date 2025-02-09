using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.AddUser;

public class AddUserCommandHandler(IUserService userService) : ICommandHandler<AddUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.AddAsync(model, request.Password);
        return result.Match(u => u.ToResponse(), Result<UserResponse>.Failure);
    }
}
