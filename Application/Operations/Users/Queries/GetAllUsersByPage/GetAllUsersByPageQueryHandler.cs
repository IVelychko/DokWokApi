using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;
using Domain.Models;

namespace Application.Operations.Users.Queries.GetAllUsersByPage;

public sealed class GetAllUsersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllUsersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await userService.GetAllUsersAsync(pageInfo);
    }
}
