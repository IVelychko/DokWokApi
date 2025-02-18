using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserService userService) 
    : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetUserByIdAsync(request.Id);
    }
}
