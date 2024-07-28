using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(IUserService userService) : IQueryHandler<GetCustomerByIdQuery, UserModel?>
{
    public async Task<UserModel?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken) =>
        await userService.GetCustomerByIdAsync(request.Id);
}
