using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserFromToken;

public class GetUserFromTokenQueryHandler(IUserService userService) : IQueryHandler<GetUserFromTokenQuery, UserModel?>
{
    public async Task<UserModel?> Handle(GetUserFromTokenQuery request, CancellationToken cancellationToken) =>
        await userService.GetUserFromTokenAsync(request.Token);
}
