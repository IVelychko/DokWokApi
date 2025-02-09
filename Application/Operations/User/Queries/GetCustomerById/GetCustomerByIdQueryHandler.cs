using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;

namespace Application.Operations.User.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(IUserService userService) : IQueryHandler<GetCustomerByIdQuery, UserResponse?>
{
    public async Task<UserResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await userService.GetCustomerByIdAsync(request.Id);
        return customer?.ToResponse();
    }
}
