using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(IUserService userService) 
    : IQueryHandler<GetCustomerByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetCustomerByIdAsync(request.Id);
    }
}
