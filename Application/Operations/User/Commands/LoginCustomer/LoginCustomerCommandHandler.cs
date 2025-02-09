using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.LoginCustomer;

public class LoginCustomerCommandHandler(IUserService userService) : ICommandHandler<LoginCustomerCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(request.UserName, request.Password);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
