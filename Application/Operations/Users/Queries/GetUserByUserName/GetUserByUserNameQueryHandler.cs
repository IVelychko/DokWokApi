using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Queries.GetUserByUserName;

public class GetUserByUserNameQueryHandler(IUserService userService) 
    : IQueryHandler<GetUserByUserNameQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetUserByUserNameAsync(request.UserName);
    }
}
