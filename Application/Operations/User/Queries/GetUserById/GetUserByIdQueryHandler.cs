using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, UserModel?>
{
    public async Task<UserModel?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) =>
        await userService.GetUserByIdAsync(request.Id);
}
