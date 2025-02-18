using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserService userService) 
    : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetAllUsersAsync();
    }
}
