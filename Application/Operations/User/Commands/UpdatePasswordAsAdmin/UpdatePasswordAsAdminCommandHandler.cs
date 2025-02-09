using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public class UpdatePasswordAsAdminCommandHandler(IUserService userService) : ICommandHandler<UpdatePasswordAsAdminCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdatePasswordAsAdminCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsAdminAsync(request.UserId, request.NewPassword);
}
