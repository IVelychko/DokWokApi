using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserService userService) : ICommandHandler<DeleteUserCommand, bool?>
{
    public async Task<bool?> Handle(DeleteUserCommand request, CancellationToken cancellationToken) =>
        await userService.DeleteAsync(request.Id);
}
