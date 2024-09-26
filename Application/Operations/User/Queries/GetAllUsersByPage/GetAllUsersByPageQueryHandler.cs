using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.User.Queries.GetAllUsersByPage;

public sealed class GetAllUsersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllUsersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var users = await userService.GetAllUsersAsync(pageInfo);
        return users.Select(u => u.ToResponse());
    }
}
