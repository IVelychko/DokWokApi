using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.User.Queries.GetAllCustomersByPage;

public sealed class GetAllCustomersByPageQueryHandler(IUserService userService)
    : IQueryHandler<GetAllCustomersByPageQuery, IEnumerable<UserResponse>>
{
    public async Task<IEnumerable<UserResponse>> Handle(GetAllCustomersByPageQuery request, CancellationToken cancellationToken)
    {
        var customers = await userService.GetAllCustomersByPageAsync(request.PageNumber, request.PageSize);
        return customers.Select(c => c.ToResponse());
    }
}
