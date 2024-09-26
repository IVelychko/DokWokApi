using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.User.Queries.GetAllCustomersByPage;

public sealed class GetAllCustomersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllCustomersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllCustomersByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var customers = await userService.GetAllCustomersAsync(pageInfo);
        return customers.Select(c => c.ToResponse());
    }
}
