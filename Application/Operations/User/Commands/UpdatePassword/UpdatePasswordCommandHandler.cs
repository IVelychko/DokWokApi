using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(IUserService userService) : ICommandHandler<UpdatePasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
}
