using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.IsCustomerTokenValid;

public class IsCustomerTokenValidQueryHandler(IUserService userService) : IQueryHandler<IsCustomerTokenValidQuery, UserModel?>
{
    public async Task<UserModel?> Handle(IsCustomerTokenValidQuery request, CancellationToken cancellationToken) =>
        await userService.IsCustomerTokenValidAsync(request.Token);
}
