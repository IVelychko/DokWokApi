using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Helpers;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(IUserService userService) : ICommandHandler<UpdatePasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
}
