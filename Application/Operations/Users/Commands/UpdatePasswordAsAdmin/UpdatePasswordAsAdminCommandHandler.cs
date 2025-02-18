using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;

namespace Application.Operations.Users.Commands.UpdatePasswordAsAdmin;

public class UpdatePasswordAsAdminCommandHandler(IUserService userService) 
    : ICommandHandler<UpdatePasswordAsAdminCommand>
{
    public async Task Handle(UpdatePasswordAsAdminCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsync(request.UserId, request.NewPassword);
}
