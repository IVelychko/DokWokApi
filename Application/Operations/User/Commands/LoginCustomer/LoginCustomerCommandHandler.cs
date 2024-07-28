using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginCustomer;

public class LoginCustomerCommandHandler(IUserService userService) : ICommandHandler<LoginCustomerCommand, Result<AuthorizedUserModel>>
{
    public async Task<Result<AuthorizedUserModel>> Handle(LoginCustomerCommand request, CancellationToken cancellationToken) =>
        await userService.CustomerLoginAsync(request.UserName, request.Password);
}
