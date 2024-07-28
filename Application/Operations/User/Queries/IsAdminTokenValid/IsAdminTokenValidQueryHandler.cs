using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.IsAdminTokenValid;

public class IsAdminTokenValidQueryHandler(IUserService userService) : IQueryHandler<IsAdminTokenValidQuery, UserModel?>
{
    public async Task<UserModel?> Handle(IsAdminTokenValidQuery request, CancellationToken cancellationToken) =>
        await userService.IsAdminTokenValidAsync(request.Token);
}
