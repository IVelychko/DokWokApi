using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Application.Operations.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IUserService userService) : ICommandHandler<RefreshTokenCommand, Result<AuthorizedUserResponse>>
{
    public async Task<Result<AuthorizedUserResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.RefreshTokenAsync(request.Token, request.RefreshToken);
        return result.Match(au => au.ToResponse(), Result<AuthorizedUserResponse>.Failure);
    }
}
