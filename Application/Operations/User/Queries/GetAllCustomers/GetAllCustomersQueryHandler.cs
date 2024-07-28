using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler(IUserService userService) : IQueryHandler<GetAllCustomersQuery, IEnumerable<UserModel>>
{
    public async Task<IEnumerable<UserModel>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken) =>
        await userService.GetAllCustomersAsync();
}
