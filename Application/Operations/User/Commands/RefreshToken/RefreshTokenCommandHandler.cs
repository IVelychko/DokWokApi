using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IUserService userService) : ICommandHandler<RefreshTokenCommand, Result<AuthorizedUserModel>>
{
    public async Task<Result<AuthorizedUserModel>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        await userService.RefreshTokenAsync(request.Token, request.RefreshToken);
}
