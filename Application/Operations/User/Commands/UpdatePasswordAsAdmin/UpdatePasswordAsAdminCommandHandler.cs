using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public class UpdatePasswordAsAdminCommandHandler(IUserService userService) : ICommandHandler<UpdatePasswordAsAdminCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePasswordAsAdminCommand request, CancellationToken cancellationToken) =>
        await userService.UpdateCustomerPasswordAsAdminAsync(request.UserId, request.NewPassword);
}
