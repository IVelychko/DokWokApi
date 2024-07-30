using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetUserByUserName;

public class GetUserByUserNameQueryHandler(IUserService userService) : IQueryHandler<GetUserByUserNameQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByUserNameAsync(request.UserName);
        return user?.ToResponse();
    }
}
