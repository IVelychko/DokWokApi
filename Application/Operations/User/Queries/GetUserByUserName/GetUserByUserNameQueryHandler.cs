using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserByUserName;

public class GetUserByUserNameQueryHandler(IUserService userService) : IQueryHandler<GetUserByUserNameQuery, UserModel?>
{
    public async Task<UserModel?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken) =>
        await userService.GetUserByUserNameAsync(request.UserName);
}
