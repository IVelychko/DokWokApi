using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserService userService) : ICommandHandler<UpdateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.UpdateAsync(request);
    }
}
