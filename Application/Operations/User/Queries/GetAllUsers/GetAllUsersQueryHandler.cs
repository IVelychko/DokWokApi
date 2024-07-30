using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserService userService) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync();
        return users.Select(u => u.ToResponse());
    }
}
