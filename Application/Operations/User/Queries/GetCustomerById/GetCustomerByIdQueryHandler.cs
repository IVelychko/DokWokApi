using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(IUserService userService) : IQueryHandler<GetCustomerByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await userService.GetCustomerByIdAsync(request.Id);
        return customer?.ToResponse();
    }
}
