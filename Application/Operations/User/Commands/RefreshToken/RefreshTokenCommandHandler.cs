using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IUserService userService) : ICommandHandler<RefreshTokenCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.RefreshTokenAsync(request.Token, request.RefreshToken);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
