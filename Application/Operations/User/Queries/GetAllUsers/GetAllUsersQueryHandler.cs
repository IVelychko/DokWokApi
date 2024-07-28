using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserService userService) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserModel>>
{
    public async Task<IEnumerable<UserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) =>
        await userService.GetAllUsersAsync();
}
