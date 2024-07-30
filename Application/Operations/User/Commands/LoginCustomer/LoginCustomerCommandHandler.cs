using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginCustomer;

public class LoginCustomerCommandHandler(IUserService userService) : ICommandHandler<LoginCustomerCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.CustomerLoginAsync(request.UserName, request.Password);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
