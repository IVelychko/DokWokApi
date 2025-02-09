using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.User.Queries.GetUserFromToken;

public class GetUserFromTokenQueryHandler(IUserService userService) : IQueryHandler<GetUserFromTokenQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserFromTokenQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserFromTokenAsync(request.Token);
        return user?.ToResponse();
    }
}
