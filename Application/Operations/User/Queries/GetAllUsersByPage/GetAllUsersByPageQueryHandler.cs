using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;
using Domain.Models;

namespace Application.Operations.User.Queries.GetAllUsersByPage;

public sealed class GetAllUsersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllUsersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var users = await userService.GetAllUsersAsync(pageInfo);
        return users.Select(u => u.ToResponse());
    }
}
