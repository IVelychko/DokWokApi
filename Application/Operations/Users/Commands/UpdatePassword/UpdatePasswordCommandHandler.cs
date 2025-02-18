using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;

namespace Application.Operations.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(IUserService userService) 
    : ICommandHandler<UpdatePasswordCommand>
{
    public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsync(request.UserId, request.NewPassword);
}
