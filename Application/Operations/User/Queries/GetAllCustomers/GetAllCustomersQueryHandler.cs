using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler(IUserService userService) : IQueryHandler<GetAllCustomersQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllCustomersAsync();
        return users.Select(u => u.ToResponse());
    }
}
