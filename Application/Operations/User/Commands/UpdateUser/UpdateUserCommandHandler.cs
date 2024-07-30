using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

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
