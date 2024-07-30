using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(request.Id);
        return user?.ToResponse();
    }
}
