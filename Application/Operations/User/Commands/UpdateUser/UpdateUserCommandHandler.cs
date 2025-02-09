using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserService userService) : ICommandHandler<UpdateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await userService.UpdateAsync(model);
        return result.Match(u => u.ToResponse(), Result<UserResponse>.Failure);
    }
}
