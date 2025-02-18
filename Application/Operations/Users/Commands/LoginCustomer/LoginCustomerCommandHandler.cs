using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.LoginCustomer;

public class LoginCustomerCommandHandler(IUserService userService) 
    : ICommandHandler<LoginCustomerCommand, AuthorizedUserResponse>
{
    public async Task<AuthorizedUserResponse> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
    {
        return await userService.LoginAsync(request.UserName);
    }
}
