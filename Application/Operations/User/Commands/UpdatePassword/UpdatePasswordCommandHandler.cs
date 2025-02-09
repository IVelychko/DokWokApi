using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(IUserService userService) : ICommandHandler<UpdatePasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
}
