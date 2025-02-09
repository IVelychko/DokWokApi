using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(request.Id);
        return user?.ToResponse();
    }
}
