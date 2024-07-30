using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.IsAdminTokenValid;

public class IsAdminTokenValidQueryHandler(IUserService userService) : IQueryHandler<IsAdminTokenValidQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(IsAdminTokenValidQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.IsAdminTokenValidAsync(request.Token);
        return user?.ToResponse();
    }
}
