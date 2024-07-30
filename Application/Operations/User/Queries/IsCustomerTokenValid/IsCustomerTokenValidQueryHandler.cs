using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.IsCustomerTokenValid;

public class IsCustomerTokenValidQueryHandler(IUserService userService) : IQueryHandler<IsCustomerTokenValidQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(IsCustomerTokenValidQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.IsCustomerTokenValidAsync(request.Token);
        return user?.ToResponse();
    }
}
