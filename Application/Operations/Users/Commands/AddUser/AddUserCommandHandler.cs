using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.AddUser;

public class AddUserCommandHandler(IUserService userService) : ICommandHandler<AddUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.AddAsync(request);
    }
}
