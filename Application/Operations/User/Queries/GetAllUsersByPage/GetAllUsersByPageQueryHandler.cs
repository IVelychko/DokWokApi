using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetAllUsersByPage;

public sealed class GetAllUsersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllUsersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersByPageQuery request, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersByPageAsync(request.PageNumber, request.PageSize);
        return users.Select(u => u.ToResponse());
    }
}
