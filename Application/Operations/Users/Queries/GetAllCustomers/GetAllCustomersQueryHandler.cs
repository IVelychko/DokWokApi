using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.Users.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler(IUserService userService) 
    : IQueryHandler<GetAllCustomersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetAllCustomersAsync();
    }
}
