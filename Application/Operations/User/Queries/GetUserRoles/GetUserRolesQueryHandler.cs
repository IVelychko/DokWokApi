using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.User.Queries.GetUserRoles;

public class GetUserRolesQueryHandler(IUserService userService) : IQueryHandler<GetUserRolesQuery, Result<IEnumerable<string>>>
{
    public async Task<Result<IEnumerable<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken) =>
        await userService.GetUserRolesAsync(request.UserId);
}
