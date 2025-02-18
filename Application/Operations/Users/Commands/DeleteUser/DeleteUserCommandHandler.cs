using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;

namespace Application.Operations.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserService userService) : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken) =>
        await userService.DeleteAsync(request.Id);
}
