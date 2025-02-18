using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Users;
using Domain.DTOs.Responses.Users;
using Domain.Models;

namespace Application.Operations.Users.Queries.GetAllCustomersByPage;

public sealed class GetAllCustomersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllCustomersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllCustomersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await userService.GetAllCustomersAsync(pageInfo);
    }
}
