using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserService userService) : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken) =>
        await userService.DeleteAsync(request.Id);
}
