using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(IUserService userService) 
    : ICommandHandler<RegisterUserCommand, AuthorizedUserResponse>
{
    public async Task<AuthorizedUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await userService.RegisterAsync(request);
    }
}
