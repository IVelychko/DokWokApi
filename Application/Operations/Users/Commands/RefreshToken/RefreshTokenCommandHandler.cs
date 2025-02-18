using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IUserService userService) 
    : ICommandHandler<RefreshTokenCommand, AuthorizedUserResponse>
{
    public async Task<AuthorizedUserResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await userService.RefreshTokenAsync(request.Token, request.RefreshToken);
    }
}
